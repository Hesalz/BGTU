using DAL_Celebrity_MSSQL.Interfaces;
using DAL_Celebrity_MSSQL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Lab7.Pages
{
    public class CelebrityModel : PageModel
    {
        private readonly IRepository _repo;

        public CelebrityModel(IRepository repo)
        {
            _repo = repo;
        }

        public Celebrity Celebrity { get; set; }

        public IActionResult OnGet(int id)
        {
            Celebrity = _repo.GetCelebrityById(id);
            if (Celebrity == null)
                return NotFound();

            return Page();
        }
    }
}
