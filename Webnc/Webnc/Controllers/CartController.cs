using HienlthOnline.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using Webnc.Helpers;
using Webnc.Models;
using Webnc.Services;
using Webnc.ViewModels;

namespace Webnc.Controllers
{
    public class CartController : Controller
    {
        private readonly WebncContext db;
        private readonly IVnPayService _vnPayservice;
        private readonly ProductService _productService;


        public CartController(WebncContext context, IVnPayService vnPayservice, ProductService productService)
        {
            db = context;
            _vnPayservice = vnPayservice;
            _productService = productService;
        }
        private void CalculateCartTotals(List<CartItem> cartItems, out CartCalculator cartCalculator)
        {
            cartCalculator = new CartCalculator(cartItems);
            var discountCode = HttpContext.Session.GetString("DiscountCode");
            if (!string.IsNullOrEmpty(discountCode))
            {
                var discount = db.GiamGia.FirstOrDefault(d => d.Code == discountCode);
                if (discount != null)
                {
                    cartCalculator.CalculateTotals(true, discount.DonGia);
                    return;
                }
            }
            cartCalculator.CalculateTotals(false, 0);
        }
        private void ReduceDiscountQuantity(WebncContext db)
        {
            var discountCode = HttpContext.Session.GetString("DiscountCode");
            if (!string.IsNullOrEmpty(discountCode))
            {
                var discount = db.GiamGia.FirstOrDefault(d => d.Code == discountCode);
                if (discount != null && discount.Sl > 0)
                {
                    discount.Sl -= 1;
                    db.Update(discount);
                    db.SaveChanges();
                }
            }
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
        //public IActionResult Index()
        //{
        //    var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

        //    // Check if a discount code has been applied
        //    var discountCode = HttpContext.Session.GetString("DiscountCode");
        //    if (!string.IsNullOrEmpty(discountCode))
        //    {
        //        var discount = db.GiamGia.FirstOrDefault(d => d.Code == discountCode);
        //        if (discount != null)
        //        {
        //            var cartCalculator = new CartCalculator(cartItems);
        //            cartCalculator.CalculateTotals(true, discount.DonGia);

        //            ViewBag.TenGG = discount.TenGg;
        //            ViewBag.NgayBD = discount.NgayBd.ToString("dd/MM/yyyy");
        //            ViewBag.NgayKT = discount.NgayKt.ToString("dd/MM/yyyy");
        //            ViewBag.DonGia = string.Format("{0:n0} đ", discount.DonGia);
        //        }
        //    }
        //    else
        //    {
        //        var cartCalculator = new CartCalculator(cartItems);
        //        cartCalculator.CalculateTotals(false, 0);
        //    }

        //    HttpContext.Session.Set(MySetting.CART_KEY, cartItems);

        //    return View(cartItems);
        //}
        public IActionResult Index()
        {
            var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
            CalculateCartTotals(cartItems, out var cartCalculator);

            var discountCode = HttpContext.Session.GetString("DiscountCode");
            if (!string.IsNullOrEmpty(discountCode))
            {
                var discount = db.GiamGia.FirstOrDefault(d => d.Code == discountCode);
                if (discount != null)
                {
                    ViewBag.TenGG = discount.TenGg;
                    ViewBag.NgayBD = discount.NgayBd.ToString("dd/MM/yyyy");
                    ViewBag.NgayKT = discount.NgayKt.ToString("dd/MM/yyyy");
                    ViewBag.DonGia = string.Format("{0:n0} đ", discount.DonGia);
                }
            }

            HttpContext.Session.Set(MySetting.CART_KEY, cartItems);
            return View(cartItems);
        }


        public async Task<IActionResult> AddToCart(int id, int soLuong, string type = "Normal")
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);

            if (!await _productService.IsStockAvailableAsync(id, soLuong))
            {
                TempData["ErrorMessage"] = "Số lượng sản phẩm không đủ trong kho.";
                return RedirectToAction("Index", "Products");
            }

            if (item == null)
            {
                var hangHoa = await db.HangHoas.SingleOrDefaultAsync(p => p.MaHh == id);
                if (hangHoa == null)
                {
                    TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                    return Redirect("/404");
                }
                item = new CartItem
                {
                    MaHh = hangHoa.MaHh,
                    TenHH = hangHoa.TenHh,
                    DonGia = hangHoa.DonGia ?? 0,
                    Hinh = hangHoa.Hinh ?? string.Empty,
                    SoLuong = soLuong
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += soLuong;
            }

            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);

