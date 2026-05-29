public interface IReportService
{
    Task<List<RevenueReportDto>> GetRevenueReport(int chuSanId);
}