namespace Webnc.ViewModels
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }
    }

    public class VnPaymentRequestModel
    {
        public int OrderId { get; set; }
        //public string FullName { get; set; }
        //public string Description { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? HoTen { get; set; }
        public string DiaChi { get; set; } = null!;
        public string? DienThoai { get; set; }
        //public string CachThanhToan { get; set; } = null!;
        //public string TrangThaiTt { get; set; } = null!;
        //public double ThanhTien { get; set; }
        //public double PhiVanChuyen { get; set; }

        public string? GhiChu { get; set; }

    }
}
