using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Webnc.Models;


namespace Webnc.Pages.ThuongHieu
{
    public class DetailModel : PageModel
    {
        private readonly WebncContext context;

        public DetailModel(WebncContext ctx)
        {
            context = ctx;
        }

        public Webnc.Models.ThuongHieu? ThuongHieu { get; set; }

        // Phýõng th?c này ðý?c g?i khi có yêu c?u HTTP GET ðý?c g?i t?i Razor Page. 

        public async Task OnGetAsync(int id)
        {
            ThuongHieu = await context.ThuongHieus.FindAsync(id);
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Ki?m tra giá tr? id 
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            var thuongHieu = await context.ThuongHieus.FindAsync(id);

            if (thuongHieu == null)
            {
                return NotFound();
            }
            return RedirectToPage("./Index");

        }
    }
}
