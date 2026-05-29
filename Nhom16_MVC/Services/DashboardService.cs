using Nhom16_MVC.Repositories.Interfaces;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repo;

    public DashboardService(IDashboardRepository repo)
    {
        _repo = repo;
    }

    public async Task<DashboardResponseDto> GetDashboardAsync(int chuSanId)
    {
        return await _repo.GetDashboardAsync(chuSanId);
    }
}