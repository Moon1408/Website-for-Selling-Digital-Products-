using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Policy;
using Webnc.Helpers;
using Webnc.Models;
using Webnc.ViewModels;

namespace Webnc.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly WebncContext db;
        private readonly IMapper _mapper;

        public TaiKhoanController(WebncContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DangKy(RegisterVM model)
        {
            // Kiểm tra không được bỏ trống 
            if (ModelState.IsValid)
            {
                try
                {

                    // Kiểm tra xem tên đăng nhập, email hoặc số điện thoại đã tồn tại trong cơ sở dữ liệu hay không
                    if (db.KhachHangs.Any(kh => kh.TenDn == model.TenDN || kh.Email == model.Email || kh.DienThoai == model.DienThoai))
                    {
                        if (db.KhachHangs.Any(kh => kh.TenDn == model.TenDN))
                        {
                            ModelState.AddModelError(string.Empty, "Tên đăng nhập đã được sử dụng.");
                        }
                        if (db.KhachHangs.Any(kh => kh.Email == model.Email))
                        {
                            ModelState.AddModelError(string.Empty, "Email đã được sử dụng.");
                        }
                        if (db.KhachHangs.Any(kh => kh.DienThoai == model.DienThoai))
                        {
                            ModelState.AddModelError(string.Empty, "Số điện thoại đã được sử dụng.");
                        }
                    }
                    else
                    {
                        var khachHang = _mapper.Map<KhachHang>(model);
                        khachHang.TenDn = model.TenDN;
                        khachHang.MatKhau = model.MatKhau;
                        khachHang.HieuLuc = true;
                       

                        db.Add(khachHang);
                        db.SaveChanges();
                        //// Hiển thị thông báo thành công và chuyển hướng đến trang Index
                        //ViewBag.SuccessMessage = "Đăng ký thành công!";
                        return RedirectToAction("DangNhap", "TaiKhoan");

                    }
                }
                catch (Exception ex)
                {
                    var mess = $"{ex.Message} shh";
                    // Xử lý lỗi nếu cần
                }

            }
            return View(model);
        }
        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Bạn chưa điền thông tin đăng nhập." });
            }
            var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.TenDn == model.UserName);
            if (khachHang != null)
            {
                if (khachHang == null || !khachHang.HieuLuc)
                {
                    return new JsonResult(new { success = false, message = "Tài khoản đã bị khóa. Vui lòng liên hệ Admin" });
                }

                if (khachHang.MatKhau != model.Password)
                {
                    return new JsonResult(new { success = false, message = "Sai mật khẩu." });
                }

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, khachHang.Email),
            new Claim(ClaimTypes.Name, khachHang.HoTen),
            new Claim(MySetting.CLAIM_MAKH, khachHang.MaKh.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(claimsPrincipal);
                return new JsonResult(new { success = true, message = "Đăng nhập thành công." });
            }
            var nhanVien = await db.NhanViens
                .Include(nv => nv.MaPbNavigation) 
                .FirstOrDefaultAsync(nv => nv.TenDn == model.UserName);
            if (nhanVien != null)
            {
                if (nhanVien == null || !nhanVien.HieuLuc)
                {
                    return new JsonResult(new { success = false, message = "Tài khoản đã bị khóa. Vui lòng liên hệ Admin" });
                }
                if (nhanVien.MatKhau != model.Password)
                {
                    return new JsonResult(new { success = false, message = "Sai mật khẩu." });
                }
                //biến role sẽ có giá trị "Quản lý" nếu phòng ban (TenPb) của nhân viên là "Quản lý", và "Nhân viên" trong các trường hợp khác.
                string role = nhanVien.MaPbNavigation?.TenPb == "Quản lý" ? "Quản lý" : "Nhân viên";

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, nhanVien.Email),
                new Claim(ClaimTypes.Name, nhanVien.HoTen),
                //new Claim(ClaimTypes.Role, "Employee"),
                // ClaimTypes.Role: Đây là một loại claim định nghĩa vai trò của người dùng.
                 new Claim(ClaimTypes.Role, role),
                new Claim(MySetting.CLAIM_MANV, nhanVien.MaNv.ToString())
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(claimsPrincipal);
                    return new JsonResult(new { success = true, message = "Đăng nhập thành công" });
                }

                return new JsonResult(new { success = false, message = "Không có tài khoản này." });
        }

      
        [HttpPost]

        public async Task<IActionResult> DangXuat()
        {
            try
            {
                await HttpContext.SignOutAsync();
                return new OkObjectResult(new { message = "Đăng xuất thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       
    }
}















