using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webnc.Helpers;
using Webnc.Models;
using static NuGet.Packaging.PackagingConstants;

namespace Webnc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "CombinedPolicy")]

    public class KhachHangAdminController : Controller
    {
        private readonly WebncContext db;
        private readonly KiemTraQuyen _kiemTraQuyen;

        public KhachHangAdminController(WebncContext context, KiemTraQuyen kiemTraQuyen)
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
        // GET: Admin/KhachHangAdmin
        public async Task<IActionResult> Index()
        {
            return db.KhachHangs != null ?
                        View(await db.KhachHangs.ToListAsync()) :
                        Problem("Entity set 'WebncContext.KhachHangs'  is null.");
        }
        // GET: Admin/KhachHangAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 7, "xem");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            if (id == null || db.KhachHangs == null)
            {
                return NotFound();
            }

            var khachHang = await db.KhachHangs
                .FirstOrDefaultAsync(m => m.MaKh == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }
        // GET: Admin/KhachHangAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 7, "sua");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            if (id == null || db.KhachHangs == null)
            {
                return NotFound();
            }

            var khachHang = await db.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return View(khachHang);
        }

        // POST: Admin/KhachHangAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKh,TenDn,MatKhau,HoTen,DiaChi,DienThoai,Email,HieuLuc,VaiTro")] KhachHang khachHang)
        {
            if (id != khachHang.MaKh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(khachHang);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachHangExists(khachHang.MaKh))
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
            return View(khachHang);
        }
        // POST: Admin/KhachHangAdmin/Delete/5
       
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 7, "x");
            if (!coQuyen)
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập vào trang này." });
            }
            // Kết thúc KTQ 
            // Kiểm tra HoaDon nếu khách hàng đó có 1 đơn hàng là đã hủy ( -1 )
            // Cho xóa
            // Nhiều đơn hơn trạng thái khác không cho xóa 
            bool hoadonTT = await db.HoaDons.AnyAsync(pq => pq.MaKh == id &&
                                                             (pq.MaTrangThai == 0 ||
                                                              pq.MaTrangThai == 1 ||
                                                              pq.MaTrangThai == 2));
            if (hoadonTT)
            {
                return Json(new { success = false, message = "Không thể xóa, KhachHang đang tồn tại trong HoaDon." });
            }
            // Xóa trang và lưu thay đổi vào cơ sở dữ liệu
            var khachHang = await db.KhachHangs.FindAsync(id);
            if (khachHang != null)
            {
                db.KhachHangs.Remove(khachHang);
            }
            await db.SaveChangesAsync();
            return Json(new { success = true});
        }
        private bool KhachHangExists(int id)
        {
            return (db.KhachHangs?.Any(e => e.MaKh == id)).GetValueOrDefault();
        } 
    }
}
