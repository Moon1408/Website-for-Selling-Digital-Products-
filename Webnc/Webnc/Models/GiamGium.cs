using System;
using System.Collections.Generic;

namespace Webnc.Models
{
    public partial class GiamGium
    {
        public GiamGium()
        {
            ChiTietGgs = new HashSet<ChiTietGg>();
        }

        public int MaGg { get; set; }
        public string TenGg { get; set; } = null!;
        public DateTime NgayBd { get; set; }
        public DateTime NgayKt { get; set; }
        public double DonGia { get; set; }
        public double GiaAd { get; set; }
        public string Code { get; set; } = null!;
        public int Sl { get; set; }

        public virtual ICollection<ChiTietGg> ChiTietGgs { get; set; }
    }
}
