namespace FirstProjectAPI.Dtos.ReponseApprenant
{
    public class ResultatDto
    {
        public int TotalQuestions { get; set; }

        public int BonnesReponses { get; set; }

        public int MauvaisesReponses { get; set; }

        public double Pourcentage { get; set; }
    }
}