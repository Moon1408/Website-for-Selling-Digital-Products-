using Microsoft.AspNetCore.Mvc;
using Webnc.Models;
using Webnc.ViewModels;
namespace Webnc.ViewComponents
{
    public class MenuHome : ViewComponent
    {
        // Khai báo database 
        private readonly WebncContext db;
        public MenuHome(WebncContext context) => db = context;
        // Bắt buộc dùng Invoke
        public IViewComponentResult Invoke()
        {
            // Chuyển từ Actions qua Model 
            // MenuLoaiVM là lấy từ ViewModels 
            // Mỗi đối tượng Loais được chuyển đổi thành một đối tượng MenuLoaiVM (ViewModel của MenuLoai)
            var data = db.Loais.Select(lo => new MenuVM
            {
                // Lấy tên loại, số lượng sp thuộc loại đó 
                MaLoai = lo.MaLoai,
                TenLoai = lo.TenLoai,
            }).OrderBy(p => p.TenLoai);
            // Gửi qua dạng list 
            return View(data); // Default.cshtml
                               //return View("Default", data);
        }
    }
}


