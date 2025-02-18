using System;
using System.Collections.Generic;

namespace Webnc.Models
{
    public partial class PhongBan
    {
        public PhongBan()
        {
            NhanViens = new HashSet<NhanVien>();
        }

        public int MaPb { get; set; }
        public string TenPb { get; set; } = null!;

        public virtual ICollection<NhanVien> NhanViens { get; set; }
    }
}
