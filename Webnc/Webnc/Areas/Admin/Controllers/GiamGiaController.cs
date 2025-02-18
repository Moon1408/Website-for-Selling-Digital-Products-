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
using Webnc.ViewModels;

namespace Webnc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "QuanLyP")]
    public class GiamGiaController : Controller
    {
        private readonly WebncContext db;
        private readonly KiemTraQuyen _kiemTraQuyen;
        public GiamGiaController(WebncContext context, KiemTraQuyen kiemTraQuyen)
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
        private List<string> ValidateGG(GiamGium giamGium, bool code)
        {
            var errorMessages = new List<string>();

            if (code)
            {
                errorMessages.Add("Mã code đã tồn tại. Vui lòng chọn tên đăng nhập khác.");
            }

            if (string.IsNullOrWhiteSpace(giamGium.TenGg))
            {
                errorMessages.Add("Bạn chưa điền tên giảm giá.");
            }

            if (giamGium.NgayBd == null)
            {
                errorMessages.Add("Ngày bắt đầu không được bỏ trống.");
            }
            else if (giamGium.NgayBd < new DateTime(1753, 1, 1) || giamGium.NgayBd > new DateTime(9999, 12, 31))
            {
                errorMessages.Add("Ngày bắt đầu không hợp lệ.");
            }

            if (giamGium.NgayKt == null)
            {
                errorMessages.Add("Ngày kết thúc không được bỏ trống.");
            }
            // Kiểm tra nếu Ngày kết thúc lớn hơn Ngày bắt đầu
            if (giamGium.NgayBd != null && giamGium.NgayKt != null && giamGium.NgayKt <= giamGium.NgayBd)
            {
                errorMessages.Add("Ngày kết thúc phải lớn hơn ngày bắt đầu.");
            }
            else if (giamGium.NgayKt < new DateTime(1753, 1, 1) || giamGium.NgayKt > new DateTime(9999, 12, 31))
            {
                errorMessages.Add("Ngày kết thúc không hợp lệ.");
            }
            if (giamGium.DonGia == null || giamGium.DonGia < 0)  
            {
                errorMessages.Add("Bạn chưa điền đơn giá hoặc đơn giá  phải >0");
            }

            if (giamGium.GiaAd == null || giamGium.GiaAd < 0)  
            {
                errorMessages.Add("Bạn chưa điền giá áp dụng hoặc giá áp dụng phải > 0");
            }
            if (string.IsNullOrWhiteSpace(giamGium.Code))
            {
                errorMessages.Add("Bạn chưa điền code");
            }
            if (giamGium.Sl == null || giamGium.Sl < 0)
            {
                errorMessages.Add("Bạn chưa điền số lượng hoặc số lượng phải >0 ");
            }
            return errorMessages;
        }
        // GET: Admin/GiamGia
        public async Task<IActionResult> Index()
        {
              return db.GiamGia != null ? 
                          View(await db.GiamGia.ToListAsync()) :
                          Problem("Entity set 'WebncContext.GiamGia'  is null.");
        }

        // GET: Admin/GiamGia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 10, "xem");
            if (!coQuyen)
            {
                return Json(new { error = "Bạn không có quyền truy cập vào trang này." });

            }
            if (id == null || db.GiamGia == null)
            {
                return NotFound();
            }

            var giamGium = await db.GiamGia
                .FirstOrDefaultAsync(m => m.MaGg == id);
            if (giamGium == null)
            {
                return NotFound();
            }

            return View(giamGium);
        }

        // GET: Admin/GiamGia/Create
        public async Task<IActionResult> Create()
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 10, "them");
            if (!coQuyen)
            {
                return Json(new { error = "Bạn không có quyền truy cập vào trang này." });

            }
            return View();
        }
        // POST: Admin/GiamGia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGg,TenGg,NgayBd,NgayKt,DonGia,GiaAd,Code,Sl")] GiamGium giamGium)
        {
            // Kiểm tra tên đăng nhập trùng lặp
            var code = await db.GiamGia.AnyAsync(nv => nv.Code == giamGium.Code && nv.MaGg != giamGium.MaGg);

            // Kiểm tra lỗi và lấy các thông báo lỗi
            var errorMessages = ValidateGG(giamGium, code);

            // Thêm các thông báo lỗi vào ModelState
            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }

            // Nếu có lỗi, trả về view với model và hiển thị các thông báo lỗi
            if (errorMessages.Any())
            {
                return View(giamGium);
            }
                db.Add(giamGium);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }

        // GET: Admin/GiamGia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 10, "sua");
            if (!coQuyen)
            {
                return Json(new { error = "Bạn không có quyền truy cập vào trang này." });

            }
            if (id == null || db.GiamGia == null)
            {
                return NotFound();
            }

            var giamGium = await db.GiamGia.FindAsync(id);
            if (giamGium == null)
            {
                return NotFound();
            }
            return View(giamGium);
        }

        // POST: Admin/GiamGia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaGg,TenGg,NgayBd,NgayKt,DonGia,GiaAd,Code,Sl")] GiamGium giamGium)
        {

            // Kiểm tra tên đăng nhập trùng lặp
            var code = await db.GiamGia.AnyAsync(nv => nv.Code == giamGium.Code && nv.MaGg != giamGium.MaGg);

            // Kiểm tra lỗi và lấy các thông báo lỗi
            var errorMessages = ValidateGG(giamGium, code);

            // Thêm các thông báo lỗi vào ModelState
            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }

            // Nếu có lỗi, trả về view với model và hiển thị các thông báo lỗi
            if (errorMessages.Any())
            {
                return View(giamGium);
            }

            if (id != giamGium.MaGg)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(giamGium);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiamGiumExists(giamGium.MaGg))
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
            return View(giamGium);
        }

        // GET: Admin/GiamGia/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv,10, "xoa");
            if (!coQuyen)
            {
                return Json(new { error = "Bạn không có quyền truy cập vào trang này." });

            }
            var giamGium = await db.GiamGia
                .FirstOrDefaultAsync(m => m.MaGg == id);
            if (giamGium == null)
            {
                return Json(new { success = false, message = "Item not found." });
            }
            // Kiểm tra xem trang có liên kết với bảng PhanQuyen không
            var CTGG = await db.GiamGia.AnyAsync(pq => pq.MaGg == id);
            if (CTGG)
            {
                return Json(new { success = false, message = "Không thể xóa, Giảm giá đang tồn tại trong bảng Chi tiết giảm giá." });
            }
            // Xóa trang và lưu thay đổi vào cơ sở dữ liệu
            db.GiamGia.Remove(giamGium);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Item deleted successfully." });
        }
        private bool GiamGiumExists(int id)
        {
          return (db.GiamGia?.Any(e => e.MaGg == id)).GetValueOrDefault();
        }
    }
}
