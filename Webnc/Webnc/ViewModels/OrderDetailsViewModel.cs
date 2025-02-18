namespace Webnc.ViewModels
{
    public class OrderDetailsViewModel
    {
        public int MaHd { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public string DienThoai { get; set; }
        public DateTime NgayDat { get; set; }
        public double TongTien { get; set; }
        public string TenTrangThai { get; set; } // Thêm thuộc tính TenTrangThai
        public List<OrderItemViewModel> Items { get; set; }
    }

    public class OrderItemViewModel
    {
        public string TenHh { get; set; }
        public int SoLuong { get; set; }
        public double DonGia { get; set; }
        public double TienCuoiCung { get; set; }
        public double TienGG {  get; set; }
    }

    public class OrderListViewModel
    {
        public int MaHd { get; set; }
        public DateTime NgayDat { get; set; }
        public double TongTien { get; set; }
        public string TenTrangThai { get; set; } // Thêm thuộc tính TenTrangThai
        public string CachThanhToan { get; set; }
    }
}