            if (type == "ajax")
            {
                return Json(new { SoLuong = Cart.Sum(c => c.SoLuong) });
            }
            return RedirectToAction("Index");
        }
     
        [HttpPost]
        //public IActionResult ApplyDiscountCode(string discountCode, int productId)
        //{
        //    var discount = db.GiamGia.FirstOrDefault(d => d.Code == discountCode &&
        //                                                        DateTime.Now >= d.NgayBd &&
        //                                                        DateTime.Now <= d.NgayKt);
        //    if (discount == null)
        //    {
        //        return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn." });
        //    }
        //    // Check if the discount is applicable to the product
        //    var isDiscountApplicable = db.ChiTietGgs.Any(ctg => ctg.MaGg == discount.MaGg && ctg.MaHh == productId);
        //    if (!isDiscountApplicable)
        //    {
        //        return Json(new { success = false, message = "Mã giảm giá không áp dụng cho sản phẩm này." });
        //    }


        //    //if (discount == null)
        //    //{
        //    //    return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn." });
        //    //}
        //    //var product = db.HangHoas.FirstOrDefault(p => p.MaHh == productId);
        //    //if (!IsDiscountApplicableToProduct(discount, productId))
        //    //{
        //    //    return Json(new { success = false, message = "Mã giảm giá không áp dụng cho sản phẩm này." });
        //    //}

        //    var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
        //    var cartTotal = cartItems.Sum(item => item.ThanhTien);

        //    if (cartTotal < discount.GiaAd)
        //    {
        //        return Json(new { success = false, message = $"Tổng giá trị giỏ hàng của bạn là {cartTotal:n0} đ. Hãy mua tối thiểu {discount.GiaAd:n0} đ để áp dụng mã giảm giá này." });
        //    }
        //    // Apply discount to cart items
        //    var cartCalculator = new CartCalculator(cartItems);
        //    cartCalculator.CalculateTotals(true, discount.DonGia);
        //    HttpContext.Session.Set(MySetting.CART_KEY, cartItems);
        //    // Check if quantity is sufficient
        //    if (discount.Sl <= 0)
        //    {
        //        return Json(new { success = false, message = "Số lượng mã giảm giá đã hết. Vui lòng thử mã khác." });
        //    }

        //    // Save the discount code in session
        //    HttpContext.Session.SetString("DiscountCode", discountCode);

        //    return Json(new { success = true });
        //}
        public IActionResult ApplyDiscountCode(string discountCode, int productId)
        {
            var discount = db.GiamGia.FirstOrDefault(d => d.Code == discountCode &&
                                                             DateTime.Now >= d.NgayBd &&
                                                             DateTime.Now <= d.NgayKt);
            if (discount == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn." });
            }

            var isDiscountApplicable = db.ChiTietGgs.Any(ctg => ctg.MaGg == discount.MaGg && ctg.MaHh == productId);
            if (!isDiscountApplicable)
            {
                return Json(new { success = false, message = "Mã giảm giá không áp dụng cho sản phẩm này." });
            }

            var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
            var cartTotal = cartItems.Sum(item => item.ThanhTien);

            if (cartTotal < discount.GiaAd)
            {
                return Json(new { success = false, message = $"Tổng giá trị giỏ hàng của bạn là {cartTotal:n0} đ. Hãy mua tối thiểu {discount.GiaAd:n0} đ để áp dụng mã giảm giá này." });
            }

            // CalculateCartTotals(cartItems, out var cartCalculator);
            var cartCalculator = new CartCalculator(cartItems);
            var totalDiscount = discount.DonGia; // Số tiền giảm giá cho sản phẩm cụ thể
            cartCalculator.CalculateTotals(true, totalDiscount);
            HttpContext.Session.Set(MySetting.CART_KEY, cartItems);

            if (discount.Sl <= 0)
            {
                return Json(new { success = false, message = "Số lượng mã giảm giá đã hết. Vui lòng thử mã khác." });
            }

            HttpContext.Session.SetString("DiscountCode", discountCode);
            return Json(new { success = true });
        }


