using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Webnc.Models
{
    public partial class WebncContext : DbContext
    {
        public WebncContext()
        {
        }

        public WebncContext(DbContextOptions<WebncContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BinhLuan> BinhLuans { get; set; } = null!;
        public virtual DbSet<ChiTietGg> ChiTietGgs { get; set; } = null!;
        public virtual DbSet<ChiTietHd> ChiTietHds { get; set; } = null!;
        public virtual DbSet<GiamGium> GiamGia { get; set; } = null!;
        public virtual DbSet<HangHoa> HangHoas { get; set; } = null!;
        public virtual DbSet<HoaDon> HoaDons { get; set; } = null!;
        public virtual DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public virtual DbSet<Loai> Loais { get; set; } = null!;
        public virtual DbSet<NhanVien> NhanViens { get; set; } = null!;
        public virtual DbSet<PhanQuyen> PhanQuyens { get; set; } = null!;
        public virtual DbSet<PhongBan> PhongBans { get; set; } = null!;
        public virtual DbSet<ThuongHieu> ThuongHieus { get; set; } = null!;
        public virtual DbSet<Trang> Trangs { get; set; } = null!;
        public virtual DbSet<TrangThai> TrangThais { get; set; } = null!;
        public virtual DbSet<YeuThich> YeuThiches { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=TUYETNHI;Initial Catalog=Webnc;Integrated Security=True;Trust Server Certificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BinhLuan>(entity =>
            {
                entity.HasKey(e => e.MaBl)
                    .HasName("PK_Review");

                entity.ToTable("BinhLuan");

                entity.Property(e => e.MaBl).HasColumnName("MaBL");

                entity.Property(e => e.MaHh).HasColumnName("MaHH");

                entity.Property(e => e.MaKh).HasColumnName("MaKH");

                entity.Property(e => e.NgayBl)
                    .HasColumnType("datetime")
                    .HasColumnName("NgayBL");

                entity.HasOne(d => d.MaHhNavigation)
                    .WithMany(p => p.BinhLuans)
                    .HasForeignKey(d => d.MaHh)
                    .HasConstraintName("FK_Products_Review");

                entity.HasOne(d => d.MaKhNavigation)
                    .WithMany(p => p.BinhLuans)
                    .HasForeignKey(d => d.MaKh)
                    .HasConstraintName("FK_Customer_Review");
            });

            modelBuilder.Entity<ChiTietGg>(entity =>
            {
                entity.HasKey(e => e.MaCtgg)
                    .HasName("PK_DetailsDicount");

                entity.ToTable("ChiTietGG");

                entity.Property(e => e.MaCtgg).HasColumnName("MaCTGG");

                entity.Property(e => e.MaGg).HasColumnName("MaGG");

                entity.Property(e => e.MaHh).HasColumnName("MaHH");

                entity.HasOne(d => d.MaGgNavigation)
                    .WithMany(p => p.ChiTietGgs)
                    .HasForeignKey(d => d.MaGg)
                    .HasConstraintName("FK_Products_DetailDiscount");

                entity.HasOne(d => d.MaHhNavigation)
                    .WithMany(p => p.ChiTietGgs)
                    .HasForeignKey(d => d.MaHh)
                    .HasConstraintName("FK_Discount_DetailDiscount");
            });

            modelBuilder.Entity<ChiTietHd>(entity =>
            {
                entity.HasKey(e => e.MaCt)
                    .HasName("PK_OrderDetails");

                entity.ToTable("ChiTietHD");

                entity.Property(e => e.MaCt).HasColumnName("MaCT");

                entity.Property(e => e.MaHd).HasColumnName("MaHD");

                entity.Property(e => e.MaHh).HasColumnName("MaHH");

                entity.Property(e => e.SoLuong).HasDefaultValueSql("((1))");

                entity.Property(e => e.TienGg).HasColumnName("TienGG");

                entity.HasOne(d => d.MaHdNavigation)
                    .WithMany(p => p.ChiTietHds)
                    .HasForeignKey(d => d.MaHd)
                    .HasConstraintName("FK_OrderDetails_Orders");

                entity.HasOne(d => d.MaHhNavigation)
                    .WithMany(p => p.ChiTietHds)
                    .HasForeignKey(d => d.MaHh)
                    .HasConstraintName("FK_OrderDetails_Products");
            });

            modelBuilder.Entity<GiamGium>(entity =>
            {
                entity.HasKey(e => e.MaGg)
                    .HasName("PK_Discount");

                entity.Property(e => e.MaGg).HasColumnName("MaGG");

                entity.Property(e => e.Code)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.GiaAd).HasColumnName("GiaAD");

                entity.Property(e => e.NgayBd)
                    .HasColumnType("datetime")
                    .HasColumnName("NgayBD");

                entity.Property(e => e.NgayKt)
                    .HasColumnType("datetime")
                    .HasColumnName("NgayKT");

                entity.Property(e => e.Sl).HasColumnName("SL");

                entity.Property(e => e.TenGg)
                    .HasMaxLength(50)
                    .HasColumnName("TenGG");
            });

            modelBuilder.Entity<HangHoa>(entity =>
            {
                entity.HasKey(e => e.MaHh)
                    .HasName("PK_Products");

                entity.ToTable("HangHoa");

                entity.Property(e => e.MaHh).HasColumnName("MaHH");

                entity.Property(e => e.DonGia).HasDefaultValueSql("((0))");

                entity.Property(e => e.Hinh).HasMaxLength(250);

                entity.Property(e => e.MaTh).HasColumnName("MaTH");

                entity.Property(e => e.Sl).HasColumnName("SL");

                entity.Property(e => e.TenHh)
                    .HasMaxLength(50)
                    .HasColumnName("TenHH");

                entity.HasOne(d => d.MaLoaiNavigation)
                    .WithMany(p => p.HangHoas)
                    .HasForeignKey(d => d.MaLoai)
                    .HasConstraintName("FK_Products_Categories");

                entity.HasOne(d => d.MaThNavigation)
                    .WithMany(p => p.HangHoas)
                    .HasForeignKey(d => d.MaTh)
                    .HasConstraintName("FK_Products_Brand");
            });

            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.HasKey(e => e.MaHd)
                    .HasName("PK_Orders");

                entity.ToTable("HoaDon");

                entity.Property(e => e.MaHd).HasColumnName("MaHD");

                entity.Property(e => e.CachThanhToan).HasMaxLength(50);

                entity.Property(e => e.DiaChi).HasMaxLength(60);

                entity.Property(e => e.DienThoai).HasMaxLength(24);

                entity.Property(e => e.GhiChu).HasMaxLength(50);

                entity.Property(e => e.HoTen).HasMaxLength(50);

                entity.Property(e => e.MaKh).HasColumnName("MaKH");

                entity.Property(e => e.NgayDat).HasColumnType("datetime");

                entity.Property(e => e.TrangThaiTt)
                    .HasMaxLength(50)
                    .HasColumnName("TrangThaiTT");

                entity.HasOne(d => d.MaKhNavigation)
                    .WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.MaKh)
                    .HasConstraintName("FK_Orders_Customers");

                entity.HasOne(d => d.MaTrangThaiNavigation)
                    .WithMany(p => p.HoaDons)
                    .HasForeignKey(d => d.MaTrangThai)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HoaDon_TrangThai");
            });

            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasKey(e => e.MaKh)
                    .HasName("PK_Customers");

                entity.ToTable("KhachHang");

                entity.Property(e => e.MaKh).HasColumnName("MaKH");

                entity.Property(e => e.DiaChi).HasMaxLength(60);

                entity.Property(e => e.DienThoai).HasMaxLength(24);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.HoTen).HasMaxLength(50);

                entity.Property(e => e.MatKhau).HasMaxLength(50);

                entity.Property(e => e.TenDn)
                    .HasMaxLength(20)
                    .HasColumnName("TenDN");
            });

            modelBuilder.Entity<Loai>(entity =>
            {
                entity.HasKey(e => e.MaLoai)
                    .HasName("PK_Categories");

                entity.ToTable("Loai");

                entity.Property(e => e.TenLoai).HasMaxLength(50);
            });

            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.HasKey(e => e.MaNv);

                entity.ToTable("NhanVien");

                entity.Property(e => e.MaNv).HasColumnName("MaNV");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.HoTen).HasMaxLength(50);

                entity.Property(e => e.MaPb).HasColumnName("MaPB");

                entity.Property(e => e.MatKhau).HasMaxLength(50);

                entity.Property(e => e.TenDn)
                    .HasMaxLength(20)
                    .HasColumnName("TenDN");

                entity.HasOne(d => d.MaPbNavigation)
                    .WithMany(p => p.NhanViens)
                    .HasForeignKey(d => d.MaPb)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NhanVien_PhongBan");
            });

            modelBuilder.Entity<PhanQuyen>(entity =>
            {
                entity.HasKey(e => e.MaPq);

                entity.ToTable("PhanQuyen");

                entity.Property(e => e.MaPq).HasColumnName("MaPQ");

                entity.Property(e => e.MaNv).HasColumnName("MaNV");

                entity.HasOne(d => d.MaNvNavigation)
                    .WithMany(p => p.PhanQuyens)
                    .HasForeignKey(d => d.MaNv)
                    .HasConstraintName("FK_MaNv_PhongBan");

                entity.HasOne(d => d.MaTrangNavigation)
                    .WithMany(p => p.PhanQuyens)
                    .HasForeignKey(d => d.MaTrang)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PhanQuyen_Trang");
            });

            modelBuilder.Entity<PhongBan>(entity =>
            {
                entity.HasKey(e => e.MaPb);

                entity.ToTable("PhongBan");

                entity.Property(e => e.MaPb).HasColumnName("MaPB");

                entity.Property(e => e.TenPb)
                    .HasMaxLength(50)
                    .HasColumnName("TenPB");
            });

            modelBuilder.Entity<ThuongHieu>(entity =>
            {
                entity.HasKey(e => e.MaTh)
                    .HasName("PK_Brand");

                entity.ToTable("ThuongHieu");

                entity.Property(e => e.MaTh).HasColumnName("MaTH");

                entity.Property(e => e.TenTh)
                    .HasMaxLength(50)
                    .HasColumnName("TenTH");
            });

            modelBuilder.Entity<Trang>(entity =>
            {
                entity.HasKey(e => e.MaTrang);

                entity.ToTable("Trang");

                entity.Property(e => e.TenTrang).HasMaxLength(50);
            });

            modelBuilder.Entity<TrangThai>(entity =>
            {
                entity.HasKey(e => e.MaTrangThai);

                entity.ToTable("TrangThai");

                entity.Property(e => e.MaTrangThai).ValueGeneratedNever();

                entity.Property(e => e.MoTa).HasMaxLength(500);

                entity.Property(e => e.TenTrangThai).HasMaxLength(50);
            });

            modelBuilder.Entity<YeuThich>(entity =>
            {
                entity.HasKey(e => e.MaYt)
                    .HasName("PK_Favorites");

                entity.ToTable("YeuThich");

                entity.Property(e => e.MaYt).HasColumnName("MaYT");

                entity.Property(e => e.MaHh).HasColumnName("MaHH");

                entity.Property(e => e.MaKh).HasColumnName("MaKH");

                entity.Property(e => e.MoTa).HasMaxLength(255);

                entity.Property(e => e.NgayChon).HasColumnType("datetime");

                entity.HasOne(d => d.MaHhNavigation)
                    .WithMany(p => p.YeuThiches)
                    .HasForeignKey(d => d.MaHh)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_YeuThich_HangHoa");

                entity.HasOne(d => d.MaKhNavigation)
                    .WithMany(p => p.YeuThiches)
                    .HasForeignKey(d => d.MaKh)
                    .HasConstraintName("FK_Favorites_Customers");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
