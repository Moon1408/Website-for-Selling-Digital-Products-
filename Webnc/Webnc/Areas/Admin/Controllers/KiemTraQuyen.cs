using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webnc.Helpers;
using Webnc.Models;

namespace Webnc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KiemTraQuyen : Controller
    {
        private readonly WebncContext db;
        public KiemTraQuyen(WebncContext dbContext)
        {
            db = dbContext;
        }
        public async Task<bool> KiemTraChucNang(int maNv, int matrang, string loaiQuyen)
        {
            var phanQuyen = await db.PhanQuyens
                .FirstOrDefaultAsync(pq => pq.MaNv == maNv && pq.MaTrang == matrang);

            if (phanQuyen == null)
                return false;

            switch (loaiQuyen.ToLower())
            {
                case "xem":
                    return phanQuyen.Xem;
                case "sua":
                    return phanQuyen.Sua;
                case "xoa":
                    return phanQuyen.Xoa;
                case "them":
                    return phanQuyen.Them;
                default:
                    return false;
            }
        }
        public IActionResult TuChoi()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DangXuat()
        {
            try
            {
                await HttpContext.SignOutAsync();
                return Ok(new { message = "Đăng xuất thành công" });
               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

