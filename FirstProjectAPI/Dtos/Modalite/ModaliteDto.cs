using FirstProjectAPI.Models;

namespace FirstProjectAPI.Dtos.Modalite
{
    public class ModaliteDto
    {
        public int Id { get; set; }

        public string Titre { get; set; } = string.Empty;

        public string? Contenu { get; set; }

        public TypeModalite Type { get; set; }

        public int Ordre { get; set; }

        public int IdModule { get; set; }

        public string? Module { get; set; }
    }
}