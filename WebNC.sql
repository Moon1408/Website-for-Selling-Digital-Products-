IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Webnc')
BEGIN
  CREATE DATABASE Webnc;
END;
GO

USE [Webnc]
GO
/****** Object:  Table [dbo].[ChiTietHD]    Script Date: 12/1/2023 7:35:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChiTietHD](
	[MaCT] [int] IDENTITY(1,1) NOT NULL,
	[MaHD] [int] NOT NULL,
	[MaHH] [int] NOT NULL,
	[DonGia] [float] NOT NULL,
	[SoLuong] [int] NOT NULL,
	[TienGG] [float] NOT NULL,
	[TienCuoiCung] [float] NOT NULL,
	-- khóa chính 
 CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED 
(
	[MaCT] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HangHoa]    Script Date: 12/1/2023 7:35:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HangHoa](
	[MaHH] [int] IDENTITY(1,1) NOT NULL,
	[TenHH] [nvarchar](50) NOT NULL,
	-- [TenAlias] [nvarchar](50) NULL,
	[MaLoai] [int] NOT NULL,
	-- [MoTaDonVi] [nvarchar](50) NULL,
	[DonGia] [float] NULL,
	[Hinh] [nvarchar](250) NULL,
	-- [NgaySX] [datetime] NOT NULL,
	-- [GiamGia] [float] NOT NULL,
	-- [SoLanXem] [int] NOT NULL,
	 [MoTa] [nvarchar](max) NULL,
	 [MaTH] [int] NOT NULL,
	 [SL] [int] NOT NULL,
	 --[MaGG] [int] NOT NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[MaHH] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GiamGia]    Script Date: 12/1/2023 7:35:08 AM ******/
