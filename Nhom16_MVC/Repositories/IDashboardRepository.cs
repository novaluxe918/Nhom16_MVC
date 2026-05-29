using Nhom16_MVC.Models.DTOs;

namespace Nhom16_MVC.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardResponseDto> GetDashboardAsync(int chuSanId);
    }
}