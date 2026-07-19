using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace FirstProjectAPI.Services.Avatar
{
    // ATTENTION : stockage en mémoire du processus. Si vous déployez avec plusieurs
    // instances (scaling horizontal), il faudra remplacer ceci par un store partagé
    // (Redis, etc.). Suffisant pour un déploiement mono-instance (ex: Railway starter).
    public class AvatarSessionManager
    {
        private readonly ConcurrentDictionary<int, AvatarSession> _sessions = new();

        public bool TryAdd(int userId, AvatarSession session) => _sessions.TryAdd(userId, session);

        public bool TryGet(int userId, out AvatarSession? session) => _sessions.TryGetValue(userId, out session);

        public bool TryRemove(int userId, out AvatarSession? session) => _sessions.TryRemove(userId, out session);
    }

    public class AvatarSession
    {
        public required string SessionId { get; init; }
        public required ClientWebSocket Socket { get; init; }
        public required CancellationTokenSource KeepAliveCts { get; init; }
    }
}