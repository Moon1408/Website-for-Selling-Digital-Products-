using System;
using System.Collections.Generic;

namespace Webnc.Models
{
    public partial class Loai
    {
        public Loai()
        {
            HangHoas = new HashSet<HangHoa>();
        }

        public int MaLoai { get; set; }
        public string TenLoai { get; set; } = null!;

        public virtual ICollection<HangHoa> HangHoas { get; set; }
    }
}
