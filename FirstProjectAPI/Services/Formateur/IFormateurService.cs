using FirstProjectAPI.DTOs.Formateur;

namespace FirstProjectAPI.Services.Formateur
{
    public interface IFormateurService
    {
        Task<List<FormateurDto>> GetAllAsync();

        Task<FormateurDto?> GetByIdAsync(int id);

        Task<FormateurDto> CreateAsync(CreateFormateurDto dto);

        Task<bool> UpdateAsync(int id, UpdateFormateurDto dto);

        Task<bool> DeleteAsync(int id);
    }
}