        [HttpPost]
        public IActionResult RemoveDiscountCode()
        {
            // Xóa mã giảm giá khỏi session
            HttpContext.Session.Remove("DiscountCode");

            // Lấy lại giỏ hàng từ session
            var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

            // Tính toán lại tổng tiền không có mã giảm giá
            var cartCalculator = new CartCalculator(cartItems);
            cartCalculator.CalculateTotals(false, 0); // Không áp dụng giảm giá

            // Cập nhật lại giỏ hàng trong session
            HttpContext.Session.Set(MySetting.CART_KEY, cartItems);

            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(int id, int soLuong, string type = "Normal")
        {
            if (soLuong < 1)
            {
                soLuong = 1;
            }

            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                if (!await _productService.IsStockAvailableAsync(id, soLuong - item.SoLuong))
                {
                    TempData["ErrorMessage"] = "Số lượng sản phẩm không đủ trong kho.";
                    return RedirectToAction("Index");
                }
                item.SoLuong = soLuong;
            }
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return View(Cart);
            }
            else
            {
                return Redirect("/TaiKhoan/DangNhap");
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model, string payment = "COD")
        {
            if (ModelState.IsValid)
            {
                // VNPAY 
                if (payment == "Thanh toán VNPay")
                {
                    //var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_MAKH).Value;
                    var customerId = int.Parse(HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_MAKH).Value);
                    var khachHang = new KhachHang();
                    // Kiểm tra xem GiongKhachHang có được chọn hay không 
                    if (model.GiongKhachHang)
                    {
                        khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(model.HoTen))
                        { ModelState.AddModelError(string.Empty, "Bạn chưa điền họ tên "); }
                        if (string.IsNullOrWhiteSpace(model.DiaChi))
                        { ModelState.AddModelError(string.Empty, "Bạn chưa điền tên địa chỉ"); }
                        if (string.IsNullOrWhiteSpace(model.DienThoai))
                        { ModelState.AddModelError(string.Empty, "Bạn chưa điền tên điện thoại"); }
                        if (!ModelState.IsValid)
                        {
                            return View(Cart);
                        }
                    }
                    var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
                    CalculateCartTotals(cartItems, out var cartCalculator);

                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = cartCalculator.ThanhTien,
                        CreatedDate = DateTime.Now,
                        //HoTen = model.HoTen,
                        //DiaChi = model.DiaChi,
                        //DienThoai = model.DienThoai,
                        HoTen = model.HoTen ?? khachHang.HoTen,
                        DiaChi = model.DiaChi ?? khachHang.DiaChi,
                        DienThoai = model.DienThoai ?? khachHang.DienThoai,
                        GhiChu = model.GhiChu,
                        OrderId = new Random().Next(1000, 100000)
                    };
                    var vnPayModelJson = JsonConvert.SerializeObject(vnPayModel);
                    TempData["VnPayModel"] = vnPayModelJson;
                    return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
                }
                // COD
                else
                {
                    //var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_MAKH).Value;
                    var customerId = int.Parse(HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_MAKH).Value);
                    var khachHang = new KhachHang();
                    // Kiểm tra xem GiongKhachHang có được chọn hay không 
                    if (model.GiongKhachHang)
                    {
                        khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(model.HoTen))
                        { ModelState.AddModelError(string.Empty, "Bạn chưa điền họ tên "); }
                        if (string.IsNullOrWhiteSpace(model.DiaChi))
                        { ModelState.AddModelError(string.Empty, "Bạn chưa điền tên địa chỉ"); }
                        if (string.IsNullOrWhiteSpace(model.DienThoai))
                        { ModelState.AddModelError(string.Empty, "Bạn chưa điền tên điện thoại"); }

                    }
                    //var cartCalculator = new CartCalculator(Cart);
                    //cartCalculator.CalculateTotals(false, 0);
                    //var cartCalculator = new CartCalculator(Cart);
                    //var discountCode = HttpContext.Session.GetString("DiscountCode");

                    //if (!string.IsNullOrEmpty(discountCode))
                    //{
                    //    var discount = db.GiamGia.FirstOrDefault(d => d.Code == discountCode);
                    //    if (discount != null)
                    //    {
                    //        cartCalculator.CalculateTotals(true, discount.DonGia);
                    //    }
                    //}
                    //else
                    //{
                    //    cartCalculator.CalculateTotals(false, 0);
                    //}
                    var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
                    CalculateCartTotals(cartItems, out var cartCalculator);
                    var hoadon = new HoaDon
                    {
                        MaKh = customerId,
                        HoTen = model.HoTen ?? khachHang.HoTen,
                        DiaChi = model.DiaChi ?? khachHang.DiaChi,
                        DienThoai = model.DienThoai ?? khachHang.DienThoai,
                        NgayDat = DateTime.Now,
                        CachThanhToan = "COD",
                        TrangThaiTt = "Chưa thanh toán",
                        ThanhTien = cartCalculator.ThanhTien,
                        PhiVanChuyen = cartCalculator.PhiVanChuyen,
                        TongTien = cartCalculator.TongTien,
                        MaTrangThai = 0,
                        GhiChu = model.GhiChu
                    };
                    db.Database.BeginTransaction();
                    {
                        try
                        {

                            db.Add(hoadon);
                            db.SaveChanges();

                            var cthds = new List<ChiTietHd>();
                            var discountCode = HttpContext.Session.GetString("DiscountCode");
                            var discount = !string.IsNullOrEmpty(discountCode)
                                ? db.GiamGia.FirstOrDefault(d => d.Code == discountCode)
                                : null;
                            foreach (var item in cartItems)
                            {
                                var cartTotal = item.ThanhTien;
                                double tienGG = 0;
                                double totalDiscount = 0;
                                if (discount != null)
                                {
                                    if (cartTotal >= discount.GiaAd)
                                    {
                                        totalDiscount = discount.DonGia;

                                        var discountDetails = db.ChiTietGgs
                                            .Where(cd => cd.MaGg == discount.MaGg)
                                            .ToList();

                                        // Xác định giá trị giảm giá cho sản phẩm
                                        tienGG = discountDetails.Any(dd => dd.MaHh == item.MaHh) ? totalDiscount : 0;
                                    }
                                    else
                                    {
                                        totalDiscount = 0;
                                        tienGG = 0; // Không áp dụng giảm giá
                                    }
                                }
                                cthds.Add(new ChiTietHd
                                {
                                    MaHd = hoadon.MaHd,
                                    SoLuong = item.SoLuong,
                                    DonGia = item.DonGia,
                                    MaHh = item.MaHh,
                                    TienGg = tienGG, 
                                    TienCuoiCung = ( item.DonGia * item.SoLuong) - tienGG
                                });

                                var hangHoa = db.HangHoas.SingleOrDefault(hh => hh.MaHh == item.MaHh);
                                if (hangHoa != null)
                                {
                                    hangHoa.Sl -= item.SoLuong;
                                    db.Update(hangHoa);
                                }
                            }
                            ReduceDiscountQuantity(db);
                            //foreach (var item in Cart)
                            //{
                            //    cthds.Add(new ChiTietHd
                            //    {
                            //        MaHd = hoadon.MaHd,
                            //        SoLuong = item.SoLuong,
                            //        DonGia = item.DonGia,
                            //        MaHh = item.MaHh,

                            //    });
                            //    // Cập nhật số lượng đơn hàng 
                            //    var hangHoa = db.HangHoas.SingleOrDefault(hh => hh.MaHh == item.MaHh);
                            //    if (hangHoa != null)
                            //    {
                            //        hangHoa.Sl -= item.SoLuong;
                            //        db.Update(hangHoa);
                            //    }
                            //    ReduceDiscountQuantity(db);
                            //}
                            db.AddRange(cthds);
                            db.SaveChanges();
                            db.Database.CommitTransaction();
                            //Hoàn thành giao dịch
                            HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());
                            // return new JsonResult(new { success = true, message = "Đặt hàng thành công" });
                            return View("PaymentSuccess");
                        }
                        catch
                        {
                            db.Database.RollbackTransaction();
                        }
                    }
                }
            }
            return View(Cart);
        }
        [Authorize]
        public IActionResult PaymentSuccess()
        {
            return View();
        }

        [Authorize]
        public IActionResult PaymentFail()
        {
            return View();
        }


        [Authorize]


        public IActionResult PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            // Truy xuất chuỗi JSON từ TempData
            var vnPayModelJson = TempData["VnPayModel"] as string;
            // Kiểm tra xem chuỗi JSON có tồn tại hay không:
            if (string.IsNullOrEmpty(vnPayModelJson))
            {
                TempData["Message"] = "Không tìm thấy thông tin đơn hàng";
                return RedirectToAction("PaymentFail");
            }

            // Chuyển đổi chuỗi JSON thành đối tượng VnPaymentRequestModel
            var vnPayModel = JsonConvert.DeserializeObject<VnPaymentRequestModel>(vnPayModelJson);
            //var customerId = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_MAKH).Value;
            var customerId = int.Parse(HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_MAKH).Value);
            //var cartCalculator = new CartCalculator(Cart);
            //cartCalculator.CalculateTotals(false, 0);
            //var cartCalculator = new CartCalculator(Cart);
            //var discountCode = HttpContext.Session.GetString("DiscountCode");
            //if (!string.IsNullOrEmpty(discountCode))
            //{
            //    var discount = db.GiamGia.FirstOrDefault(d => d.Code == discountCode);
            //    if (discount != null)
            //    {
            //        cartCalculator.CalculateTotals(true, discount.DonGia);
            //    }
            //}
            //else
            //{
            //    cartCalculator.CalculateTotals(false, 0);
            //}
            // Calculate cart totals using the new method
            var cartItems = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
            CalculateCartTotals(cartItems, out var cartCalculator);

           
                // Lưu thông tin đơn hàng vào cơ sở dữ liệu
                var hoadon = new HoaDon
            {
                MaKh = customerId,
                HoTen = vnPayModel.HoTen,
                DiaChi = vnPayModel.DiaChi,
                DienThoai = vnPayModel.DienThoai,
                NgayDat = vnPayModel.CreatedDate,
                CachThanhToan = "VNPAY",
                TrangThaiTt = "Đã thanh toán",
                ThanhTien = vnPayModel.Amount,
                PhiVanChuyen = cartCalculator.PhiVanChuyen,
                TongTien = cartCalculator.TongTien,
                MaTrangThai = 0,
                GhiChu = vnPayModel.GhiChu,
            };
            db.Database.BeginTransaction();
            {
                try
                {

                    db.Add(hoadon);
                    db.SaveChanges();
                    var cthds = new List<ChiTietHd>();
                    var discountCode = HttpContext.Session.GetString("DiscountCode");
                    var discount = !string.IsNullOrEmpty(discountCode)
                        ? db.GiamGia.FirstOrDefault(d => d.Code == discountCode)
                        : null;                
                        foreach (var item in cartItems)
                    {
                        var cartTotal = item.ThanhTien;
                        double tienGG = 0;
                        double totalDiscount = 0;
                        if (discount != null)
                        {
                            if (cartTotal >= discount.GiaAd)
                            {
                                totalDiscount = discount.DonGia;

                                var discountDetails = db.ChiTietGgs
                                    .Where(cd => cd.MaGg == discount.MaGg)
                                    .ToList();

                                // Xác định giá trị giảm giá cho sản phẩm
                                tienGG = discountDetails.Any(dd => dd.MaHh == item.MaHh) ? totalDiscount : 0;
                            }
                            else
                            {
                                totalDiscount = 0;
                                tienGG = 0; // Không áp dụng giảm giá
                            }
                        }
                        cthds.Add(new ChiTietHd
                            {
                                MaHd = hoadon.MaHd,
                                SoLuong = item.SoLuong,
                                DonGia = item.DonGia,
                                MaHh = item.MaHh,
                                TienGg = tienGG,
                            TienCuoiCung = (item.DonGia * item.SoLuong) - tienGG
                        });

                            var hangHoa = db.HangHoas.SingleOrDefault(hh => hh.MaHh == item.MaHh);
                            if (hangHoa != null)
                            {
                                hangHoa.Sl -= item.SoLuong;
                                db.Update(hangHoa);
                            }
                        }
                        ReduceDiscountQuantity(db);
                    
                    //else
                    //{
                    //    foreach (var item in cartItems)
                    //    {
                    //        cthds.Add(new ChiTietHd
                    //        {
                    //            MaHd = hoadon.MaHd,
                    //            SoLuong = item.SoLuong,
                    //            DonGia = item.DonGia,
                    //            MaHh = item.MaHh,
                    //            TienGg = 0 // Không áp dụng giảm giá
                    //        });

                    //        var hangHoa = db.HangHoas.SingleOrDefault(hh => hh.MaHh == item.MaHh);
                    //        if (hangHoa != null)
                    //        {
                    //            hangHoa.Sl -= item.SoLuong;
                    //            db.Update(hangHoa);
                    //        }
                    //    }
                    //}
                     db.AddRange(cthds);
                    db.SaveChanges();
                    db.Database.CommitTransaction();
                    //Hoàn thành giao dịch
                    HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());
                    return View("PaymentSuccess");
                }
                catch
                {
                    db.Database.RollbackTransaction();
                }
            }
            TempData["Message"] = $"Thanh toán VNPay thành công";
            return RedirectToAction("PaymentSuccess");
        }
    }
}




