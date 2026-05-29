using Microsoft.EntityFrameworkCore;
using Nhom16_MVC.Data;
using Nhom16_MVC.Models.DTOs;
using Nhom16_MVC.Models.Entities;
using Nhom16_MVC.Models.Enums;
using Nhom16_MVC.Repositories;

namespace Nhom16_MVC.Services
{
    public class SanBongChiTietService : ISanBongChiTietService
    {
        private readonly AppDbContext _context;

        public SanBongChiTietService(AppDbContext context)
        {
            _context = context;
        }

         private readonly ISanBongChiTietRepository _repo;

        public SanBongChiTietService(ISanBongChiTietRepository repo)
        {
            _repo = repo;
        }

        //lấy thông tin cơ bản của sân con 
        public async Task<ChiTietSanConLichDto> GetThongTinSanConAsync(int maSanChiTiet)
        {
            var sanCon = await _context.sanbongchitiet
                .Include(sc=>sc.masanbongNavigation)
                .Include(sc=>sc.maloaisanNavigation)
                .Include(sc=>sc.media_sanbongchitiet)
                .Include(sc=>sc.danhgia)
                .FirstOrDefaultAsync(sc=>sc.masanchitiet == maSanChiTiet);

            if (sanCon == null || sanCon.masanbongNavigation?.daduyet != true) return null;

            var danhGias = sanCon.danhgia.ToList();
            decimal diemTb = danhGias.Count > 0 ? Math.Round((decimal)danhGias.Average(d => (double)d.diemso), 1) : 0;


            return new ChiTietSanConLichDto
            {
                MaSanChiTiet = sanCon.masanchitiet,
                TenSanChiTiet = sanCon.tensanchitiet,
                LoaiSan = sanCon.maloaisanNavigation?.tenloaisan ?? "Chưa phân loại",
                GiaBuoiSang = sanCon.giathuebuoisang,
                GiaBuoiToi = sanCon.giathuebuoitoi,
                MaSanBongMenge = sanCon.masanbong,
                TenSanMenge = sanCon.masanbongNavigation.tensan,
                DiaChiMenge = sanCon.masanbongNavigation.diachi,
                DiemTrungBinh = diemTb,
                TongSoBinhLuan = danhGias.Count,
                GioHoatDong = $"{sanCon.masanbongNavigation.giomocua:HH:mm} - {sanCon.masanbongNavigation.giodongcua:HH:mm}",
                AlbumMediaSanCon = sanCon.media_sanbongchitiet.Select(m => m.link).ToList()
            };
        }

        //lấy lịch trống theo ngày của user chọn từ calender
     
         public async Task<List<sanbongchitiet>> GetAll()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<List<sanbongchitiet>> GetBySanBong(int masanbong)
        {
            return await _repo.GetBySanBongIdAsync(masanbong);
        }

        public async Task<bool> Create(CreateSanConDTO dto)
        {
            var entity = new sanbongchitiet
            {
                masanbong = dto.masanbong,
                maloaisan = dto.maloaisan,
                tensanchitiet = dto.tensanchitiet,
                giathuebuoisang = dto.giathuebuoisang,
                giathuebuoitoi = dto.giathuebuoitoi
            };

            await _repo.AddAsync(entity);
            await _repo.SaveAsync();

            return true;
        }

        public async Task<bool> Update(int id, UpdateSanConDTO dto)
        {
            var data = await _repo.GetByIdAsync(id);

            if (data == null) return false;

            data.tensanchitiet = dto.tensanchitiet;
            data.giathuebuoisang = dto.giathuebuoisang;
            data.giathuebuoitoi = dto.giathuebuoitoi;

            await _repo.SaveAsync();
            return true;
        }

        public async Task<bool> Delete(int id)
        {
            var data = await _repo.GetByIdAsync(id);

            if (data == null) return false;

            await _repo.DeleteAsync(data);
            await _repo.SaveAsync();

            return true;
        }
    
        public async Task<LichTrongTheoNgayDto?> GetLichTrongTheoNgayAsync(int maSanChiTiet, DateOnly ngayChon)
        {
            var sanCon = await _context.sanbongchitiet
                .Include(sc => sc.masanbongNavigation)
                .FirstOrDefaultAsync(sc => sc.masanchitiet == maSanChiTiet);

            if (sanCon == null || sanCon.masanbongNavigation == null)
                return null;

            var batDauNgay = ngayChon.ToDateTime(TimeOnly.MinValue);
            var ketThucNgay = batDauNgay.AddDays(1);

            var lichDaDat = await _context.chitietdatsan
                .Where(c => c.masanchitiet == maSanChiTiet &&
                            c.giobatdau >= batDauNgay &&
                            c.giobatdau < ketThucNgay &&
                            c.trangthaidatsan != TrangThaiDatEnum.DaHuy)
                .ToListAsync();

            var result = new LichTrongTheoNgayDto
            {//
                Ngay = ngayChon.ToString("dd/MM/yyyy"), 
                Slots = new List<SlotGioTrangThaiDto>()
            };

            var timeStart = batDauNgay.Add(sanCon.masanbongNavigation.giomocua.ToTimeSpan());
            var timeEnd = batDauNgay.Add(sanCon.masanbongNavigation.giodongcua.ToTimeSpan());

            var currentSlot = timeStart;
            while (currentSlot.AddHours(1) <= timeEnd)
            {
                var nextSlot = currentSlot.AddHours(1);

                // Slot này có bị cắt ngang bởi đơn đặt nào trong CSDL không?
                bool biTrung = lichDaDat.Any(b => b.giobatdau < nextSlot && b.gioketthuc > currentSlot);

                // Nếu ngày user chọn là HÔM NAY, thì các slot trong QUÁ KHỨ (trước giờ hiện tại) phải bị mờ (không cho đặt)
                bool laQuaKhu = currentSlot < DateTime.Now;

                result.Slots.Add(new SlotGioTrangThaiDto
                {
                    GioBatDau = currentSlot.ToString("HH:mm"),
                    GioKetThuc = nextSlot.ToString("HH:mm"),
                    ConTrong = !biTrung && !laQuaKhu
                });

                currentSlot = nextSlot;
            }

            return result;
        }
    }
}
