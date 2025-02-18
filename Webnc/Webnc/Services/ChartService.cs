using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webnc.Models;

public class ChartService
{
    private readonly WebncContext _context;

    public ChartService(WebncContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SoLuongTheoThuongHieu>> LaySoLuongTheoThuongHieu()
    {
        return await _context.HangHoas
                             .GroupBy(h => h.MaThNavigation.TenTh)
                             .Select(g => new SoLuongTheoThuongHieu
                             {
                                 TenThuongHieu = g.Key,
                                 SoLuongSanPham = g.Count()
                             })
                             .ToListAsync();
    }
    public async Task<IEnumerable<SoLuongTheoLoai>> LaySoLuongTheoLoai()
    {
        return await _context.HangHoas
                             .GroupBy(h => h.MaLoaiNavigation.TenLoai)
                             .Select(g => new SoLuongTheoLoai
                             {
                                 TenLoai = g.Key,
                                 SoLuongSanPham = g.Count()
                             })
                             .ToListAsync();
    }

    public async Task<int> LaySoLuongKhachHang()
    {
        return await _context.KhachHangs.CountAsync();
    }

    public async Task<IEnumerable<DuLieuDonHang>> LayDuLieuDonHangTheoTrangThai()
    {
        return await _context.HoaDons
                             .Join(_context.TrangThais, hd => hd.MaTrangThai, tt => tt.MaTrangThai, (hd, tt) => new { HoaDon = hd, TrangThai = tt })
                             .GroupBy(x => x.TrangThai.TenTrangThai)
                             .Select(g => new DuLieuDonHang
                             {
                                 TenTrangThai = g.Key,
                                 SoLuongDonHang = g.Count()
                             })
                             .ToListAsync();
    }
    public async Task<int> LaySoLuongSanPham()
    {
        return await _context.HangHoas.CountAsync();
    }
}
public class SoLuongTheoThuongHieu
{
    public string TenThuongHieu { get; set; }
    public int SoLuongSanPham { get; set; }
}

public class SoLuongTheoLoai
{
    public string TenLoai { get; set; }
    public int SoLuongSanPham { get; set; }
}

public class DuLieuDonHang
{
    public int MaTrangThai { get; set; }
    public string TenTrangThai { get; set; }
    public int SoLuongDonHang { get; set; }

}

