using System;
using System.Collections.Generic;

namespace Webnc.Models
{
    public partial class PhanQuyen
    {
        public int MaPq { get; set; }
        public int MaNv { get; set; }
        public int MaTrang { get; set; }
        public bool Them { get; set; }
        public bool Sua { get; set; }
        public bool Xoa { get; set; }
        public bool Xem { get; set; }

        public virtual NhanVien MaNvNavigation { get; set; } = null!;
        public virtual Trang MaTrangNavigation { get; set; } = null!;
    }
}
