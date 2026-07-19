using FirstProjectAPI.Dtos.ReponseApprenant;

namespace FirstProjectAPI.Services
{
    public interface IReponseApprenantService
    {
        ResultatDto Repondre(int userId, RepondreDto dto);

        ResultatDto GetResultat(int userId, int idModalite);
    }
}