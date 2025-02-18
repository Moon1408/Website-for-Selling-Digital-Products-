namespace Webnc.ViewModels
{
    public class HangHoaVM
    {
        public int MaHh { get; set; }
        public string? TenHH { get; set; }
        public int MaLoai { get; set; }
        public double? DonGia { get; set; }
        public int MaTh { get; set; }
        public string MoTaNgan { get; set; } = String.Empty;
        public string? TenLoai { get; set; }
        public string? TenTH { get; set; }
        public int SoLuongTon { get; set; }
        public IFormFile? HinhFile { get; set; }
        public string Hinh { get; set; } = string.Empty;

    }
    public class ChiTietHangHoaVM
    {
        public int MaHh { get; set; }
        public string? TenHH { get; set; }
        public string? Hinh { get; set; }
        public double? DonGia { get; set; }
        public string MoTaNgan { get; set; } = String.Empty;
        public string? TenLoai { get; set; }
        public string? TenTH { get; set; }
        public int SoLuongTon { get; set; }
        // Thêm để hiển thị sản phẩm trong chi tiết 
        public List<HangHoaVM> SPXT { get; set; }
    }
}
