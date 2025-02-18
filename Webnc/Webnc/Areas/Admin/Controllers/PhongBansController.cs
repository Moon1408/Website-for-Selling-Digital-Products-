using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webnc.Helpers;
using Webnc.Models;

namespace Webnc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "QuanLyP")]
    public class PhongBansController : Controller
    {
        private readonly WebncContext db;
        private readonly KiemTraQuyen _kiemTraQuyen;

        public PhongBansController(WebncContext context, KiemTraQuyen kiemTraQuyen)
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
        // GET: Admin/PhongBans
        public async Task<IActionResult> Index()
        {
            return db.PhongBans != null ?
                        View(await db.PhongBans.ToListAsync()) :
                        Problem("Entity set 'WebncContext.PhongBans'  is null.");
        }
        // HIỂN THỊ LỖI 
        private List<string> ValidatePB(PhongBan phongBan, bool istenpb)
        {
            var errorMessages = new List<string>();

            if (istenpb)
            {
                errorMessages.Add("Tên phòng ban đã tồn tại. Vui lòng chọn tên phòng ban khác.");
            }

            if (string.IsNullOrWhiteSpace(phongBan.TenPb))
            {
                errorMessages.Add("Bạn chưa điền tên phòng ban");
            }
            return errorMessages;
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Kiểm tra quyền trước khi thực hiện 
                int manv = NVHienTai();
                bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 3, "xem");
                if (!coQuyen)
                {
                    return Json(new { error = "Bạn không có quyền truy cập vào trang này." });

                }
                // Kết thúc KTQ 
                var PhongBans = await db.PhongBans
                .FirstOrDefaultAsync(m => m.MaPb == id);
                var phongCT = new
                {
                    pban = PhongBans.TenPb
                };
                return Json(phongCT);
            }
            catch (Exception ex)
            {
                // Trả về một lỗi trong trường hợp có lỗi xảy ra
                return Json(new { error = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: Admin/PhongBans/Create
        public async Task<IActionResult> Create()
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 3, "them");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            return View();
        }
        // POST: Admin/PhongBans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaPb,TenPb")] PhongBan phongBan)
        {
            // Kiểm tra tên đăng nhập trùng lặp
            var istenpb = await db.PhongBans.AnyAsync(t => t.TenPb == phongBan.TenPb && t.MaPb != phongBan.MaPb);            // Kiểm tra lỗi và lấy các thông báo lỗi
            var errorMessages = ValidatePB(phongBan, istenpb);
            // Thêm các thông báo lỗi vào ModelState
            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }
            if (ModelState.IsValid)
            {
                db.Add(phongBan);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(phongBan);
        }

        // GET: Admin/PhongBans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 3, "sua");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            if (id == null || db.PhongBans == null)
            {
                return NotFound();
            }

            var phongBan = await db.PhongBans.FindAsync(id);
            if (phongBan == null)
            {
                return NotFound();
            }
            return View(phongBan);
        }

        // POST: Admin/PhongBans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaPb,TenPb")] PhongBan phongBan)
        {
            // Kiểm tra tên đăng nhập trùng lặp
            var istenpb = await db.PhongBans.AnyAsync(t => t.TenPb == phongBan.TenPb && t.MaPb != phongBan.MaPb);            // Kiểm tra lỗi và lấy các thông báo lỗi
            var errorMessages = ValidatePB(phongBan, istenpb);
            // Thêm các thông báo lỗi vào ModelState
            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }
            if (id != phongBan.MaPb)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(phongBan);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongBanExists(phongBan.MaPb))
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
            return View(phongBan);
        }

        // GET: Admin/PhongBans/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 3, "xoa");
            if (!coQuyen)
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập vào trang này." });

            }
            // Kết thúc KTQ 
            // Kiểm tra xem trang có liên kết với bảng NhanVien không
            var NhanVien = await db.NhanViens.AnyAsync(pq => pq.MaPb == id);
            if (NhanVien)
            {
                return Json(new { success = false, message = "Không thể xóa, PhongBan đang tồn tại trong bảng NhanVien." });
            }
            // Xóa trang và lưu thay đổi vào cơ sở dữ liệu
            var phongBan = await db.PhongBans.FindAsync(id);
            if (phongBan != null)
            {
                db.PhongBans.Remove(phongBan);
            }
            await db.SaveChangesAsync();
            return Json(new { success = true});
        }
        private bool PhongBanExists(int id)
        {
          return (db.PhongBans?.Any(e => e.MaPb == id)).GetValueOrDefault();
        }
    }
}
