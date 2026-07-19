namespace FirstProjectAPI.Services.Ai
{
    public class ModuleIntrouvableException : Exception
    {
        public ModuleIntrouvableException() : base("Module introuvable.") { }
    }

    public class GeminiApiException : Exception
    {
        public GeminiApiException(string message) : base(message) { }
    }
}