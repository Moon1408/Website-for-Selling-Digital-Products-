using System.ComponentModel.DataAnnotations;

namespace Webnc.ViewModels
{
    public class CheckoutVM
	{
		public bool GiongKhachHang { get; set; }

        //[Display(Name = "HoTen")]
        //[Required(ErrorMessage = "Họ tên không được bỏ trống")]

        public string? HoTen { get; set; }
        //[Display(Name = "DiaChi")]
        //[Required(ErrorMessage = "Địa chỉ không được bỏ trống")]
        public string? DiaChi { get; set; }
        //[Display(Name = "DienThoai")]
        //[Required(ErrorMessage = "Điện thoại không được bỏ trống")]
        public string? DienThoai { get; set; }
		public string? GhiChu { get; set; }
	}
}
