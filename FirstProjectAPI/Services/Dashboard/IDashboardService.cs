using FirstProjectAPI.Dtos.Dashboard;

namespace FirstProjectAPI.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetStatsAsync();
    }
}