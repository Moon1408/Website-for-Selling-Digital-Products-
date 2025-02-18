using System;
using System.Collections.Generic;

namespace Webnc.Models
{
    public partial class ThuongHieu
    {
        public ThuongHieu()
        {
            HangHoas = new HashSet<HangHoa>();
        }

        public int MaTh { get; set; }
        public string TenTh { get; set; } = null!;

        public virtual ICollection<HangHoa> HangHoas { get; set; }
    }
}
