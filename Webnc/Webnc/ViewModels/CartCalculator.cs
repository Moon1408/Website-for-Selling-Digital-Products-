//namespace Webnc.ViewModels
//{
//    public class CartCalculator
//    {
//        private List<CartItem> _cartItems;

//        public CartCalculator(List<CartItem> cartItems)
//        {
//            _cartItems = cartItems;
//        }

//        public double ThanhTien { get; internal set; }
//        public double PhiVanChuyen { get; internal set; }
//        public double TongTien { get; internal set; }
//        public void CalculateTotals(bool v, double giaAd, double v1)
//        {

//            double tongThanhTien = _cartItems.Sum(item => item.ThanhTien);
//            double phiVanChuyen = tongThanhTien > 1000000 ? 0 : 30000;
//            double tongTien = tongThanhTien + phiVanChuyen;

//            ThanhTien = tongThanhTien; 
//            PhiVanChuyen = phiVanChuyen; 
//            TongTien = tongTien; 

//            foreach (var item in _cartItems)
//            {

//                item.TongThanhTien = tongThanhTien;
//                item.PhiVanChuyen = phiVanChuyen;
//                item.TongTien = tongTien;
//            }
//        }


//    }
//}
using Webnc.ViewModels;

public class CartCalculator
{
    private List<CartItem> _cartItems;

    public CartCalculator(List<CartItem> cartItems)
    {
        _cartItems = cartItems;
    }

    public double ThanhTien { get; internal set; }
    public double PhiVanChuyen { get; internal set; }
    public double TongTien { get; internal set; }

    public void CalculateTotals(bool apma, double DonGia)
    {
        double tongThanhTien;
        double Thanhtien = _cartItems.Sum(item => item.ThanhTien);
        if (apma)
        {
            tongThanhTien = Thanhtien - DonGia;
        }
        else
        {
            tongThanhTien = Thanhtien;
        }

        double phiVanChuyen = tongThanhTien > 1000000 ? 0 : 30000;
        double tongTien = tongThanhTien + phiVanChuyen;

        ThanhTien = tongThanhTien;
        PhiVanChuyen = phiVanChuyen;
        TongTien = tongTien;

        foreach (var item in _cartItems)
        {
            item.TongThanhTien = tongThanhTien;
            item.PhiVanChuyen = phiVanChuyen;
            item.TongTien = tongTien;
        }
    }
    //public void CalculateTotals(bool applyDiscount, double discountAmount)
    //{
    //    double totalAmount = _cartItems.Sum(item => item.ThanhTien);

    //    double totalDiscountAmount = applyDiscount ? discountAmount : 0;
    //    double totalAfterDiscount = totalAmount - totalDiscountAmount;

    //    double shippingFee = totalAfterDiscount > 1000000 ? 0 : 30000;
    //    double totalPrice = totalAfterDiscount + shippingFee;

    //    ThanhTien = totalAfterDiscount;
    //    PhiVanChuyen = shippingFee;
    //    TongTien = totalPrice;

    //    foreach (var item in _cartItems)
    //    {
    //        item.TongThanhTien = ThanhTien;
    //        item.PhiVanChuyen = PhiVanChuyen;
    //        item.TongTien = TongTien;
    //    }
    //}

}