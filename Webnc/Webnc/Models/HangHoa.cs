using System;
using System.Collections.Generic;

namespace Webnc.Models
{
    public partial class HangHoa
    {
        public HangHoa()
        {
            BinhLuans = new HashSet<BinhLuan>();
            ChiTietGgs = new HashSet<ChiTietGg>();
            ChiTietHds = new HashSet<ChiTietHd>();
            YeuThiches = new HashSet<YeuThich>();
        }

        public int MaHh { get; set; }
        public string TenHh { get; set; } = null!;
        public int MaLoai { get; set; }
        public double? DonGia { get; set; }
        public string? Hinh { get; set; }
        public string? MoTa { get; set; }
        public int MaTh { get; set; }
        public int Sl { get; set; }

        public virtual Loai MaLoaiNavigation { get; set; } = null!;
        public virtual ThuongHieu MaThNavigation { get; set; } = null!;
        public virtual ICollection<BinhLuan> BinhLuans { get; set; }
        public virtual ICollection<ChiTietGg> ChiTietGgs { get; set; }
        public virtual ICollection<ChiTietHd> ChiTietHds { get; set; }
        public virtual ICollection<YeuThich> YeuThiches { get; set; }
    }
}
