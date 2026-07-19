namespace FirstProjectAPI.Services.Avatar
{
    public class AvatarSessionNotFoundException : Exception
    {
        public AvatarSessionNotFoundException()
            : base("Aucune session avatar active. Démarrez une session avec /api/avatar/session/start d'abord.") { }
    }

    public class AvatarSessionAlreadyActiveException : Exception
    {
        public AvatarSessionAlreadyActiveException()
            : base("Une session avatar est déjà active pour cet utilisateur. Arrêtez-la avant d'en démarrer une nouvelle.") { }
    }

    public class HeyGenApiException : Exception
    {
        public HeyGenApiException(string message) : base(message) { }
    }

    public class TtsApiException : Exception
    {
        public TtsApiException(string message) : base(message) { }
    }
}