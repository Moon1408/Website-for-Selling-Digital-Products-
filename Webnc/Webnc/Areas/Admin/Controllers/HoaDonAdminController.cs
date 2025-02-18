using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Webnc.Helpers;
using Webnc.Models;
using Webnc.ViewModels;

namespace Webnc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "CombinedPolicy")]
    public class HoaDonAdminController : Controller
    {
        private readonly WebncContext db;
        private readonly KiemTraQuyen _kiemTraQuyen;

        public HoaDonAdminController(WebncContext context, KiemTraQuyen kiemTraQuyen)
        {
            db = context;
            _kiemTraQuyen = kiemTraQuyen;
        }
        // Phương thức để lấy mã nhân viên hiện tại
        private int NVHienTai()
        {
            var manvClaim = User.FindFirst(MySetting.CLAIM_MANV);
            if (manvClaim != null && int.TryParse(manvClaim.Value, out int manv))
            {
                return manv;
            }
            throw new Exception("Không thể xác định mã nhân viên.");
        }
        // GET: Admin/HoaDonAdmin/Index
        public async Task<IActionResult> Index()
        {
            var webncContext = db.HoaDons.Include(h => h.MaKhNavigation).Include(h => h.MaTrangThaiNavigation);
            return View(await webncContext.ToListAsync());
        }
        // GET: Admin/HoaDonAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 6, "xem");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            if (id == null || db.HoaDons == null)
            {
                return NotFound();
            }

            var hoaDon = await db.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);

            if (hoaDon == null)
            {
                return NotFound();
            }

            var chiTietHds = await db.ChiTietHds
                .Where(ct => ct.MaHd == id)
                .Include(ct => ct.MaHhNavigation) // Bao gồm thuộc tính điều hướng
                .ToListAsync();

            var viewModel = new HoaDonDetailsViewModel
            {
                HoaDon = hoaDon,
                ChiTietHds = chiTietHds
            };

            return View(viewModel);
        }



        // GET: Admin/HoaDonAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 6, "sua");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            if (id == null || db.HoaDons == null)
            {
                return NotFound();
            }

            var hoaDon = await db.HoaDons
                .Include(h => h.MaKhNavigation)
                .Include(h => h.MaTrangThaiNavigation)
                .FirstOrDefaultAsync(m => m.MaHd == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            var viewModel = new HoaDonViewModel
            {
                MaHd = hoaDon.MaHd,
                MaKh = hoaDon.MaKh,
                NgayDat = hoaDon.NgayDat,
                HoTen = hoaDon.HoTen,
                DiaChi = hoaDon.DiaChi,
                DienThoai = hoaDon.DienThoai,
                CachThanhToan = hoaDon.CachThanhToan,
                TrangThaiTt = hoaDon.TrangThaiTt,
                ThanhTien = hoaDon.ThanhTien,
                PhiVanChuyen = hoaDon.PhiVanChuyen,
                TongTien = hoaDon.TongTien,
                GhiChu = hoaDon.GhiChu,
                MaTrangThai = hoaDon.MaTrangThai,
                MaKhNavigation = hoaDon.MaKhNavigation,
                MaTrangThaiNavigation = hoaDon.MaTrangThaiNavigation
            };

            ViewData["MaTrangThai"] = new SelectList(db.TrangThais, "MaTrangThai", "TenTrangThai", hoaDon.MaTrangThai);

            return View(viewModel);
        }

        // POST: Admin/HoaDonAdmin/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaHd,MaTrangThai")] UpdateTrangThaiViewModel updateTrangThaiViewModel)
        {
            if (id != updateTrangThaiViewModel.MaHd)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var hoaDon = await db.HoaDons.FindAsync(id);
                    if (hoaDon == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra nếu MaTrangThai là 1 và có yêu cầu cập nhật thành 0
                    if (hoaDon.MaTrangThai == 1 && updateTrangThaiViewModel.MaTrangThai == 0)
                    {
                        TempData["ErrorMessage"] = "Đơn hàng không thể chuyển từ trạng thái đã xác nhận sang trạng thái chờ xác nhận";
                        return RedirectToAction(nameof(Edit), new { id });
                    }

                    // Kiểm tra nếu MaTrangThai là -1
                    if (updateTrangThaiViewModel.MaTrangThai == -1)
                    {
                        // Kiểm tra nếu MaTrangThai hiện tại là 0 và TrangThaiTt là "đã thanh toán"
                        if (hoaDon.MaTrangThai == 0 && hoaDon.TrangThaiTt == "Đã thanh toán")
                        {
                            // Không cho chuyển đổi MaTrangThai sang -1
                            // Đặt thông báo lỗi vào TempData
                            TempData["ErrorMessage"] = "Đơn hàng này không thể hủy.";
                            return RedirectToAction(nameof(Edit), new { id });
                        }
                        else
                        {
                            // Cập nhật số lượng sản phẩm từ đơn hàng đã hủy
                            var chiTietHoaDons = await db.ChiTietHds.Where(ct => ct.MaHd == id).ToListAsync();
                            foreach (var chiTiet in chiTietHoaDons)
                            {
                                var sanPham = await db.HangHoas.FindAsync(chiTiet.MaHh);
                                if (sanPham != null)
                                {
                                    sanPham.Sl += chiTiet.SoLuong;
                                    db.Update(sanPham);
                                }
                            }
                        }
                    }

                    // Cập nhật trạng thái đơn hàng
                    hoaDon.MaTrangThai = updateTrangThaiViewModel.MaTrangThai;
                    db.Update(hoaDon);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(updateTrangThaiViewModel.MaHd))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaTrangThai"] = new SelectList(db.TrangThais, "MaTrangThai", "TenTrangThai", updateTrangThaiViewModel.MaTrangThai);

            return View(updateTrangThaiViewModel);
        }
        private bool HoaDonExists(int id)
        {
            return db.HoaDons.Any(e => e.MaHd == id);
        }
    }
}
