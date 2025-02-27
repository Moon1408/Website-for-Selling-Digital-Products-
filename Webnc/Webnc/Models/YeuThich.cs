﻿using System;
using System.Collections.Generic;

namespace Webnc.Models
{
    public partial class YeuThich
    {
        public int MaYt { get; set; }
        public int? MaHh { get; set; }
        public int MaKh { get; set; }
        public DateTime? NgayChon { get; set; }
        public string? MoTa { get; set; }

        public virtual HangHoa? MaHhNavigation { get; set; }
        public virtual KhachHang MaKhNavigation { get; set; } = null!;
    }
}
