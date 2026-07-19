namespace FirstProjectAPI.Services.Session
    {
    public class SessionNotFoundException : Exception
    {
        public SessionNotFoundException() : base("Session introuvable.") { }
    }

    public class FormationIntrouvableException : Exception
    {
        public FormationIntrouvableException() : base("Formation introuvable.") { }
    }

    public class DejaInscritException : Exception
    {
        public DejaInscritException() : base("Vous êtes déjà inscrit à cette session.") { }
    }
}