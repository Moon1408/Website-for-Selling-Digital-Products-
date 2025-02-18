using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Webnc.Helpers;
using Webnc.Models;

namespace Webnc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "QuanLyP")]
    public class NhanViensController : Controller
    {
        
            private readonly WebncContext db;
            private readonly KiemTraQuyen _kiemTraQuyen;
            public NhanViensController(WebncContext context, KiemTraQuyen kiemTraQuyen)
            {
                db = context;
                _kiemTraQuyen = kiemTraQuyen;
            }
            public IActionResult ExportExcel()
            {
                // Lấy danh sách nhân viên từ database
                //var nhanViens = db.NhanViens.ToList();
                var nhanViens = db.NhanViens.Include(nv => nv.MaPbNavigation).ToList();
                // Tạo package Excel
                using (var package = new ExcelPackage())
                {
                    // Tạo một sheet mới
                    var worksheet = package.Workbook.Worksheets.Add("NhanVien");

                    // Đặt tiêu đề cho các cột trong sheet Excel
                    worksheet.Cells["A1"].Value = "Mã NV";
                    worksheet.Cells["B1"].Value = "Tên đăng nhập";
                    worksheet.Cells["C1"].Value = "Họ tên";
                    worksheet.Cells["D1"].Value = "Email";
                    worksheet.Cells["F1"].Value = "Mã PB";
                    worksheet.Cells["E1"].Value = "Hiệu lực";

                    // Đặt màu nền xám cho các ô trong dòng header
                    using (var range = worksheet.Cells["A1:F1"])
                    {
                        var fill = range.Style.Fill;
                        fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d0ecfc"));

                        var font = range.Style.Font;
                        font.Bold = false; // Đặt font chữ in đậm
                        font.Color.SetColor(System.Drawing.Color.Black); // Màu chữ là màu đen

                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Căn giữa dòng
                    }

                    // Đổ dữ liệu từ danh sách nhân viên vào sheet Excel
                    int row = 2;
                    foreach (var nhanVien in nhanViens)
                    {
                        worksheet.Cells[string.Format("A{0}", row)].Value = nhanVien.MaNv;
                        worksheet.Cells[string.Format("B{0}", row)].Value = nhanVien.TenDn;
                        worksheet.Cells[string.Format("C{0}", row)].Value = nhanVien.HoTen;
                        worksheet.Cells[string.Format("D{0}", row)].Value = nhanVien.Email;
                        worksheet.Cells[string.Format("E{0}", row)].Value = nhanVien.HieuLuc ? "Có" : "Không";
                        worksheet.Cells[string.Format("F{0}", row)].Value = $"{nhanVien.MaPb} - {nhanVien.MaPbNavigation?.TenPb}";
                        row++;
                    }
                    // Tự động điều chỉnh độ rộng của các cột để vừa khớp với nội dung
                    worksheet.Cells.AutoFitColumns();

                    // Lưu file Excel vào MemoryStream
                    MemoryStream stream = new MemoryStream();
                    package.SaveAs(stream);

                    // Thiết lập stream để có thể đọc được
                    stream.Position = 0;

                    // Trả về file Excel dưới dạng file tải về
                    string excelName = $"NhanVien_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
            public IActionResult ImportExcel(IFormFile file)
            {
                // Kiểm tra xem file có tồn tại hay không
                if (file == null || file.Length <= 0)
                {
                    ModelState.AddModelError("File", "Vui lòng chọn một file");
                }
                else if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("File", "Định dạng file không hợp lệ. Vui lòng chọn file Excel (.xlsx)");
                }
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    return Json(new { success = false, errors });
                }

                // Đọc file Excel từ stream
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        // Lấy sheet đầu tiên từ file Excel
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        // Đọc dữ liệu từ Excel và lưu vào danh sách nhân viên
                        var nhanViens = new List<NhanVien>();
                        int rowCount = worksheet.Dimension.Rows;
                        var existingNhanViens = db.NhanViens.ToList(); // Lấy danh sách nhân viên hiện tại trong DB

                        // Bắt đầu từ hàng 2 vì hàng 1 là header
                        for (int row = 2; row <= rowCount; row++)
                        {
                            // Đọc từng cột tương ứng
                            var nhanVien = new NhanVien
                            {
                                TenDn = worksheet.Cells[row, 2].Text.Trim(),
                                MatKhau = worksheet.Cells[row, 3].Text.Trim(),
                                HoTen = worksheet.Cells[row, 4].Text.Trim(),
                                Email = worksheet.Cells[row, 5].Text.Trim(),
                                HieuLuc = (bool)(worksheet.Cells[row, 6].Text.Trim().Equals("Có", StringComparison.OrdinalIgnoreCase)),
                            };
                            if (existingNhanViens.Any(nv => nv.TenDn == nhanVien.TenDn))
                            {
                                ModelState.AddModelError(string.Empty, $"Dòng {row}: Tên đăng nhập đã tồn tại.");
                            }
                            if (string.IsNullOrEmpty(nhanVien.TenDn) ||
                                string.IsNullOrEmpty(nhanVien.MatKhau) ||
                                string.IsNullOrEmpty(nhanVien.HoTen))
                            {
                                ModelState.AddModelError(string.Empty, "Không được để trống dữ liệu TenDn, MatKhau, HoTen");
                            }
                        //string maPbTenPb = worksheet.Cells[row, 7].Text.Trim();
                        //if (!string.IsNullOrEmpty(maPbTenPb))
                        //{
                        //    // Tách MaPb và TenPb
                        //    string[] parts = maPbTenPb.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        //    if (parts.Length >= 1)
                        //    {
                        //        nhanVien.MaPb = Convert.ToInt32(parts[0].Trim()); // Lưu MaPb (đã chuyển đổi sang int)
                        //    }

                        //}
                        string maPbTenPb = worksheet.Cells[row, 7].Text.Trim();
                        if (!string.IsNullOrEmpty(maPbTenPb))
                        {
                            // Tách MaPb và TenPb
                            string[] parts = maPbTenPb.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 1)
                            {
                                int maPb;
                                if (int.TryParse(parts[0].Trim(), out maPb))
                                {
                                    var phongBan = db.PhongBans.Find(maPb);
                                    // Kiểm tra xem MaPb có tồn tại trong bảng PhongBans hay không
                                    // var phongBan = db.PhongBans.SingleOrDefault(pb => pb.MaPb == maPb);

                                    if (phongBan != null)
                                    {
                                        nhanVien.MaPb = maPb; // Lưu MaPb (đã chuyển đổi sang int)
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, "Phòng ban không tồn tại ");                                   
                                    }
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Phòng ban không được để trống  ");

                        }
                        nhanViens.Add(nhanVien);
                        }

                        // Lưu danh sách nhân viên vào cơ sở dữ liệu
                        // Ví dụ lưu vào DbContext (db) của bạn
                        foreach (var nv in nhanViens)
                        {
                            // Xử lý thêm vào db
                            db.NhanViens.Add(nv);
                        }
                        if (!ModelState.IsValid)
                        {
                            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                            return Json(new { success = false, errors });
                        }
                        db.SaveChanges();

                        // Trả về trang Nhanvien 
                        return Json(new { success = true, message = "Import Excel thành công!" });
                    }
                }
            }

            // GET: Admin/NhanViens
            public async Task<IActionResult> Index()
        {
            var webncContext = db.NhanViens.Include(n => n.MaPbNavigation);
            return View(await webncContext.ToListAsync());
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
        // HIỂN THỊ LỖI 
        private List<string> ValidateNhanVien(NhanVien nhanVien, bool istendn)
        {
            var errorMessages = new List<string>();

            if (istendn)
            {
                errorMessages.Add("Tên đăng nhập đã tồn tại. Vui lòng chọn tên đăng nhập khác.");
            }

            if (string.IsNullOrWhiteSpace(nhanVien.TenDn))
            {
                errorMessages.Add("Bạn chưa điền tên đăng nhập.");
            }

            if (string.IsNullOrWhiteSpace(nhanVien.MatKhau))
            {
                errorMessages.Add("Bạn chưa điền mật khẩu.");
            }

            if (string.IsNullOrWhiteSpace(nhanVien.HoTen))
            {
                errorMessages.Add("Bạn chưa điền họ tên.");
            }


            return errorMessages;
        }
        private bool KhongTrong(NhanVien nhanVien)
        {
            return nhanVien.MaPb != 0 ;
        }
      
        // GET: Admin/NhanViens/Create
        public async Task<IActionResult> Create()
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 1, "them");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            var phongBans = db.PhongBans.Select(pb => new
            {
                MaPb = pb.MaPb,
                TenPb = pb.TenPb,
                DisplayText = pb.MaPb + " - " + pb.TenPb
            }).ToList();

            ViewData["MaPb"] = new SelectList(phongBans, "MaPb", "DisplayText");
            return View();
        }

        // POST: Admin/NhanViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNv,TenDn,MatKhau,HoTen,Email,MaPb")] NhanVien nhanVien)
        {
            var phongBans = db.PhongBans.Select(pb => new
            {
                MaPb = pb.MaPb,
                TenPb = pb.TenPb,
                DisplayText = pb.MaPb + " - " + pb.TenPb
            }).ToList();

            ViewData["MaPb"] = new SelectList(phongBans, "MaPb", "DisplayText");
            // Kiểm tra tên đăng nhập trùng lặp
            var istendn = await db.NhanViens.AnyAsync(nv => nv.TenDn == nhanVien.TenDn && nv.MaNv != nhanVien.MaNv);
        
                // Kiểm tra lỗi và lấy các thông báo lỗi
                var errorMessages = ValidateNhanVien(nhanVien, istendn);

            // Thêm các thông báo lỗi vào ModelState
            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }

            // Nếu có lỗi, trả về view với model và hiển thị các thông báo lỗi
            if (errorMessages.Any())
            {
                return View(nhanVien);
            }
            if (!KhongTrong(nhanVien))
            {

                ModelState.AddModelError("", "Phòng ban không được bỏ trống.");
                return View(nhanVien);
            }
            // ModelState is valid, continue with the rest of the logic
            db.Add(nhanVien);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Admin/NhanViens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 1, "sua");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            if (id == null || db.NhanViens == null)
            {
                return NotFound();
            }

            var nhanVien = await db.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return NotFound();
            }
            //ViewData["MaPb"] = new SelectList(db.PhongBans, "MaPb", "MaPb", nhanVien.MaPb);
            var phongBans = db.PhongBans.Select(pb => new
            {
                MaPb = pb.MaPb,
                TenPb = pb.TenPb,
                DisplayText = pb.MaPb + " - " + pb.TenPb
            }).ToList();

            ViewData["MaPb"] = new SelectList(phongBans, "MaPb", "DisplayText");

            return View(nhanVien);
        }

        // POST: Admin/NhanViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNv,TenDn,MatKhau,HoTen,Email,MaPb")] NhanVien nhanVien)
        {
            if (id != nhanVien.MaNv)
            {
                return NotFound();
            }
            var phongBans = db.PhongBans.Select(pb => new
            {
                MaPb = pb.MaPb,
                TenPb = pb.TenPb,
                DisplayText = pb.MaPb + " - " + pb.TenPb
            }).ToList();

            ViewData["MaPb"] = new SelectList(phongBans, "MaPb", "DisplayText");
            // Kiểm tra tên đăng nhập trùng lặp
            var istendn = await db.NhanViens.AnyAsync(nv => nv.TenDn == nhanVien.TenDn && nv.MaNv != nhanVien.MaNv);
            // Kiểm tra lỗi và lấy các thông báo lỗi
            var errorMessages = ValidateNhanVien(nhanVien, istendn);
            // Thêm các thông báo lỗi vào ModelState
            foreach (var errorMessage in errorMessages)
            {
                ModelState.AddModelError("", errorMessage);
            }
            // Nếu có lỗi, trả về view với model và hiển thị các thông báo lỗi
            if (errorMessages.Any())
            {
                return View(nhanVien);
            }
            try
            {
                db.Update(nhanVien);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhanVienExists(nhanVien.MaNv))
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
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {

                // Kiểm tra quyền trước khi thực hiện 
                int manv = NVHienTai();
                bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 1, "xem");
                if (!coQuyen)
                {
                    return Json(new { error = "Bạn không có quyền truy cập vào trang này." });
                }
                // Kết thúc KTQ 
                // Lấy chi tiết của nhân viên
                var nhanVien = await db.NhanViens
                    .Include(n => n.MaPbNavigation)
                    .FirstOrDefaultAsync(m => m.MaNv == id);

                if (nhanVien == null)
                {
                    // Trả về một lỗi nếu không tìm thấy nhân viên với id cung cấp
                    return Json(new { error = "Không tìm thấy nhân viên." });
                }

                var nhanVienDetails = new
                {
                    tenDn = nhanVien.TenDn,
                    matKhau = nhanVien.MatKhau,
                    hoTen = nhanVien.HoTen,
                    email = nhanVien.Email,
                    maPb = nhanVien.MaPbNavigation.MaPb,
                    tenPb = nhanVien.MaPbNavigation.TenPb
                };
                return Json(nhanVienDetails);
            }
            catch (Exception ex)
            {
                // Trả về một lỗi trong trường hợp có lỗi xảy ra
                return Json(new { error = "Có lỗi xảy ra: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 1, "xoa");
            if (!coQuyen)
            {
                return Json(new { success = false, message = "Bạn không có quyền truy cập vào trang này." });
            }
            // Kết thúc KTQ 
            // Kiểm tra xem trang có tồn tại không
            var nhanVien = await db.NhanViens.FindAsync(id);
            if (nhanVien == null)
            {
                return Json(new { success = false, message = "Item not found." });
            }

            // Kiểm tra xem trang có liên kết với bảng PhanQuyen không
            var phanQuyenTT = await db.PhanQuyens.AnyAsync(pq => pq.MaNv == id);
            if (phanQuyenTT)
            {
                return Json(new { success = false, message = "Không thể xóa, NhanVien đang tồn tại trong bảng PhanQuyen." });
            }

            // Xóa trang và lưu thay đổi vào cơ sở dữ liệu
            db.NhanViens.Remove(nhanVien);
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Item deleted successfully." });
    }
      
        private bool NhanVienExists(int id)
        {
            return (db.NhanViens?.Any(e => e.MaNv == id)).GetValueOrDefault();
        }
    }
}
