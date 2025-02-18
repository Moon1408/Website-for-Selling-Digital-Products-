using Microsoft.AspNetCore.Mvc;
using Webnc.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webnc.Models;
using X.PagedList;
using System.Runtime.InteropServices;

namespace Webnc.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly WebncContext db;

        public HangHoaController(WebncContext conetxt)
        {
            db = conetxt;
        }
        // Là trang shop 
        public IActionResult Index(int? page, int? loai, int? thuonghieu, string? query)
        {
            int pageSize = 6;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;
            var hangHoas = db.HangHoas.AsQueryable();

            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }
            if (thuonghieu.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaTh == thuonghieu.Value);
            }
            if (!string.IsNullOrEmpty(query))
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTa ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai,
                TenTH = p.MaThNavigation.TenTh
            });

            var pagedList = result.ToPagedList(pageNumber, pageSize);

            if (!pagedList.Any())
            {
                ViewData["Message"] = "Không có sản phẩm phù hợp.";
            }

            return View(pagedList);
        }
        public IActionResult Detail(int id)
        {
            var product = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .Include(p => p.MaThNavigation)
                .SingleOrDefault(p => p.MaHh == id);

            if (product == null)
            {
                TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
                return Redirect("/404");
            }

            var viewModel = new ChiTietHangHoaVM
            {
                MaHh = product.MaHh,
                TenHH = product.TenHh,
                DonGia = product.DonGia ?? 0,
                Hinh = product.Hinh ?? string.Empty,
                MoTaNgan = product.MoTa ?? string.Empty,
                TenLoai = product.MaLoaiNavigation?.TenLoai ?? string.Empty,
                TenTH = product.MaThNavigation?.TenTh ?? string.Empty,
                SoLuongTon = product.Sl
                // Add other properties as needed
            };
            var relatedProducts = db.HangHoas
                // Lấy sản phẩm cùng chủng loại nhưng khác id
                .Where(p => p.MaLoai == product.MaLoai && p.MaHh != id)
                // Nhận tối đa 4 sản phẩm liên quan
                .Take(6) 
                .Select(p => new HangHoaVM
                {
                    MaHh = p.MaHh,
                    TenHH = p.TenHh,
                    DonGia = p.DonGia ?? 0,
                    Hinh = p.Hinh ?? string.Empty
                })
                .ToList();
            viewModel.SPXT = relatedProducts;
            return View(viewModel);
        }
    public IActionResult Search(string? query)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
            });
            return RedirectToAction("Index", new { query = query });
        }

    }
}
