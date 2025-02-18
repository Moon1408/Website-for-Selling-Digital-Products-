using System;
using System.Collections.Generic;

namespace Webnc.Models
{
    public partial class BinhLuan
    {
        public int MaBl { get; set; }
        public int MaHh { get; set; }
        public int MaKh { get; set; }
        public string? MoTa { get; set; }
        public DateTime NgayBl { get; set; }
        public int? SoSao { get; set; }

        public virtual HangHoa MaHhNavigation { get; set; } = null!;
        public virtual KhachHang MaKhNavigation { get; set; } = null!;
    }
}
