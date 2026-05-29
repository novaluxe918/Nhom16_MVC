public interface IDashboardService
{
    Task<DashboardResponseDto> GetDashboardAsync(int chuSanId);
}