using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using Webnc.Helpers;
using Webnc.Models;
using Webnc.ViewModels;

namespace Webnc.Controllers
{
    public class OrderController : Controller
    {
        private readonly WebncContext db;

        public OrderController(WebncContext context)
        {
            db = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst(MySetting.CLAIM_MAKH);
            if (userIdClaim == null)
            {
                return Unauthorized("User not logged in.");
            }

            var userId = int.Parse(userIdClaim.Value);

            var orders = db.HoaDons
                .Include(o => o.MaTrangThaiNavigation)
                .Where(o => o.MaKh == userId)
                .Select(o => new OrderListViewModel
                {
                    MaHd = o.MaHd,
                    NgayDat = o.NgayDat,
                    TongTien = o.TongTien,
                    TenTrangThai = o.MaTrangThaiNavigation.TenTrangThai,
                    CachThanhToan = o.CachThanhToan
                })
                .ToList();

            if (orders.Count == 0)
            {
                // Nếu không có đơn hàng, chuyển đến View khác hoặc hiển thị thông báo
                return View("EmptyOrders");
            }

            return View(orders);
        }

        public IActionResult Details(int id)
        {
            var order = db.HoaDons
                .Include(o => o.ChiTietHds)
                .ThenInclude(od => od.MaHhNavigation)
                .Include(o => o.MaTrangThaiNavigation)
                .FirstOrDefault(o => o.MaHd == id);

            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new OrderDetailsViewModel
            {
                MaHd = order.MaHd,
                HoTen = order.HoTen,
                DiaChi = order.DiaChi,
                DienThoai = order.DienThoai,
                NgayDat = order.NgayDat,
                TongTien = order.TongTien,
                TenTrangThai = order.MaTrangThaiNavigation.TenTrangThai,
                Items = order.ChiTietHds.Select(od => new OrderItemViewModel
                {
                    TenHh = od.MaHhNavigation.TenHh,
                    SoLuong = od.SoLuong,
                    DonGia = od.DonGia,
                    TienGG= od.TienGg,
                    TienCuoiCung = od.TienCuoiCung,
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public IActionResult CancelOrder(int id)
        {
            var order = db.HoaDons.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.MaTrangThai == 1)
            {
                ModelState.AddModelError(string.Empty, "Không thể hủy đơn hàng đã giao hàng.");
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng đã giao hàng.";
                return RedirectToAction("Index");
            }

            order.MaTrangThai = -1; // Đã hủy
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công.";
            return RedirectToAction("Index");
        }
    }
}
