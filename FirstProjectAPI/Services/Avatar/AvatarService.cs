using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using FirstProjectAPI.Dtos.Avatar;
using FirstProjectAPI.Services.Ai;

namespace FirstProjectAPI.Services.Avatar
{
    public class AvatarService : IAvatarService
    {
        private const string LiveAvatarBaseUrl = "https://api.liveavatar.com/v1";
        private const string TtsBaseUrl = "https://api.elevenlabs.io/v1/text-to-speech";

        private readonly HttpClient _httpClient;
        private readonly IAiService _aiService;
        private readonly AvatarSessionManager _sessionManager;

        private readonly string _heyGenApiKey;
        private readonly string _avatarId;
        private readonly string _mode;

        private readonly string _elevenLabsApiKey;
        private readonly string _elevenLabsVoiceId;
        private readonly string _elevenLabsModelId;

        public AvatarService(
            HttpClient httpClient,
            IAiService aiService,
            AvatarSessionManager sessionManager,
            IConfiguration config)
        {
            _httpClient = httpClient;
            _aiService = aiService;
            _sessionManager = sessionManager;

            var avatarSection = config.GetSection("Avatar");
            _heyGenApiKey = avatarSection["HeyGenApiKey"]
                ?? throw new InvalidOperationException("Clé API HeyGen manquante (Avatar:HeyGenApiKey).");
            _avatarId = avatarSection["AvatarId"]
                ?? throw new InvalidOperationException("AvatarId manquant (Avatar:AvatarId).");
            _mode = avatarSection["Mode"] ?? "LITE";

            var ttsSection = config.GetSection("Tts");
            _elevenLabsApiKey = ttsSection["ElevenLabsApiKey"]
                ?? throw new InvalidOperationException("Clé API ElevenLabs manquante (Tts:ElevenLabsApiKey)."); _elevenLabsVoiceId = ttsSection["VoiceId"] ?? "21m00Tcm4TlvDq8ikWAM";
            _elevenLabsModelId = ttsSection["ModelId"] ?? "eleven_flash_v2_5";
        }

        public async Task<StartSessionResponseDto> StartSessionAsync(int userId)
        {
            if (_sessionManager.TryGet(userId, out _))
                throw new AvatarSessionAlreadyActiveException();

            // 1) Créer un jeton de session HeyGen
            var tokenPayload = new
            {
                mode = _mode,
                avatar_id = _avatarId,
                video_settings = new { encoding = "VP8", quality = "high" },
                is_sandbox = false
            };

            var tokenResult = await PostToHeyGenAsync("/sessions/token", tokenPayload, apiKeyAuth: true);
            var sessionId = tokenResult.RootElement.GetProperty("data").GetProperty("session_id").GetString()!;
            var sessionToken = tokenResult.RootElement.GetProperty("data").GetProperty("session_token").GetString()!;

            // 2) Démarrer la session : ce n'est PAS la clé API qui authentifie cet appel,
            // mais le session_token obtenu à l'étape 1, envoyé en Bearer.
            var startPayload = new { session_token = sessionToken };
            var startResult = await PostToHeyGenAsync("/sessions/start", startPayload, apiKeyAuth: false, bearerToken: sessionToken);
            var data = startResult.RootElement.GetProperty("data");

            var livekitUrl = data.GetProperty("livekit_url").GetString()!;
            var livekitClientToken = data.GetProperty("livekit_client_token").GetString()!;
            var wsUrl = data.GetProperty("ws_url").GetString()!;
            var maxDuration = data.TryGetProperty("max_session_duration", out var d) ? d.GetInt32() : 0;

            // 3) Ouvrir le WebSocket de contrôle (LITE/CUSTOM mode) et le garder ouvert côté serveur.
            // NOTE : on n'attend PAS ici un état "connected" bloquant, car cet état ne survient
            // que lorsqu'un participant (le frontend, via le SDK LiveKit) a rejoint la room vidéo.
            // Sans frontend connecté, HeyGen n'émettra jamais cet événement, et on bloquerait
            // indéfiniment. On ouvre donc le socket de façon optimiste ; le frontend devra
            // rejoindre livekitUrl/livekitClientToken avant que "speak" ait un effet visible.
            var socket = new ClientWebSocket();
            await socket.ConnectAsync(new Uri(wsUrl), CancellationToken.None);

            var keepAliveCts = new CancellationTokenSource();
            var session = new AvatarSession
            {
                SessionId = sessionId,
                Socket = socket,
                KeepAliveCts = keepAliveCts
            };

            if (!_sessionManager.TryAdd(userId, session))
            {
                keepAliveCts.Cancel();
                socket.Dispose();
                throw new AvatarSessionAlreadyActiveException();
            }

            _ = RunKeepAliveLoopAsync(socket, keepAliveCts.Token);
            _ = RunListenLoopAsync(socket, keepAliveCts.Token);

            return new StartSessionResponseDto
            {
                SessionId = sessionId,
                LivekitUrl = livekitUrl,
                LivekitClientToken = livekitClientToken,
                MaxSessionDuration = maxDuration
            };
        }

