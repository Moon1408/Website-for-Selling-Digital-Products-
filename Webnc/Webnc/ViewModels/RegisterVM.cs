using System.ComponentModel.DataAnnotations;

namespace Webnc.ViewModels
{
	public class RegisterVM
	{
       


        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Tên đăng nhập không được bỏ trống")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]

        public string TenDN { get; set; } = String.Empty;


        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = String.Empty;

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "Họ tên không được bỏ trống")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public string? HoTen { get; set; } = String.Empty;

        [Display(Name = "Địa chỉ")]
        [MaxLength(60, ErrorMessage = "Tối đa 60 kí tự")]
        public string? DiaChi { get; set; }

        [Display(Name = "Điện thoại")]
        [Required(ErrorMessage = "Điện thoại không được bỏ trống")]
        [MaxLength(24, ErrorMessage = "Tối đa 24 kí tự")]
        [RegularExpression(@"0[9875]\d{8}", ErrorMessage = "Chưa đúng định dạng di động Việt Nam")]
        public string DienThoai { get; set; } = String.Empty;


        [EmailAddress(ErrorMessage = "Chưa đúng định dạng email")]
        [Required(ErrorMessage = "Email không được bỏ trống")]
        public string Email { get; set; } = String.Empty;

        
    }
}
