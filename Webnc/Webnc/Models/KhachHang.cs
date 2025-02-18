using System;
using System.Collections.Generic;

namespace Webnc.Models
{
    public partial class KhachHang
    {
        public KhachHang()
        {
            BinhLuans = new HashSet<BinhLuan>();
            HoaDons = new HashSet<HoaDon>();
            YeuThiches = new HashSet<YeuThich>();
        }

        public int MaKh { get; set; }
        public string TenDn { get; set; } = null!;
        public string? MatKhau { get; set; }
        public string HoTen { get; set; } = null!;
        public string? DiaChi { get; set; }
        public string? DienThoai { get; set; }
        public string Email { get; set; } = null!;
        public bool HieuLuc { get; set; }

        public virtual ICollection<BinhLuan> BinhLuans { get; set; }
        public virtual ICollection<HoaDon> HoaDons { get; set; }
        public virtual ICollection<YeuThich> YeuThiches { get; set; }
    }
}
