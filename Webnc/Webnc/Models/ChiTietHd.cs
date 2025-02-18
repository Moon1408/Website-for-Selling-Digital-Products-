using System;
using System.Collections.Generic;

namespace Webnc.Models
{
    public partial class ChiTietHd
    {
        public int MaCt { get; set; }
        public int MaHd { get; set; }
        public int MaHh { get; set; }
        public double DonGia { get; set; }
        public int SoLuong { get; set; }
        public double TienGg { get; set; }
        public double TienCuoiCung { get; set; }

        public virtual HoaDon MaHdNavigation { get; set; } = null!;
        public virtual HangHoa MaHhNavigation { get; set; } = null!;
    }
}
