namespace FirstProjectAPI.Dtos.Module
{
    public class ModuleDto
    {
        public int Id { get; set; }

        public string Titre { get; set; } = string.Empty;

        public int Ordre { get; set; }

        public int IdFormation { get; set; }

        public string? Formation { get; set; }
    }
}