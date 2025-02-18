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
    public class PhanQuyensController : Controller
    {
        private readonly WebncContext db;
        private readonly KiemTraQuyen _kiemTraQuyen;

        public PhanQuyensController(WebncContext context, KiemTraQuyen kiemTraQuyen)
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
        private async Task<bool> TrungLap(int maNv, int maTrang, WebncContext db)
        {
            // Kiểm tra xem có nhân viên có MaNv như maNv không
            var TTNhanVien = await db.NhanViens.AnyAsync(nv => nv.MaNv == maNv);

            // Kiểm tra xem có trang có MaTrang như maTrang không
            var TTTrang = await db.Trangs.AnyAsync(t => t.MaTrang == maTrang);

            // Nếu cả hai đều tồn tại, có sự trùng lặp
            return TTNhanVien && TTTrang;
        }
        private async Task<bool> TrungLap1(int maNv, int maTrang, PhanQuyen phanQuyen)
        {
            return await db.PhanQuyens
                .AnyAsync(pq => pq.MaNv == maNv && pq.MaTrang == maTrang && pq.MaPq != phanQuyen.MaPq);
        }
        private bool KhongTrong(PhanQuyen phanQuyen)
        {
            return phanQuyen.MaNv != 0 && phanQuyen.MaTrang != 0;
        }
        private async Task DanhSachHT()
        {
            var nhanVien = await db.NhanViens.Select(nv => new
            {
                MaNv = nv.MaNv,
                DisplayText = nv.MaNv + " - " + nv.HoTen
            }).ToListAsync();

            ViewData["MaNv"] = new SelectList(nhanVien, "MaNv", "DisplayText");

            var trang = await db.Trangs.Select(t => new
            {
                MaTrang = t.MaTrang,
                DisplayText = t.MaTrang + " - " + t.TenTrang
            }).ToListAsync();

            ViewData["MaTrang"] = new SelectList(trang, "MaTrang", "DisplayText");
        }


        // GET: Admin/PhanQuyens
        public async Task<IActionResult> Index()
        {
            var webncContext = db.PhanQuyens.Include(p => p.MaNvNavigation).Include(p => p.MaTrangNavigation);
            return View(await webncContext.ToListAsync());
        }

        // GET: Admin/PhanQuyens/Create
        public async Task<IActionResult> Create()
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 2, "them");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            await DanhSachHT();
            return View();
        }
        // POST: Admin/PhanQuyens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("MaNv,MaTrang,Them,Sua,Xoa,Xem")] PhanQuyen phanQuyen)
        {
            // Kiểm tra xem MaNv và MaTrang không bị bỏ trống
            if (!KhongTrong(phanQuyen))
            {
                ModelState.AddModelError("", "Nhân viên và trang không được bỏ trống.");
                await DanhSachHT();
                return View(phanQuyen);
            }

            // Kiểm tra sự trùng lặp
            if (await TrungLap(phanQuyen.MaNv, phanQuyen.MaTrang, db))
            {
                ModelState.AddModelError("", "Nhân viên và trang đã được phân quyền trước đó.");
                await DanhSachHT();
                return View(phanQuyen);
            }

            // ModelState is valid, continue with the rest of the logic
            db.Add(phanQuyen);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Admin/PhanQuyens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 2, "sua");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            if (id == null || db.PhanQuyens == null)
            {
                return NotFound();
            }

            var phanQuyen = await db.PhanQuyens.FindAsync(id);
            if (phanQuyen == null)
            {
                return NotFound();
            }
            await DanhSachHT();
            return View(phanQuyen);
        }

        // POST: Admin/PhanQuyens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public async Task<IActionResult> Edit(int id, [Bind("MaPq,MaNv,MaTrang,Them,Sua,Xoa,Xem")] PhanQuyen phanQuyen)
        {
            if (id != phanQuyen.MaPq)
            {
                return NotFound();
            }

            // Kiểm tra sự trùng lặp
            if (await TrungLap1(phanQuyen.MaNv, phanQuyen.MaTrang, phanQuyen))
            {
                ModelState.AddModelError("", "Nhân viên và trang đã được phân quyền trước đó.");
                await DanhSachHT();
                return View(phanQuyen);
            }
            try
            {
                db.Update(phanQuyen);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhanQuyenExists(phanQuyen.MaPq))
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
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 2, "xoa");
            if (!coQuyen)
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập vào trang này." });
            }
            // Kết thúc KTQ 
            // Kiểm tra xem trang có tồn tại không
            var phanQuyen = await db.PhanQuyens.FindAsync(id);
            if (phanQuyen == null)
            {
                return Json(new { success = false, message = "Item not found." });
            }

            db.PhanQuyens.Remove(phanQuyen);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Item deleted successfully." });
        }

        private bool PhanQuyenExists(int id)
        {
          return (db.PhanQuyens?.Any(e => e.MaPq == id)).GetValueOrDefault();
        }
    }
}
