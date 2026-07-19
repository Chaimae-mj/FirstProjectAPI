using FirstProjectAPI.Dtos.Dashboard;
using FirstProjectAPI.Infra;
using FirstProjectAPI.Services.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace FirstProjectAPI.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly MyContext _context;

        public DashboardService(MyContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetStatsAsync()
        {
            return new DashboardDto
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalFormateurs = await _context.Formateurs.CountAsync(),
                TotalFormations = await _context.Formations.CountAsync(),
                TotalModules = await _context.Modules.CountAsync(),
                TotalQuestions = await _context.Questions.CountAsync(),
                TotalSessions = await _context.Sessions.CountAsync(),
                TotalInscriptions = await _context.Set<Dictionary<string, object>>("Inscription").CountAsync()
            };
        }
    }
}