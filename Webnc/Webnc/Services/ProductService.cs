using Microsoft.EntityFrameworkCore;
using Webnc.Models;

namespace Webnc.Services
{
    public class ProductService
    {
        private readonly WebncContext db;

        public ProductService(WebncContext context)
        {
            db = context;
        }

        public async Task<bool> IsStockAvailableAsync(int productId, int quantity)
        {
            var hangHoa = await db.HangHoas.FindAsync(productId);
            return hangHoa != null && hangHoa.Sl >= quantity;
        }
    }
}
