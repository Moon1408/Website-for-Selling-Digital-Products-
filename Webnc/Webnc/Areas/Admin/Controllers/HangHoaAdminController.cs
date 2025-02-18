using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Webnc.Helpers;
using Webnc.Models;
using Webnc.ViewModels;

namespace Webnc.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "CombinedPolicy")]
    public class HangHoaAdminController : Controller
    {
        private readonly WebncContext db;
        private readonly KiemTraQuyen _kiemTraQuyen;

        public HangHoaAdminController(WebncContext context, KiemTraQuyen kiemTraQuyen)
        {
            db = context;
            _kiemTraQuyen = kiemTraQuyen;
        }
        [HttpGet]
        public IActionResult ExportExcel()
        {
            var hangHoas = db.HangHoas.Include(h => h.MaLoaiNavigation)
                .Include(h => h.MaThNavigation)
                .ToList();
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("HangHoas");

                worksheet.Cells["A1"].Value = "Mã Hh";
                worksheet.Cells["B1"].Value = "Tên Hh";
                worksheet.Cells["C1"].Value = "Đơn Giá";
                worksheet.Cells["D1"].Value = "Hình";
                worksheet.Cells["E1"].Value = "Mô Tả";
                worksheet.Cells["F1"].Value = "Số lượng";
                worksheet.Cells["G1"].Value = "Mã Th";
                worksheet.Cells["H1"].Value = "Mã Loai";

                // Đặt màu nền xám cho các ô trong dòng header
                using (var range = worksheet.Cells["A1:H1"])
                {
                    var fill = range.Style.Fill;
                    fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#d0ecfc"));

                    var font = range.Style.Font;
                    font.Bold = false; // Đặt font chữ in đậm
                    font.Color.SetColor(System.Drawing.Color.Black); // Màu chữ là màu đen

                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center; // Căn giữa dòng
                }
                var row = 2;
                foreach (var hangHoa in hangHoas)
                {
                    worksheet.Cells["A" + row].Value = hangHoa.MaHh;
                    worksheet.Cells["B" + row].Value = hangHoa.TenHh;
                    worksheet.Cells["C" + row].Value = hangHoa.DonGia;
                    worksheet.Cells["D" + row].Value = hangHoa.Hinh;
                    worksheet.Cells["E" + row].Value = hangHoa.MoTa;
                    worksheet.Cells["F" + row].Value = hangHoa.Sl;
                    worksheet.Cells["G" + row].Value = $"{hangHoa.MaLoai} - {hangHoa.MaLoaiNavigation.TenLoai}";
                    worksheet.Cells["H" + row].Value = $"{hangHoa.MaTh} - {hangHoa.MaThNavigation.TenTh}";

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
                string excelName = $"HangHoa_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
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
                    var Hanghoas = new List<HangHoa>();
                    int rowCount = worksheet.Dimension.Rows;
                    var existingHHs = db.HangHoas.ToList();

                    // Bắt đầu từ hàng 2 vì hàng 1 là header
                    for (int row = 2; row <= rowCount; row++)
                    {
                        // Đọc từng cột tương ứng
                        var hanghoas = new HangHoa
                        {
                            TenHh = worksheet.Cells[row, 2].Text.Trim(),
                            DonGia = Convert.ToDouble(worksheet.Cells[row, 3].Text.Trim()),
                            Hinh = worksheet.Cells[row, 4].Text.Trim(),
                            MoTa = worksheet.Cells[row, 5].Text.Trim(),
                            Sl = Convert.ToInt32(worksheet.Cells[row, 6].Text.Trim()),
                        };
                        if (string.IsNullOrEmpty(hanghoas.TenHh) ||
                              hanghoas.DonGia == null ||
                              string.IsNullOrEmpty(hanghoas.Hinh) ||
                              hanghoas.Sl == 0)
                        {
                            ModelState.AddModelError(string.Empty, "Có thể bạn đã để trống TenHh, Sl hoặc DonGia, Hinh của bạn bằng 0 ");
                        }
                        string maThTenTh = worksheet.Cells[row, 8].Text.Trim();
                        if (!string.IsNullOrEmpty(maThTenTh))
                        {
                            // Tách MaPb và TenPb
                            string[] parts = maThTenTh.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 1)
                            {
                                int maTH;
                                if (int.TryParse(parts[0].Trim(), out maTH))
                                {
                                    var thuongHieu = db.ThuongHieus.Find(maTH);
                                    if (thuongHieu != null)
                                    {
                                        hanghoas.MaTh = maTH;
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, "Thương hiệu không tồn tại ");
                                    }
                                }
                            }

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Thương hiệu không được để trống  ");

                        }
                        string maLTenL = worksheet.Cells[row, 7].Text.Trim();
                        if (!string.IsNullOrEmpty(maLTenL))
                        {
                            // Tách MaPb và TenPb
                            string[] parts = maLTenL.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length >= 1)
                            {
                                int maL;
                                if (int.TryParse(parts[0].Trim(), out maL))
                                {
                                    var loai = db.Loais.Find(maL);
                                    if (loai != null)
                                    {
                                        hanghoas.MaLoai = maL;

                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, "Loại không tồn tại ");
                                    }
                                }
                            }

                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Loại không được để trống  ");

                        }
                        Hanghoas.Add(hanghoas);
                    }

                    // Lưu danh sách nhân viên vào cơ sở dữ liệu
                    // Ví dụ lưu vào DbContext (db) của bạn
                    foreach (var nv in Hanghoas)
                    {
                        // Xử lý thêm vào db
                        db.HangHoas.Add(nv);
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
        // GET: Admin/HangHoaAdmin
        public async Task<IActionResult> Index()
        {
            var webncContext = db.HangHoas.Include(h => h.MaLoaiNavigation).Include(h => h.MaThNavigation);
            return View(await webncContext.ToListAsync());
        }

        // GET: Admin/HangHoaAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 5, "xem");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            if (id == null || db.HangHoas == null)
            {
                return NotFound();
            }

            var hangHoa = await db.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .Include(h => h.MaThNavigation)
                .FirstOrDefaultAsync(m => m.MaHh == id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            return View(hangHoa);
        }

        // GET: Admin/HangHoaAdmin/Create
        public async Task<IActionResult> Create()
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 5, "them");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            ViewData["MaLoai"] = new SelectList(db.Loais, "MaLoai", "TenLoai");
            ViewData["MaTh"] = new SelectList(db.ThuongHieus, "MaTh", "TenTh");
            return View();
        }

        // POST: Admin/HangHoaAdmin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HangHoaVM hangHoaViewModel)
        {
            if (ModelState.IsValid)
            {
                var hangHoa = new HangHoa
                {
                    MaHh = hangHoaViewModel.MaHh,
                    TenHh = hangHoaViewModel.TenHH,
                    MaLoai = hangHoaViewModel.MaLoai,
                    DonGia = hangHoaViewModel.DonGia,
                    MoTa = hangHoaViewModel.MoTaNgan,
                    MaTh = hangHoaViewModel.MaTh,
                    Sl = hangHoaViewModel.SoLuongTon
                };

                if (hangHoaViewModel.HinhFile != null)
                {
                    var fileName = Path.GetFileName(hangHoaViewModel.HinhFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "HangHoa", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await hangHoaViewModel.HinhFile.CopyToAsync(stream);
                    }

                    hangHoa.Hinh = "/Hinh/HangHoa/" + fileName;
                }

                db.Add(hangHoa);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }


            ViewData["MaLoai"] = new SelectList(db.Loais, "MaLoai", "TenLoai", hangHoaViewModel.MaLoai);
            ViewData["MaTh"] = new SelectList(db.ThuongHieus, "MaTh", "TenTh", hangHoaViewModel.MaTh);
            return View(hangHoaViewModel);
        }


        // GET: Admin/HangHoaAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 5, "sua");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            if (id == null || db.HangHoas == null)
            {
                return NotFound();
            }

            var hangHoa = await db.HangHoas
               .Include(h => h.MaLoaiNavigation)
               .Include(h => h.MaThNavigation)
               .FirstOrDefaultAsync(m => m.MaHh == id);

            if (hangHoa == null)
            {
                return NotFound();
            }

            var hangHoaVM = new HangHoaVM
            {
                MaHh = hangHoa.MaHh,
                TenHH = hangHoa.TenHh,
                MaLoai = hangHoa.MaLoai,
                DonGia = hangHoa.DonGia,
                MoTaNgan = hangHoa.MoTa,
                MaTh = hangHoa.MaTh,
                SoLuongTon = hangHoa.Sl,
                Hinh = hangHoa.Hinh // Lưu đường dẫn hình cũ
            };

            ViewData["MaLoai"] = new SelectList(db.Loais, "MaLoai", "TenLoai", hangHoa.MaLoai);
            ViewData["MaTh"] = new SelectList(db.ThuongHieus, "MaTh", "TenTh", hangHoa.MaTh);

            return View(hangHoaVM);
        }
        //

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HangHoaVM hangHoaViewModel)
        {
            if (id != hangHoaViewModel.MaHh)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["MaLoai"] = new SelectList(db.Loais, "MaLoai", "TenLoai", hangHoaViewModel.MaLoai);
                ViewData["MaTh"] = new SelectList(db.ThuongHieus, "MaTh", "TenTh", hangHoaViewModel.MaTh);
                return View(hangHoaViewModel);
            }

            var hangHoa = await db.HangHoas.FindAsync(id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            hangHoa.TenHh = hangHoaViewModel.TenHH;
            hangHoa.MaLoai = hangHoaViewModel.MaLoai;
            hangHoa.DonGia = hangHoaViewModel.DonGia;
            hangHoa.MoTa = hangHoaViewModel.MoTaNgan;
            hangHoa.MaTh = hangHoaViewModel.MaTh;
            hangHoa.Sl = hangHoaViewModel.SoLuongTon;

            if (hangHoaViewModel.HinhFile != null)
            {
                var fileName = Path.GetFileName(hangHoaViewModel.HinhFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "HangHoa", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await hangHoaViewModel.HinhFile.CopyToAsync(stream);
                }

                hangHoa.Hinh = "/Hinh/HangHoa/" + fileName;
            }

            else
            {
                // Giữ lại hình cũ nếu không có hình mới
                hangHoa.Hinh = hangHoaViewModel.Hinh;
            }

            db.Entry(hangHoa).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Admin/HangHoaAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            // Kiểm tra quyền trước khi thực hiện 
            int manv = NVHienTai();
            bool coQuyen = await _kiemTraQuyen.KiemTraChucNang(manv, 5, "xoa");
            if (!coQuyen)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập vào trang này.";
                return RedirectToAction("Index");
            }
            // Kết thúc KTQ 
            if (id == null || db.HangHoas == null)
            {
                return NotFound();
            }

            var hangHoa = await db.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .Include(h => h.MaThNavigation)
                .FirstOrDefaultAsync(m => m.MaHh == id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            return View(hangHoa);
        }
        // POST: Admin/HangHoaAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (db.HangHoas == null)
            {
                return Problem("Entity set 'WebncContext.HangHoas'  is null.");
            }
            var hangHoa = await db.HangHoas.FindAsync(id);
            if (hangHoa != null)
            {
                db.HangHoas.Remove(hangHoa);
            }

            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HangHoaExists(int id)
        {
            return (db.HangHoas?.Any(e => e.MaHh == id)).GetValueOrDefault();
        }
    }
}
