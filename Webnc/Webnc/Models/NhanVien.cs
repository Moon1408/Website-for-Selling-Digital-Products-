using System;
using System.Collections.Generic;

namespace Webnc.Models
{
    public partial class NhanVien
    {
        public NhanVien()
        {
            PhanQuyens = new HashSet<PhanQuyen>();
        }

        public int MaNv { get; set; }
        public string TenDn { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public string? Email { get; set; }
        public int MaPb { get; set; }
        public bool HieuLuc { get; set; }

        public virtual PhongBan MaPbNavigation { get; set; } = null!;
        public virtual ICollection<PhanQuyen> PhanQuyens { get; set; }
    }
}
