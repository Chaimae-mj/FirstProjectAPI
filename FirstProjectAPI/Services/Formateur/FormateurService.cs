using FirstProjectAPI.DTOs.Formateur;
using FirstProjectAPI.Infra;
using FirstProjectAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstProjectAPI.Services.Formateur
{
    public class FormateurService : IFormateurService
    {
        private readonly MyContext _context;

        public FormateurService(MyContext context)
        {
            _context = context;
        }

        public async Task<List<FormateurDto>> GetAllAsync()
        {
            return await _context.Formateurs
                .Select(f => new FormateurDto
                {
                    Id = f.Id,
                    Nom = f.Nom,
                    Prenom = f.Prenom,
                    Email = f.Email,
                    Telephone = f.Telephone,
                    Specialite = f.Specialite,
                    Photo = f.Photo
                })
                .ToListAsync();
        }

        public async Task<FormateurDto?> GetByIdAsync(int id)
        {
            var f = await _context.Formateurs.FindAsync(id);

            if (f == null)
                return null;

            return new FormateurDto
            {
                Id = f.Id,
                Nom = f.Nom,
                Prenom = f.Prenom,
                Email = f.Email,
                Telephone = f.Telephone,
                Specialite = f.Specialite,
                Photo = f.Photo
            };
        }

        public async Task<FormateurDto> CreateAsync(CreateFormateurDto dto)
        {
            var formateur = new FirstProjectAPI.Models.Formateur
            {
                Nom = dto.Nom,
                Prenom = dto.Prenom,
                Email = dto.Email,
                Telephone = dto.Telephone,
                Specialite = dto.Specialite,
                Photo = dto.Photo
            };

            _context.Formateurs.Add(formateur);

            await _context.SaveChangesAsync();

            return new FormateurDto
            {
                Id = formateur.Id,
                Nom = formateur.Nom,
                Prenom = formateur.Prenom,
                Email = formateur.Email,
                Telephone = formateur.Telephone,
                Specialite = formateur.Specialite,
                Photo = formateur.Photo
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateFormateurDto dto)
        {
            var formateur = await _context.Formateurs.FindAsync(id);

            if (formateur == null)
                return false;

            formateur.Nom = dto.Nom;
            formateur.Prenom = dto.Prenom;
            formateur.Email = dto.Email;
            formateur.Telephone = dto.Telephone;
            formateur.Specialite = dto.Specialite;
            formateur.Photo = dto.Photo;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var formateur = await _context.Formateurs.FindAsync(id);

            if (formateur == null)
                return false;

            _context.Formateurs.Remove(formateur);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}