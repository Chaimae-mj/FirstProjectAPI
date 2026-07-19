namespace FirstProjectAPI.Dtos.Ai
{
    public class ExplainModuleResponseDto
    {
        public int IdModule { get; set; }

        public string ModuleTitre { get; set; } = string.Empty;

        public string Explication { get; set; } = string.Empty;
    }
}