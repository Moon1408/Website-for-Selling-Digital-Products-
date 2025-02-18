using Webnc.ViewModels;


namespace Webnc.Services
{
    public interface IVnPayService
    {
        // buil đường dẫn 
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
