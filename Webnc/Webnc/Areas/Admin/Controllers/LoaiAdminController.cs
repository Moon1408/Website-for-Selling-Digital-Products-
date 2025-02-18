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
    public class LoaiAdminController : Controller
    {
        private readonly WebncContext db;
        private readonly KiemTraQuyen _kiemTraQuyen;

        public LoaiAdminController(WebncContext context, KiemTraQuyen kiemTraQuyen)
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
        private List<string> ValidateL(Loai loai, bool istenl)
        {
            var errorMessages = new List<string>();

            if (istenl)
            {
                errorMessages.Add("Tên loại đã tồn tại. Vui lòng chọn tên loại khác.");
            }

            if (string.IsNullOrWhiteSpace(loai.TenLoai))
            {
                errorMessages.Add("Bạn chưa điền tên loại");
            }
            return errorMessages;
        }
        // GET: Admin/LoaiAdmin
        public async Task<IActionResult> Index()
        {
            return db.Loais != null ?
                        View(await db.Loais.ToListAsync()) :
                        Problem("Entity set 'WebncContext.Loais'  is null.");
        }

        // GET: Admin/LoaiAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                // Kiểm tra quyền trước khi thực hiện 
                int manv = NVHienTai();
                bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 8, "xem");
                if (!coQuyen)
                {
                    return Json(new { error = "Bạn không có quyền truy cập vào trang này." });

                }
                // Kết thúc KTQ 
                var loaiT = await db.Loais.FirstOrDefaultAsync(m => m.MaLoai == id);
                var loaiCT = new
                {
                    loai = loaiT.TenLoai
                };
                return Json(loaiCT);
            }
            catch (Exception ex)
            {
                // Trả về một lỗi trong trường hợp có lỗi xảy ra
                return Json(new { error = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: Admin/LoaiAdmin/Create
        public async Task<IActionResult> Create()
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 8, "them");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            return View();
        }

        // POST: Admin/LoaiAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLoai,TenLoai")] Loai loai)
        {
            // Kiểm tra tên đăng nhập trùng lặp
            var istenpb = await db.Loais.AnyAsync(t => t.TenLoai == loai.TenLoai && t.MaLoai != loai.MaLoai);          
            var errorMessages = ValidateL(loai, istenpb);
            // Thêm các thông báo lỗi vào ModelState
            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }
            if (ModelState.IsValid)
            {
                db.Add(loai);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loai);
        }

        // GET: Admin/LoaiAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 8, "sua");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            if (id == null || db.Loais == null)
            {
                return NotFound();
            }

            var loai = await db.Loais.FindAsync(id);
            if (loai == null)
            {
                return NotFound();
            }
            return View(loai);
        }

        // POST: Admin/LoaiAdmin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLoai,TenLoai")] Loai loai)
        {
            // Kiểm tra tên đăng nhập trùng lặp
            var istenpb = await db.Loais.AnyAsync(t => t.TenLoai == loai.TenLoai && t.MaLoai != loai.MaLoai);
            var errorMessages = ValidateL(loai, istenpb);
            // Thêm các thông báo lỗi vào ModelState
            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }
            if (id != loai.MaLoai)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(loai);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiExists(loai.MaLoai))
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
            return View(loai);
        }
        // GET: Admin/PhongBans/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 8, "xoa");
            if (!coQuyen)
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập vào trang này." });

            }
            // Kết thúc KTQ 
            // Kiểm tra xem sanpham có liên kết với bảng loai không
            var hh = await db.HangHoas.AnyAsync(pq => pq.MaLoai == id);
            if (hh)
            {
                return Json(new { success = false, message = "Không thể xóa, Loai đang tồn tại trong bảng HangHoa." });
            }
            // Xóa trang và lưu thay đổi vào cơ sở dữ liệu
            var loai = await db.Loais.FindAsync(id);
            if (loai != null)
            {
                db.Loais.Remove(loai);
            }
            await db.SaveChangesAsync();
            return Json(new { success = true });

        }
        private bool LoaiExists(int id)
        {
            return (db.Loais?.Any(e => e.MaLoai == id)).GetValueOrDefault();
        }
    }
}
