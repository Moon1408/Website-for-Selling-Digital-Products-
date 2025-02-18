using System;
using System.Collections.Generic;

namespace Webnc.Models
{
    public partial class ChiTietGg
    {
        public int MaCtgg { get; set; }
        public int MaHh { get; set; }
        public int MaGg { get; set; }

        public virtual GiamGium MaGgNavigation { get; set; } = null!;
        public virtual HangHoa MaHhNavigation { get; set; } = null!;
    }
}
