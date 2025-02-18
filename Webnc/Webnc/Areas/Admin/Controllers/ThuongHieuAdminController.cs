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

namespace Webnc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "CombinedPolicy")]
    public class ThuongHieuAdminController : Controller
    {
        private readonly WebncContext db;
        private readonly KiemTraQuyen _kiemTraQuyen;

        public ThuongHieuAdminController(WebncContext context, KiemTraQuyen kiemTraQuyen)
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
        // HIỂN THỊ LỖI 
        private List<string> ValidateTH(ThuongHieu thuongHieu, bool istenTH)
        {
            var errorMessages = new List<string>();

            if (istenTH)
            {
                errorMessages.Add("Tên thương hiệu đã tồn tại. Vui lòng chọn tên thương hiệu khác.");
            }

            if (string.IsNullOrWhiteSpace(thuongHieu.TenTh))
            {
                errorMessages.Add("Bạn chưa điền tên thương hiệu");
            }
            return errorMessages;
        }

        // GET: Admin/ThuongHieuAdmin
        public async Task<IActionResult> Index()
        {
              return db.ThuongHieus != null ? 
                          View(await db.ThuongHieus.ToListAsync()) :
                          Problem("Entity set 'WebncContext.ThuongHieus'  is null.");
        }

        // GET: Admin/ThuongHieuAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                // Kiểm tra quyền trước khi thực hiện 
                int manv = NVHienTai();
                bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 9, "xem");
                if (!coQuyen)
                {
                    return Json(new { error = "Bạn không có quyền truy cập vào trang này." });

                }
                // Kết thúc KTQ 
                var thuongHieuT = await db.ThuongHieus.FirstOrDefaultAsync(m => m.MaTh == id);
                var thuongHieuCT = new
                {
                    thuonghieu = thuongHieuT.TenTh
                };
                return Json(thuongHieuCT);
            }
            catch (Exception ex)
            {
                // Trả về một lỗi trong trường hợp có lỗi xảy ra
                return Json(new { error = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: Admin/ThuongHieuAdmin/Create
        public async Task<IActionResult> Create()
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 9, "them");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            return View();
        }

        // POST: Admin/ThuongHieuAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTh,TenTh")] ThuongHieu thuongHieu)
        {
            // Kiểm tra tên đăng nhập trùng lặp
            var istenTH = await db.ThuongHieus.AnyAsync(t => t.TenTh == thuongHieu.TenTh && t.MaTh != thuongHieu.MaTh);
            var errorMessages = ValidateTH(thuongHieu, istenTH);
            // Thêm các thông báo lỗi vào ModelState
            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }
            if (ModelState.IsValid)
            {
                db.Add(thuongHieu);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(thuongHieu);
        }

        // GET: Admin/ThuongHieuAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 9, "sua");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            if (id == null || db.ThuongHieus == null)
            {
                return NotFound();
            }

            var thuongHieu = await db.ThuongHieus.FindAsync(id);
            if (thuongHieu == null)
            {
                return NotFound();
            }
            return View(thuongHieu);
        }

        // POST: Admin/ThuongHieuAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTh,TenTh")] ThuongHieu thuongHieu)
        {
            // Kiểm tra tên đăng nhập trùng lặp
            var istenTH = await db.ThuongHieus.AnyAsync(t => t.TenTh == thuongHieu.TenTh && t.MaTh != thuongHieu.MaTh);
            var errorMessages = ValidateTH(thuongHieu, istenTH);
            // Thêm các thông báo lỗi vào ModelState
            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }
            if (id != thuongHieu.MaTh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(thuongHieu);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThuongHieuExists(thuongHieu.MaTh))
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
            return View(thuongHieu);
        }

        // GET: Admin/ThuongHieuAdmin/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 9, "xoa");
            if (!coQuyen)
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập vào trang này." });

            }
            // Kết thúc KTQ 
            // Kiểm tra xem sanpham có liên kết với bảng loai không
            var hh = await db.HangHoas.AnyAsync(pq => pq.MaTh == id);
            if (hh)
            {
                return Json(new { success = false, message = "Không thể xóa, ThuongHieu đang tồn tại trong bảng HangHoa." });
            }
            var thuongHieu = await db.ThuongHieus.FindAsync(id);
            if (thuongHieu != null)
            {
                db.ThuongHieus.Remove(thuongHieu);
            }

            await db.SaveChangesAsync();
            return Json(new { success = true });
        }
        private bool ThuongHieuExists(int id)
        {
          return (db.ThuongHieus?.Any(e => e.MaTh == id)).GetValueOrDefault();
        }
    }
}
