using System;
using System.Collections.Generic;

namespace Webnc.Models
{
    public partial class Trang
    {
        public Trang()
        {
            PhanQuyens = new HashSet<PhanQuyen>();
        }

        public int MaTrang { get; set; }
        public string TenTrang { get; set; } = null!;

        public virtual ICollection<PhanQuyen> PhanQuyens { get; set; }
    }
}