CREATE TABLE [dbo].[GiamGia](
	[MaGG] [int] IDENTITY(1,1) NOT NULL,
	[TenGG] [nvarchar](50) NOT NULL,
	[NgayBD] [datetime] NOT NULL,
	[NgayKT] [datetime] NOT NULL,
	[DonGia] [float] NOT NULL,
	[GiaAD] [float] NOT NULL,
	[Code] [varchar](250) NOT NULL,
	[SL] [int] NOT NULL,
 CONSTRAINT [PK_Discount] PRIMARY KEY CLUSTERED 
(
	[MaGG] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


/****** Object:  Table [dbo].[ChiTietGG] Script Date: 12/1/2023 7:35:08 AM ******/

CREATE TABLE [dbo].[ChiTietGG](
	[MaCTGG] [int] IDENTITY(1,1) NOT NULL,
	[MaHH] [int] NOT NULL,
	[MaGG] [int] NOT NULL,
	-- khóa chính 
 CONSTRAINT [PK_DetailsDicount] PRIMARY KEY CLUSTERED 
(
	[MaCTGG] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HoaDon]    Script Date: 12/1/2023 7:35:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HoaDon](
	[MaHD] [int] IDENTITY(1,1) NOT NULL,
	[MaKH] [int]  NOT NULL,
	[NgayDat] [datetime] NOT NULL,
	[HoTen] [nvarchar](50) NULL,
	[DiaChi] [nvarchar](60) NOT NULL,
	[DienThoai] [nvarchar](24) NULL,
	[CachThanhToan] [nvarchar](50) NOT NULL,
	[TrangThaiTT] [nvarchar](50) NOT NULL,
	[ThanhTien] [float] NOT NULL,
	[PhiVanChuyen] [float] NOT NULL,
	[TongTien] [float] NOT NULL,
	--[MaNV] [nvarchar](50) NULL,
	[GhiChu] [nvarchar](50) NULL,
	[MaTrangThai] [int] NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[MaHD] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KhachHang]    Script Date: 12/1/2023 7:35:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KhachHang](
	[MaKH] [int] IDENTITY(1,1) NOT NULL,
	[TenDN] [nvarchar](20) NOT NULL,
	[MatKhau] [nvarchar](50) NULL,
	[HoTen] [nvarchar](50) NOT NULL,
	[DiaChi] [nvarchar](60) NULL,
	[DienThoai] [nvarchar](24) NULL,
	[Email] [nvarchar](50) NOT NULL,
	[HieuLuc] [bit] NOT NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[MaKH] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChiTietGG] Script Date: 12/1/2023 7:35:08 AM ******/

CREATE TABLE [dbo].[BinhLuan](
	[MaBL] [int] IDENTITY(1,1) NOT NULL,
	[MaHH] [int] NOT NULL,
	[MaKH] [int] NOT NULL,
	[MoTa] [nvarchar](max) NULL,
	[NgayBL] [datetime] NOT NULL,
	[SoSao] INT CHECK (SoSao >= 1 AND SoSao <= 5),
	-- khóa chính 
 CONSTRAINT [PK_Review] PRIMARY KEY CLUSTERED 
(
	[MaBL] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Loai]    Script Date: 12/1/2023 7:35:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Loai](
    [MaLoai] [int] IDENTITY(1,1) NOT NULL,
    [TenLoai] [nvarchar](50) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
    (
        [MaLoai] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ThuongHieu]   Script Date: 12/1/2023 7:35:08 AM ******/
CREATE TABLE [dbo].[ThuongHieu](
    [MaTH] [int] IDENTITY(1,1) NOT NULL,
    [TenTH] [nvarchar](50) NOT NULL,
    CONSTRAINT [PK_Brand] PRIMARY KEY CLUSTERED 
    (
        [MaTH] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO 

/****** Object:  Table [dbo].[NhanVien]    Script Date: 12/1/2023 7:35:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NhanVien](
	[MaNV] [int] IDENTITY(1,1) NOT NULL,
	[TenDN] [nvarchar](20) NOT NULL,
	[MatKhau] [nvarchar](50) NOT NULL,
	[HoTen] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](50)  NULL,
	[MaPB] [int] NOT NULL,
	[HieuLuc] [bit] NOT NULL,
 CONSTRAINT [PK_NhanVien] PRIMARY KEY CLUSTERED 
(
	[MaNV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Trang]   Script Date: 12/1/2023 7:35:08 AM ******/
CREATE TABLE [dbo].[Trang](
	[MaTrang] [int] IDENTITY(1,1) NOT NULL,
	[TenTrang] [nvarchar](50) NOT NULL
 CONSTRAINT [PK_Trang] PRIMARY KEY CLUSTERED 
(
	[MaTrang] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO

/****** Object:  Table [dbo].[PhongBan]    Script Date: 12/1/2023 7:35:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhongBan](
	[MaPB]  [int] IDENTITY(1,1) NOT NULL,
	[TenPB] [nvarchar](50) NOT NULL
 CONSTRAINT [PK_PhongBan] PRIMARY KEY CLUSTERED 
(
	[MaPB] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] 
GO
/****** Object:  Table [dbo].[PhanQuyen]    Script Date: 12/1/2023 7:35:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PhanQuyen](
	[MaPQ] [int] IDENTITY(1,1) NOT NULL,
    [MaNV] [int] NOT NULL,
    [MaTrang] [int] NOT NULL,
    [Them] [bit] NOT NULL,
    [Sua] [bit] NOT NULL,
    [Xoa] [bit] NOT NULL,
    [Xem] [bit] NOT NULL,
 CONSTRAINT [PK_PhanQuyen] PRIMARY KEY CLUSTERED 
(
   [MaPQ] ASC
)WITH (
    PAD_INDEX = OFF, 
    STATISTICS_NORECOMPUTE = OFF, 
    IGNORE_DUP_KEY = OFF, 
    ALLOW_ROW_LOCKS = ON, 
    ALLOW_PAGE_LOCKS = ON, 
    OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
) ON [PRIMARY]
) ON [PRIMARY];


/****** Object:  Table [dbo].[TrangThai]    Script Date: 12/1/2023 7:35:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrangThai](
	[MaTrangThai] [int] NOT NULL,
	[TenTrangThai] [nvarchar](50) NOT NULL,
	[MoTa] [nvarchar](500) NULL,
 CONSTRAINT [PK_TrangThai] PRIMARY KEY CLUSTERED 
(
	[MaTrangThai] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[YeuThich]    Script Date: 12/1/2023 7:35:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[YeuThich](
	[MaYT] [int] IDENTITY(1,1) NOT NULL,
	[MaHH] [int] NULL,
	[MaKH] [int]  NOT NULL,
	[NgayChon] [datetime] NULL,
	[MoTa] [nvarchar](255) NULL,
 CONSTRAINT [PK_Favorites] PRIMARY KEY CLUSTERED 
(
	[MaYT] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[HangHoa] ON 
-- HÀNG HÓA 
-- LOA
-- HÀNG HÓA 
-- LOA
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1001, N'Loa Bluetooth JBL Partybox', 1000, 9990000, N'Loa1.jpg', 
		N'Loa Bluetooth JBL Partybox Encore 2Mic sở hữu hệ thống đèn LED nổi bật cùng thiết kế vuông vắn đẹp mắt, 
		công nghệ âm thanh sống động và mạnh mẽ, tốc độ kết nối ổn định, tiện lợi sử dụng trong những bữa tiệc cá nhân 
		và các buổi tụ họp bạn bè hàng ngày.',1003, 10 )
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1002, N'Loa Bluetooth AVA+ MiniPod Y23', 1000, 90000, N'Loa2.jpg', 
		N'Loa Bluetooth AVA+ MiniPod Y23 sở hữu thiết kế nhỏ nhắn vừa vặn tay cầm, trang bị âm thanh sống động 
		và mạnh mẽ, hỗ trợ chống nước IPX7, tiện lợi sử dụng cho không gian phòng ngủ hay không gian nhỏ.', 1003, 10)
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1003, N'Loa Bluetooth AVA+ Go Z12', 1000, 120000, N'Loa3.jpg', 
		N'Loa Bluetooth AVA+ Go Z12 sở hữu ngoại hình nhỏ gọn với đèn LED ấn tượng, công nghệ Bluetooth 5.3 
		hiện đại và khả năng kết nối cùng lúc 2 loa giúp không gian giải trí của bạn thêm phần sống động.', 1003, 10)
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1004, N'Loa Bluetooth AVA+ Go Z09', 1000, 90000, N'Loa4.jpg', 
		N'Loa Bluetooth AVA+ Go Z09 là phiên bản loa giá rẻ nhưng có thiết kế ấn tượng với ngoại hình nhỏ gọn, 
		đèn LED sống động, đi cùng các tiện ích hiện đại, phục vụ tốt nhu cầu giải trí cơ bản của người dùng.', 1003, 10)
--Laptop 8 1005 1006 
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1005, N'Laptop Apple MacBook Air 13 inch M1', 1001, 18190000, N'L1.jpg', 
		N'  Laptop Apple MacBook Air M1 2020 thuộc dòng laptop cao cấp sang trọng có cấu hình mạnh mẽ, chinh phục được 
			các tính năng văn phòng lẫn đồ hoạ mà bạn mong muốn, thời lượng pin dài, 
			thiết kế mỏng nhẹ sẽ đáp ứng tốt các nhu cầu làm việc của bạn.',1000, 10)
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1006, N'Laptop HP 15s fq5229TU i3', 1001, 10790000, N'L2.jpg', 
		N'  Với một mức giá thành lý tưởng, bạn đã có thể sở hữu cho mình một mẫu laptop học tập - văn phòng có hiệu năng ổn định, ngoại hình thanh lịch cùng các tính năng hiện đại. Laptop HP 15s fq5229TU i3 1215U (8U237PA) chắc chắn sẽ đáp ứng đầy đủ nhu cầu làm việc, 
			học tập và giải trí thường ngày của sinh viên cũng như người đi làm.',1001, 10)
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1007, N'Laptop Asus Vivobook 15 OLED A1505ZA i5', 1001, 16490000, N'L3.jpg', 
		N'  Laptop Asus Vivobook 15 OLED A1505ZA i5 12500H (L1337W) có không gian hiển thị rộng rãi, sắc nét với màn hình 15.6 inch OLED cùng nhiều hiệu năng mạnh mẽ khác. Đây chắc hẳn là chiếc laptop đồ họa - kỹ thuật phù hợp 
			với những bạn đang có đòi hỏi về đồ họa hay các công việc sáng tạo.', 1000, 10)
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1008, N'Laptop Dell Inspiron 15 3520 i5', 1001, 16490000, N'L4.jpg', 
		N'  Laptop Dell Inspiron 15 3520 i5 1235U (25P231) hoàn toàn sở hữu mọi yếu tố mà một người dùng cá nhân cơ bản cần, hiệu năng ổn định từ con chip Intel i5, RAM 16 GB, màn hình 15.6 inch
			thoải mái cũng như sở hữu một mức giá lý tưởng tại Thế Giới Di Động.', 1001, 10)
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1009, N'Laptop HP Pavilion 14 dv2073TU i5', 1001, 18390000, N'L5.jpg', 
		N'  Laptop HP Pavilion 14 dv2073TU i5 1235U (7C0P2PA) vừa có mặt tại Thế Giới Di Động sẽ khiến bạn thực sự ấn tượng với vẻ ngoài vô cùng trẻ trung, sở hữu thông số cấu hình mạnh mẽ cho bất cứ nhu cầu sử dụng cá nhân nào. 
			Đây chính xác là một mẫu laptop học tập - văn phòng dành cho mọi nhà.', 1000, 10)
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1010, N'Laptop Asus Vivobook Go 15 E1504FA R5', 1001, 12490000, N'L6.jpg', 
		null , 1000, 10)
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1011, N'Laptop Acer Aspire Lite 15 51M 5542 i5', 1001, 12290000, N'L7.jpg', 
		null, 1001, 10)
-- Bàn phím máy tính  
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (10012, N'Bàn Phím Cơ Dareu EK75 Pro', 1002, 1000000, N'BP1.jpg', 
		N'  Bàn Phím Cơ Bluetooth Dareu EK75 Pro sở hữu đầy đủ đặc điểm cần có của một mẫu bàn phím cơ cao cấp, đèn LED RGB đa sắc màu giúp 
			không gian làm việc/giải trí nổi bật trong màn đêm, trang bị switch độc quyền cho hành trình phím sâu, 
			không chiếm quá nhiều diện tích trên bàn làm việc hay balo.',1004, 10)
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (10013, N'Bàn Phím Có Dây Gaming Rapoo V50S', 1002, 390000, N'BP2.jpg', 
		N'  Bàn Phím Có Dây Gaming Rapoo V50S sở hữu ngoại hình đẹp mắt, hệ thống đèn LED RGB độc đáo, 
			kích thước gọn gàng phù hợp với mọi không gian bàn làm việc hay bàn học.', 1004, 10)
-- Chuột máy tính
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1014, N'Chuột Không Dây Silent Rapoo B2S', 1003, 150000, N'CMT1.jpg', 
		N'  Chuột Không Dây Silent Rapoo B2S có kích thước vừa vặn tay cầm, kết nối nhanh chóng và độ nhạy chuột cao,
			mang đến cho bạn những trải nghiệm sử dụng chuột tối ưu.', 1002, 10)
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1015, N'Chuột Không Dây Silent Rapoo B2S', 1003, 150000, N'CMT1.jpg', 
		N'  Chuột Không Dây Silent Rapoo B2S có kích thước vừa vặn tay cầm, kết nối nhanh chóng và độ nhạy chuột cao,
			mang đến cho bạn những trải nghiệm sử dụng chuột tối ưu.', 1002, 10)
INSERT [dbo].[HangHoa] ([MaHH], [TenHH], [MaLoai], [DonGia], [Hinh],[MoTa], [MaTH], [SL]) 
VALUES (1016, N'Chuột Bluetooth Silent Logitech M240', 1003, 335000, N'CMT2.jpg', 
		N'  Chuột Bluetooth Silent Logitech M240 với kiểu dáng gọn gàng, gam màu đẹp mắt, kích thước vừa vặn tay cầm, 
			kết nối ổn định cùng độ nhạy khá cao, hứa hẹn mang đến cho bạn những trải nghiệm tuyệt vời.', 1002, 10) 

SET IDENTITY_INSERT [dbo].[HangHoa] OFF
GO

SET IDENTITY_INSERT [dbo].[KhachHang] ON 
GO
INSERT [dbo].[KhachHang] ([MaKH], [TenDN],[MatKhau], [HoTen], [DiaChi], [DienThoai], [Email], [HieuLuc] )
VALUES (1, N'Nhi123', N'1', N'Nhi Pham', N'123/34D Bùi Minh Trực P5 Q8', N'0703470085', N'nhipham@abc.com', 1)
INSERT [dbo].[KhachHang] ([MaKH], [TenDN],[MatKhau], [HoTen], [DiaChi], [DienThoai], [Email], [HieuLuc] )
VALUES (2, N'Nhi148', N'1', N'Nhi Tran', N'195/34D Bùi Minh Trực P5 Q8', N'0703470085', N'nhitran@gmail.com', 1)
INSERT [dbo].[KhachHang] ([MaKH], [TenDN],[MatKhau], [HoTen], [DiaChi], [DienThoai], [Email], [HieuLuc] )
VALUES (3, N'Nhung123', N'1', N'Nhung', N'195/34D Phạm Thế Hiển P5 Q8', N'0703470089', N'nhung@gmail.com', 1)
INSERT [dbo].[KhachHang] ([MaKH], [TenDN],[MatKhau], [HoTen], [DiaChi], [DienThoai], [Email], [HieuLuc] )
VALUES (4, N'Thien123', N'1', N'Thien', N'195/34D Phạm Thế Hiển P6 Q10', N'0703471189', N'thien@gmail.com', 1)
INSERT [dbo].[KhachHang] ([MaKH], [TenDN],[MatKhau], [HoTen], [DiaChi], [DienThoai], [Email], [HieuLuc])
VALUES (5, N'Ngan123', N'1', N'Ngan', N'195/34D Phạm Thế Hiển P5 Q8', N'0703472289', N'ngan@gmail.com', 1)
SET IDENTITY_INSERT [dbo].[KhachHang] OFF
GO

-- LOẠI 
SET IDENTITY_INSERT [dbo].[Loai] ON 
INSERT [dbo].[Loai] ([MaLoai], [TenLoai]) VALUES (1000, N'Loa')
INSERT [dbo].[Loai] ([MaLoai], [TenLoai]) VALUES (1001, N'Laptop')
INSERT [dbo].[Loai] ([MaLoai], [TenLoai]) VALUES (1002, N'Bàn phím máy tính')
INSERT [dbo].[Loai] ([MaLoai], [TenLoai]) VALUES (1003, N'Chuột máy tính')
SET IDENTITY_INSERT [dbo].[Loai] OFF
-- THƯƠNG HIỆU 
SET IDENTITY_INSERT [dbo].[ThuongHieu] ON 
INSERT [dbo].[ThuongHieu] ([MaTH], [TenTH]) VALUES (1000, N'Asus')
INSERT [dbo].[ThuongHieu] ([MaTH], [TenTH]) VALUES (1001, N'Dell')
INSERT [dbo].[ThuongHieu] ([MaTH], [TenTH]) VALUES (1002, N'Ugreen')
INSERT [dbo].[ThuongHieu] ([MaTH], [TenTH]) VALUES (1003, N'Ava')
INSERT [dbo].[ThuongHieu] ([MaTH], [TenTH]) VALUES (1004, N'Dareu')

SET IDENTITY_INSERT [dbo].[ThuongHieu] OFF
-- Giảm giá 
SET IDENTITY_INSERT [dbo].[GiamGia] ON;
INSERT [dbo].[GiamGia] ([MaGG], [TenGG], [NgayBD], [NgayKT], [DonGia],[GiaAD] ,[Code], [SL]) 
VALUES (1,N'Giảm 50.000đ cho đơn mua tối thiểu 19.980.000 đ', CAST(N'2024-07-16T16:00:00.000' AS DateTime),CAST(N'2024-07-30T16:00:00.000' AS DateTime), 50000,19980000, 1, 10)
INSERT [dbo].[GiamGia] ([MaGG], [TenGG], [NgayBD], [NgayKT], [DonGia],[GiaAD] ,[Code], [SL]) 
VALUES (2,N'Giảm 20.000đ cho đơn mua tối thiểu 500.000 đ', CAST(N'2024-07-16T16:00:00.000' AS DateTime),CAST(N'2024-07-30T16:00:00.000' AS DateTime), 20000,500000, 2, 10)
SET IDENTITY_INSERT [dbo].[GiamGia] OFF;
-- CT GG
SET IDENTITY_INSERT [dbo].[ChiTietGG] ON;
INSERT [dbo].[ChiTietGG] ([MaCTGG], [MaHH], [MaGG]) 
VALUES (1,1001,1)
INSERT [dbo].[ChiTietGG] ([MaCTGG], [MaHH], [MaGG]) 
VALUES (2,1001,2)
INSERT [dbo].[ChiTietGG] ([MaCTGG], [MaHH], [MaGG]) 
VALUES (3,1002,1)
INSERT [dbo].[ChiTietGG] ([MaCTGG], [MaHH], [MaGG]) 
VALUES (4,1002,2)
SET IDENTITY_INSERT [dbo].[ChiTietGG] OFF;
---
SET IDENTITY_INSERT [dbo].[BinhLuan] ON;
INSERT [dbo].[BinhLuan] ([MaBL], [MaHH], [MaKH], [MoTa], [NgayBL], [SoSao]) 
VALUES (1,1001,1, null, CAST(N'2024-07-01T00:00:00.000' AS DateTime), 5)
SET IDENTITY_INSERT [dbo].[BinhLuan] OFF;
-- HóaDon
SET IDENTITY_INSERT [dbo].[HoaDon] ON
INSERT INTO [dbo].[HoaDon] 
( [MaHD], [MaKH],  [NgayDat], [HoTen], [DiaChi], [DienThoai],  [CachThanhToan],
[TrangThaiTT], [ThanhTien], [PhiVanChuyen], [TongTien], [GhiChu], 
[MaTrangThai]
)
VALUES (1, 1, CAST(N'2023-05-23T00:00:00.000' AS DateTime),N'Nhi Pham', N'123/34D Bùi Minh Trực P5 Q8',N'0703470085', N'COD'
		,N'Chưa thanh toán',9990000, 0, 9990000,  NULL, 0 ) 
INSERT INTO [dbo].[HoaDon] 
( [MaHD], [MaKH],  [NgayDat], [HoTen], [DiaChi], [DienThoai],  [CachThanhToan],
[TrangThaiTT], [ThanhTien], [PhiVanChuyen], [TongTien], [GhiChu], 
[MaTrangThai]
)
VALUES (2, 2, CAST(N'2023-05-23T00:00:00.000' AS DateTime), N'Nhi Tran', N'195/34D Bùi Minh Trực P5 Q8', N'0703470085', N'COD'
		,N'Chưa thanh toán',9990000, 0, 9990000,  NULL, 0 )  
SET IDENTITY_INSERT [dbo].[HoaDon] OFF
-- CTHD 
SET IDENTITY_INSERT [dbo].[ChiTietHD] ON 
INSERT INTO [dbo].[ChiTietHD] ([MaCT], [MaHD], [MaHH], [DonGia], [SoLuong],  [TienGG], [TienCuoiCung]  ) 
VALUES (1, 1, 1001 ,9990000,1,0, 9990000)
INSERT INTO [dbo].[ChiTietHD]([MaCT], [MaHD], [MaHH], [DonGia], [SoLuong],  [TienGG], [TienCuoiCung] ) 
VALUES (2, 2, 1001 ,9990000,1, 0, 9990000)
SET IDENTITY_INSERT [dbo].[ChiTietHD] OFF
GO
-- Nhan Vien
SET IDENTITY_INSERT [dbo].[NhanVien] ON 
INSERT [dbo].[NhanVien] ([MaNV], [TenDN], [MatKhau], [HoTen], [Email], [MaPB], [HieuLuc]) 
VALUES (1, N'ql1',N'1', N'Quản lý 1 ', N'ql1@gmail.com', 1, 1 )
INSERT [dbo].[NhanVien] ([MaNV], [TenDN], [MatKhau], [HoTen], [Email] , [MaPB], [HieuLuc]) 
VALUES (2, N'nv1',N'1', N'Nhân viên 1', N'nv1@gmail.com', 2, 1 )
SET IDENTITY_INSERT [dbo].[NhanVien] OFF
GO
-- Phân quyền 
SET IDENTITY_INSERT [dbo].[PhanQuyen] ON 
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (1, 1, 1, 1, 1,1, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (2, 2, 1 , 0, 0, 0, 0)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (3, 1, 2 , 1, 1, 1, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (4, 2, 2 , 0, 0, 0, 0)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (5, 1, 3 , 1, 1, 1, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (6, 2, 3 , 0, 0, 0, 0)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (7, 1, 4 , 1, 1, 1, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (8, 2, 4 , 0, 0, 0, 0)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (9, 1, 5 , 1, 1, 1, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (10, 2,5, 1, 1, 0, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (11, 1,6, 1, 1, 1, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (12, 2,6, 0, 1, 0, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (13, 1,7, 1, 1, 1, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (14, 2,7, 0, 1, 0, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (15, 1,8, 1, 1, 1, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (16, 2,8, 1, 1, 0, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (17, 1,9, 1, 1, 1, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (18, 2,9, 1, 1, 0, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (19, 1,10, 1, 1, 1, 1)
INSERT [dbo].[PhanQuyen] ([MaPQ],[MaNV], [MaTrang], [Them], [Sua], [Xoa], [Xem]) VALUES (20, 1,11, 1, 1, 1, 1)
SET IDENTITY_INSERT [dbo].[PhanQuyen] OFF
GO
-- Phòng ban 
SET IDENTITY_INSERT [dbo].[PhongBan] ON 
INSERT [dbo].[PhongBan] ([MaPB], [TenPB]) VALUES (1, N'Quản lý')
INSERT [dbo].[PhongBan] ([MaPB], [TenPB]) VALUES (2, N'Nhân viên')

SET IDENTITY_INSERT [dbo].[PhongBan] OFF
GO
-- Trang 
SET IDENTITY_INSERT [dbo].[Trang] ON 
INSERT [dbo].[Trang] ([MaTrang], [TenTrang]) VALUES (1, N'Nhân viên')
INSERT [dbo].[Trang] ([MaTrang], [TenTrang]) VALUES (2, N'Phân quyền')
INSERT [dbo].[Trang] ([MaTrang], [TenTrang]) VALUES (3, N'Chức vụ')
INSERT [dbo].[Trang] ([MaTrang], [TenTrang]) VALUES (4, N'Trang')
INSERT [dbo].[Trang] ([MaTrang], [TenTrang]) VALUES (5, N'Hàng hóa')
INSERT [dbo].[Trang] ([MaTrang], [TenTrang]) VALUES (6, N'Hóa đơn')
INSERT [dbo].[Trang] ([MaTrang], [TenTrang]) VALUES (7, N'Khách hàng')
INSERT [dbo].[Trang] ([MaTrang], [TenTrang]) VALUES (8, N'Loại')
INSERT [dbo].[Trang] ([MaTrang], [TenTrang]) VALUES (9, N'Thương hiệu')
INSERT [dbo].[Trang] ([MaTrang], [TenTrang]) VALUES (10, N'Thông tin giảm giá')
INSERT [dbo].[Trang] ([MaTrang], [TenTrang]) VALUES (11, N'Giảm giá cho sản phẩm')
SET IDENTITY_INSERT [dbo].[Trang] OFF
GO

-- 
INSERT [dbo].[TrangThai] ([MaTrangThai], [TenTrangThai], [MoTa]) VALUES (-1, N'Đã hủy', NULL)
INSERT [dbo].[TrangThai] ([MaTrangThai], [TenTrangThai], [MoTa]) VALUES (0, N'Mới đặt hàng', NULL)
INSERT [dbo].[TrangThai] ([MaTrangThai], [TenTrangThai], [MoTa]) VALUES (1,  N'Chờ giao hàng', NULL)
INSERT [dbo].[TrangThai] ([MaTrangThai], [TenTrangThai], [MoTa]) VALUES (2, N'Đã giao hàng', NULL)

ALTER TABLE [dbo].[ChiTietHD] ADD  CONSTRAINT [DF_Order_Details_UnitPrice]  DEFAULT ((0)) FOR [DonGia]
GO
ALTER TABLE [dbo].[ChiTietHD] ADD  CONSTRAINT [DF_Order_Details_Quantity]  DEFAULT ((1)) FOR [SoLuong]
GO


ALTER TABLE [dbo].[HangHoa] ADD  CONSTRAINT [DF_Products_UnitPrice]  DEFAULT ((0)) FOR [DonGia]
GO
ALTER TABLE [dbo].[KhachHang] ADD  CONSTRAINT [DF_Customers_Active]  DEFAULT ((0)) FOR [HieuLuc]
GO
ALTER TABLE [dbo].[PhanQuyen] ADD  CONSTRAINT [DF_PhanQuyen_Them]  DEFAULT ((0)) FOR [Them]
GO
ALTER TABLE [dbo].[PhanQuyen] ADD  CONSTRAINT [DF_PhanQuyen_Sua]  DEFAULT ((0)) FOR [Sua]
GO
ALTER TABLE [dbo].[PhanQuyen] ADD  CONSTRAINT [DF_PhanQuyen_Xoa]  DEFAULT ((0)) FOR [Xoa]
GO
ALTER TABLE [dbo].[PhanQuyen] ADD  CONSTRAINT [DF_PhanQuyen_Xem]  DEFAULT ((0)) FOR [Xem]
GO

ALTER TABLE [dbo].[NhanVien] ADD  CONSTRAINT [DF_NV_Active]  DEFAULT ((0)) FOR [HieuLuc]
GO

ALTER TABLE [dbo].[ChiTietHD]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Orders] FOREIGN KEY([MaHD])
REFERENCES [dbo].[HoaDon] ([MaHD])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ChiTietHD]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Products] FOREIGN KEY([MaHH])
REFERENCES [dbo].[HangHoa] ([MaHH])
ON UPDATE CASCADE
ON DELETE CASCADE
ALTER TABLE [dbo].[ChiTietHD] CHECK CONSTRAINT [FK_OrderDetails_Orders]
GO
ALTER TABLE [dbo].[HangHoa]  WITH CHECK ADD  CONSTRAINT [FK_Products_Categories] FOREIGN KEY([MaLoai])
REFERENCES [dbo].[Loai] ([MaLoai])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HangHoa] CHECK CONSTRAINT [FK_Products_Categories]
GO

ALTER TABLE [dbo].[HangHoa] WITH CHECK ADD CONSTRAINT [FK_Products_Brand] 
FOREIGN KEY([MaTH]) REFERENCES [dbo].[ThuongHieu] ([MaTH])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ChiTietGG] WITH CHECK ADD CONSTRAINT [FK_Products_DetailDiscount] 
FOREIGN KEY([MaGG]) REFERENCES [dbo].[GiamGia] ([MaGG])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ChiTietGG] WITH CHECK ADD CONSTRAINT [FK_Discount_DetailDiscount] 
FOREIGN KEY([MaHH]) REFERENCES [dbo].[HangHoa] ([MaHH])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BinhLuan] WITH CHECK ADD CONSTRAINT [FK_Products_Review] 
FOREIGN KEY([MaHH]) REFERENCES [dbo].[HangHoa] ([MaHH])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[BinhLuan] WITH CHECK ADD CONSTRAINT [FK_Customer_Review] 
FOREIGN KEY([MaKH]) REFERENCES [dbo].[KhachHang] ([MaKH])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HangHoa] CHECK CONSTRAINT [FK_Products_Brand]
GO
ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD  CONSTRAINT [FK_HoaDon_TrangThai] FOREIGN KEY([MaTrangThai])
REFERENCES [dbo].[TrangThai] ([MaTrangThai])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[HoaDon] CHECK CONSTRAINT [FK_HoaDon_TrangThai]
GO
ALTER TABLE [dbo].[HoaDon]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Customers] FOREIGN KEY([MaKH])
REFERENCES [dbo].[KhachHang] ([MaKH])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[HoaDon] CHECK CONSTRAINT [FK_Orders_Customers]
GO
ALTER TABLE [dbo].[PhanQuyen]  WITH CHECK ADD  CONSTRAINT [FK_PhanQuyen_Trang] FOREIGN KEY([MaTrang])
REFERENCES [dbo].[Trang] ([MaTrang])
GO
ALTER TABLE [dbo].[PhanQuyen]  WITH CHECK ADD  CONSTRAINT [FK_PhanQuyen_NhanVien] FOREIGN KEY([MaNV])
REFERENCES [dbo].[NhanVien] ([MaNV])
GO
ALTER TABLE [dbo].[NhanVien]  WITH CHECK ADD  CONSTRAINT [FK_NhanVien_PhongBan] FOREIGN KEY([MaPB])
REFERENCES [dbo].[PhongBan] ([MaPB])
GO

ALTER TABLE [dbo].[PhanQuyen] CHECK CONSTRAINT  [FK_PhanQuyen_NhanVien]
GO
ALTER TABLE [dbo].[PhanQuyen] CHECK CONSTRAINT [FK_PhanQuyen_Trang]
GO
ALTER TABLE [dbo].[NhanVien] CHECK CONSTRAINT [FK_NhanVien_PhongBan]
GO
ALTER TABLE PhanQuyen
ADD CONSTRAINT FK_MaNv_PhongBan
FOREIGN KEY (MaNV)
REFERENCES NhanVien(MaNv)
ON DELETE CASCADE;


ALTER TABLE [dbo].[YeuThich]  WITH CHECK ADD  CONSTRAINT [FK_Favorites_Customers] FOREIGN KEY([MaKH])
REFERENCES [dbo].[KhachHang] ([MaKH])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[YeuThich] CHECK CONSTRAINT [FK_Favorites_Customers]
GO
ALTER TABLE [dbo].[YeuThich]  WITH CHECK ADD  CONSTRAINT [FK_YeuThich_HangHoa] FOREIGN KEY([MaHH])
REFERENCES [dbo].[HangHoa] ([MaHH])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[YeuThich] CHECK CONSTRAINT [FK_YeuThich_HangHoa]
GO
