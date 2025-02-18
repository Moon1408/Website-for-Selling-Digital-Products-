using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Area("Admin")]
public class ThongKeController : Controller
{
    private readonly ChartService _chartService;

    public ThongKeController(ChartService chartService)
    {
        _chartService = chartService;
    }

    public IActionResult ThongKe()
    {
        return View();
    }

    [HttpGet("ThongKe/SoLuongTheoThuongHieu")]
    public async Task<IActionResult> LaySoLuongTheoThuongHieu()
    {
        var data = await _chartService.LaySoLuongTheoThuongHieu();
        return Json(data);
    }

    [HttpGet("ThongKe/SoLuongTheoLoai")]
    public async Task<IActionResult> LaySoLuongTheoLoai()
    {
        var data = await _chartService.LaySoLuongTheoLoai();
        return Json(data);
    }

    [HttpGet("ThongKe/SoLuongKhachHang")]
    public async Task<IActionResult> LaySoLuongKhachHang()
    {
        var count = await _chartService.LaySoLuongKhachHang();
        return Json(count);
    }

    [HttpGet("ThongKe/DuLieuDonHangTheoTrangThai")]
    public async Task<IActionResult> LayDuLieuDonHangTheoTrangThai()
    {
        var data = await _chartService.LayDuLieuDonHangTheoTrangThai();
        return Json(data);
    }


    [HttpGet("ThongKe/SoLuongSanPham")]
    public async Task<IActionResult> LaySoLuongSanPham()
    {
        var count = await _chartService.LaySoLuongSanPham();
        return Json(count);
    }
}