        public async Task<string> SpeakAsync(int userId, string question)
        {
            if (!_sessionManager.TryGet(userId, out var session) || session == null)
                throw new AvatarSessionNotFoundException();

            // 1) Gemini génère la réponse texte
            var reponseTexte = await _aiService.AskAsync(question);

            // 2) TTS : convertit le texte en audio PCM 16-bit / 24kHz (format exigé par agent.speak)
            var pcmAudio = await SynthesizeSpeechAsync(reponseTexte);

            // 3) Envoie l'audio à l'avatar en morceaux d'environ 1 seconde
            var eventId = Guid.NewGuid().ToString();
            const int bytesPerSecond = 24000 * 2; // 24kHz * 16-bit (2 octets/échantillon), mono

            for (var offset = 0; offset < pcmAudio.Length; offset += bytesPerSecond)
            {
                var chunkLength = Math.Min(bytesPerSecond, pcmAudio.Length - offset);
                var chunk = new byte[chunkLength];
                Array.Copy(pcmAudio, offset, chunk, 0, chunkLength);

                var speakCommand = new
                {
                    type = "agent.speak",
                    audio = Convert.ToBase64String(chunk),
                    event_id = eventId
                };

                await SendWsCommandAsync(session.Socket, speakCommand);
            }

            await SendWsCommandAsync(session.Socket, new
            {
                type = "agent.speak_end",
                event_id = eventId
            });

            return reponseTexte;
        }

        public async Task StopAsync(int userId)
        {
            if (!_sessionManager.TryRemove(userId, out var session) || session == null)
                throw new AvatarSessionNotFoundException();

            session.KeepAliveCts.Cancel();

            try
            {
                if (session.Socket.State == WebSocketState.Open)
                {
                    await session.Socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure, "Session terminée par l'utilisateur", CancellationToken.None);
                }
            }
            finally
            {
                session.Socket.Dispose();
            }
        }

        // ----------------- Helpers privés -----------------

        private async Task<JsonDocument> PostToHeyGenAsync(string path, object payload, bool apiKeyAuth, string? bearerToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, LiveAvatarBaseUrl + path)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };

            if (apiKeyAuth)
            {
                request.Headers.Add("X-API-KEY", _heyGenApiKey);
            }
            else if (!string.IsNullOrEmpty(bearerToken))
            {
                request.Headers.Add("Authorization", $"Bearer {bearerToken}");
            }

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HeyGenApiException($"Erreur HeyGen ({(int)response.StatusCode}) sur {path} : {content}");

            return JsonDocument.Parse(content);
        }

        private async Task<byte[]> SynthesizeSpeechAsync(string text)
        {
            var payload = new
            {
                text,
                model_id = _elevenLabsModelId
            };

            var url = $"{TtsBaseUrl}/{_elevenLabsVoiceId}?output_format=pcm_24000";
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("xi-api-key", _elevenLabsApiKey);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new TtsApiException($"Erreur ElevenLabs ({(int)response.StatusCode}) : {errorContent}");
            }

            // Avec output_format=pcm_24000, ElevenLabs renvoie directement l'audio PCM brut
            // (S16LE, 24kHz, mono) dans le corps de la réponse — pas de JSON, pas d'en-tête à retirer.
            return await response.Content.ReadAsByteArrayAsync();
        }

        private static async Task SendWsCommandAsync(ClientWebSocket socket, object command)
        {
            var json = JsonSerializer.Serialize(command);
            var bytes = Encoding.UTF8.GetBytes(json);
            await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private static async Task RunListenLoopAsync(ClientWebSocket socket, CancellationToken token)
        {
            var buffer = new byte[8192];

            try
            {
                while (!token.IsCancellationRequested && socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(buffer, token);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine($"[AVATAR-DEBUG] WebSocket fermé par HeyGen. CloseStatus={result.CloseStatus}, Description={result.CloseStatusDescription}");
                        break;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"[AVATAR-DEBUG] Message WS reçu : {message}");
                }
            }
            catch (OperationCanceledException)
            {
                // Arrêt normal lors de StopAsync.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AVATAR-DEBUG] Erreur d'écoute WebSocket : {ex.Message}");
            }
        }

        private static async Task RunKeepAliveLoopAsync(ClientWebSocket socket, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested && socket.State == WebSocketState.Open)
                {
                    await Task.Delay(TimeSpan.FromMinutes(4), token);

                    if (socket.State != WebSocketState.Open)
                        break;

                    await SendWsCommandAsync(socket, new
                    {
                        type = "session.keep_alive",
                        event_id = Guid.NewGuid().ToString()
                    });
                }
            }
            catch (OperationCanceledException)
            {
                // Arrêt normal lors de StopAsync.
            }
            catch (Exception)
            {
                // Le socket a probablement été fermé côté serveur HeyGen ; rien à faire de plus ici.
            }
        }
    }
}