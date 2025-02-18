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
    [Authorize(Policy = "QuanLyP")]
    public class TrangsController : Controller
    {
        private readonly WebncContext db;
        private readonly KiemTraQuyen _kiemTraQuyen;

        public TrangsController(WebncContext context, KiemTraQuyen kiemTraQuyen)
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
        // GET: Admin/Trangs
        public async Task<IActionResult> Index()
        {
              return db.Trangs != null ? 
                          View(await db.Trangs.ToListAsync()) :
                          Problem("Entity set 'WebncContext.Trangs'  is null.");
        }
       
        // HIỂN THỊ LỖI 
        private List<string> ValidateTrang(Trang trang, bool istentrang)
        {
            var errorMessages = new List<string>();

            if (istentrang)
            {
                errorMessages.Add("Tên trang đã tồn tại. Vui lòng chọn tên trang khác.");
            }

            if (string.IsNullOrWhiteSpace(trang.TenTrang))
            {
                errorMessages.Add("Bạn chưa điền tên tarng");
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
                bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 4, "xem");
                if (!coQuyen)
                {
                    return Json(new { error = "Bạn không có quyền truy cập vào trang này." });
                }
                // Kết thúc KTQ 
                var Trangs = await db.Trangs
                .FirstOrDefaultAsync(m => m.MaTrang == id);
                var trangCT = new
                {
                    trang = Trangs.TenTrang
                };
                return Json(trangCT);

            }
            catch (Exception ex)
            {
                // Trả về một lỗi trong trường hợp có lỗi xảy ra
                return Json(new { error = "Có lỗi xảy ra: " + ex.Message });
            }
        }
            // GET: Admin/Trangs/Create
            public async Task<IActionResult> Create()
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 4, "them");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            return View();
        }

        // POST: Admin/Trangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTrang,TenTrang")] Trang trang)
        {
           
                // Kiểm tra tên đăng nhập trùng lặp
                var istentrang = await db.Trangs.AnyAsync(t => t.TenTrang == trang.TenTrang && t.MaTrang != trang.MaTrang);            // Kiểm tra lỗi và lấy các thông báo lỗi
            var errorMessages = ValidateTrang(trang, istentrang);
            // Thêm các thông báo lỗi vào ModelState
            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }
            if (ModelState.IsValid)
            {
                db.Add(trang);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trang);
        }

        // GET: Admin/Trangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 4, "sua");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            if (id == null || db.Trangs == null)
            {
                return NotFound();
            }

            var trang = await db.Trangs.FindAsync(id);
            if (trang == null)
            {
                return NotFound();
            }
            return View(trang);
        }

        // POST: Admin/Trangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTrang,TenTrang")] Trang trang)
        {
          
            // Kiểm tra tên đăng nhập trùng lặp
            var istentrang = await db.Trangs.AnyAsync(t => t.TenTrang == trang.TenTrang && t.MaTrang != trang.MaTrang);            // Kiểm tra lỗi và lấy các thông báo lỗi
            var errorMessages = ValidateTrang(trang, istentrang);
            // Thêm các thông báo lỗi vào ModelState
            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }
            if (id != trang.MaTrang)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(trang);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrangExists(trang.MaTrang))
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
            return View(trang);
        }
        // XÓA
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 4, "xoa");
            if (!coQuyen)
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập vào trang này." });
            }
            // Kết thúc KTQ 
            // Kiểm tra xem trang có liên kết với bảng PhanQuyen không
            var phanQuyenTT = await db.PhanQuyens.AnyAsync(pq => pq.MaTrang == id);
            if (phanQuyenTT)
            {
                return Json(new { success = false, message = "Không thể xóa, Trang đang tồn tại trong bảng PhanQuyen." });
            }

            // Xóa trang và lưu thay đổi vào cơ sở dữ liệu
            var trang = await db.Trangs.FindAsync(id);
            if (trang != null)
            {
                db.Trangs.Remove(trang);
            }
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Item deleted successfully." });
            
        }

        private bool TrangExists(int id)
        {
          return (db.Trangs?.Any(e => e.MaTrang == id)).GetValueOrDefault();
        }
    }
}
