using Microsoft.AspNetCore.Mvc;
using Webnc.Data;
using Webnc.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Webnc.Models;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Webnc.Controllersz
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebncContext db;

        public HomeController(ILogger<HomeController> logger, WebncContext context)
        {
            _logger = logger;
            db = context;
        }
        //public IActionResult Index(int? loai, int? thuonghieu)
        //{
        //    var hangHoas = db.HangHoas.AsQueryable();

        //    if (loai.HasValue)
        //    {
        //        hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
        //    }

        //    if (thuonghieu.HasValue)
        //    {
        //        hangHoas = hangHoas.Where(p => p.MaTh == thuonghieu.Value);
        //    }

        //    var result = hangHoas.Select(p => new HangHoaVM
        //    {
        //        MaHh = p.MaHh,
        //        TenHH = p.TenHh,
        //        DonGia = p.DonGia ?? 0,
        //        Hinh = p.Hinh ?? "",
        //        MoTaNgan = p.MoTa ?? "",
        //        TenLoai = p.MaLoaiNavigation.TenLoai,
        //        TenTH = p.MaThNavigation.TenTh
        //    }).ToList();

        //    if (!result.Any())
        //    {
        //        ViewData["Message"] = "Không có sản phẩm phù hợp.";
        //    }

        //    return View(result);
        //}
        public IActionResult Index(int? loai, int? thuonghieu)
        {
            var hangHoas = db.HangHoas.AsQueryable();
            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }
            if (thuonghieu.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaTh == thuonghieu.Value);
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
            }).ToList();
            if (!result.Any())
            {
                ViewData["Message"] = "Không có sản phẩm phù hợp.";
            }

            // Load sản phẩm Laptop 
            var SPNB = db.HangHoas
                                    .Where(p => p.MaLoai == 1001)
                                    .Select(p => new HangHoaVM
                                    {
                                        MaHh = p.MaHh,
                                        TenHH = p.TenHh,
                                        DonGia = p.DonGia ?? 0,
                                        Hinh = p.Hinh ?? "",
                                        MoTaNgan = p.MoTa ?? "",
                                        TenLoai = p.MaLoaiNavigation.TenLoai,
                                        TenTH = p.MaThNavigation.TenTh
                                    })
                                    .ToList();
            ViewBag.SPNB = SPNB;
            return View(result);
        }




        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Shop()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
      

    }
}
