namespace FirstProjectAPI.Dtos.Session
{
    public class SessionDto
    {
        public int Id { get; set; }

        public DateTime DateDebut { get; set; }

        public int Duree { get; set; }

        // Calculée : DateDebut + Duree (en jours). Adapter si Duree représente des heures.
        public DateTime DateFin { get; set; }

        public int IdFormation { get; set; }

        public string? FormationNom { get; set; }

        public int NombreInscrits { get; set; }
    }
}