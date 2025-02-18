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
    public class ChiTietGgController : Controller
    {
        private readonly WebncContext db;
        private readonly KiemTraQuyen _kiemTraQuyen;
        public ChiTietGgController(WebncContext context, KiemTraQuyen kiemTraQuyen)
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
        // Create
        private async Task<bool> TrungLap( int maGG, int maHH, WebncContext db)
        {
            // Kiểm tra xem có trang có MaTrang như maTrang không
            var TTGG = await db.GiamGia.AnyAsync(t => t.MaGg == maGG);
            // Kiểm tra xem có nhân viên có MaNv như maNv không
            var TTHH = await db.HangHoas.AnyAsync(nv => nv.MaHh == maHH);
            // Nếu cả hai đều tồn tại, có sự trùng lặp
            return TTHH && TTGG;
        }
        // Edit 
        private async Task<bool> TrungLap1( int maGG, int maHH, ChiTietGg chiTietGg)
        {
            return await db.ChiTietGgs
                .AnyAsync(ct => ct.MaHh == maHH && ct.MaGg == maGG && ct.MaCtgg != chiTietGg.MaCtgg);
        }
        private bool KhongTrong(ChiTietGg chiTietGg)
        {
            return chiTietGg.MaGg != 0 && chiTietGg.MaHh != 0;
        }
        private async Task DanhSachHT()
        {
             var giamgia = await db.GiamGia.Select(nv => new
            {
                MaGg = nv.MaGg,
                DisplayText = nv.MaGg + " - " + nv.TenGg,
            }).ToListAsync();

            ViewData["MaGg"] = new SelectList(giamgia, "MaGg", "DisplayText");

            var hanghoa = await db.HangHoas.Select(t => new
            {
                MaHh = t.MaHh,
                DisplayText = t.MaHh + " - " + t.TenHh
            }).ToListAsync();

            ViewData["MaHh"] = new SelectList(hanghoa, "MaHh", "DisplayText");
        }
        // GET: Admin/ChiTietGg
        public async Task<IActionResult> Index()
        {
            var webncContext = db.ChiTietGgs.Include(c => c.MaGgNavigation).Include(c => c.MaHhNavigation);
            return View(await webncContext.ToListAsync());
        }

        // GET: Admin/ChiTietGg/Details/5
        

        public async Task<IActionResult> Create()
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 11, "them");
            if (!coQuyen)
            {
                return Json(new { error = "Bạn không có quyền truy cập vào trang này." });

            }
            await DanhSachHT();
            return View();
        }
        // POST: Admin/ChiTietGg/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaCtgg,MaHh,MaGg")] ChiTietGg chiTietGg)
        {
            // Kiểm tra xem MaNv và MaTrang không bị bỏ trống
            if (!KhongTrong(chiTietGg))
            {
                ModelState.AddModelError("", "Giảm giá và hàng hóa không được bỏ trống.");
                await DanhSachHT();
                return View(chiTietGg);
            }

            // Kiểm tra sự trùng lặp
            if (await TrungLap(chiTietGg.MaGg, chiTietGg.MaHh, db))
            {
                ModelState.AddModelError("", "Giảm giá và hàng hóa đã được thêm trước đó.");
                await DanhSachHT();
                return View(chiTietGg);
            }

            // ModelState is valid, continue with the rest of the logic
            db.Add(chiTietGg);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/ChiTietGg/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 11, "sua");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            if (id == null || db.ChiTietGgs == null)
            {
                return NotFound();
            }

            var chiTietGg = await db.ChiTietGgs.FindAsync(id);
            if (chiTietGg == null)
            {
                return NotFound();
            }
            await DanhSachHT();
            return View(chiTietGg);
        }

        // POST: Admin/ChiTietGg/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaCtgg,MaHh,MaGg")] ChiTietGg chiTietGg)
        {
            if (id != chiTietGg.MaCtgg)
            {
                return NotFound();
            }
            // Kiểm tra sự trùng lặp
            if (await TrungLap1(chiTietGg.MaGg, chiTietGg.MaHh, chiTietGg))
            {
                ModelState.AddModelError("", "Giảm giá và hàng hóa đã được thêm trước đó.");
                await DanhSachHT();
                return View(chiTietGg);
            }
           
                try
                {
                    db.Update(chiTietGg);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChiTietGgExists(chiTietGg.MaCtgg))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            await DanhSachHT();
            // Chuyển hướng về trang Index sau khi lưu thành công
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 11, "xoa");
            if (!coQuyen)
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập vào trang này." });
            }
            var chiTietGg = await db.ChiTietGgs.FindAsync(id);
            if (chiTietGg == null)
            {
                return Json(new { success = false, message = "Item not found." });
            }
            db.ChiTietGgs.Remove(chiTietGg);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Item deleted successfully." });
        }

        private bool ChiTietGgExists(int id)
        {
          return (db.ChiTietGgs?.Any(e => e.MaCtgg == id)).GetValueOrDefault();
        }
    }
}
