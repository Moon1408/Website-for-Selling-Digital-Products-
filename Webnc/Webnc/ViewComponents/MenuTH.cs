using Microsoft.AspNetCore.Mvc;
using Webnc.Models;
using Webnc.ViewModels;

namespace Webnc.ViewComponents
{
    public class MenuTH : ViewComponent
    {
        // Khai báo database 
        private readonly WebncContext db;
        public MenuTH(WebncContext context) => db = context;
        // Bắt buộc dùng Invoke
        public IViewComponentResult Invoke()
        {
            // Chuyển từ Actions qua Model 
            // MenuLoaiVM là lấy từ ViewModels 
            // Mỗi đối tượng Loais được chuyển đổi thành một đối tượng MenuLoaiVM (ViewModel của MenuLoai)
            var data = db.ThuongHieus.Select(th => new MenuVM
            {
                // Lấy tên loại, số lượng sp thuộc loại đó 
                MaTH = th.MaTh,
                TenTH = th.TenTh,
                SoLuongTH = th.HangHoas.Count
            }).OrderBy(p => p.TenTH);
            // Gửi qua dạng list 
            return View(data); // Default.cshtml
                               //return View("Default", data);
        }
    }
}
