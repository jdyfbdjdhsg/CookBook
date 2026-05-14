using CookBooks.Data;
using CookBooks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookBooks.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Получаем 6 последних рецептов
            var latestRecipes = await _db.Recipes
                .Include(r => r.Author)
                .OrderByDescending(r => r.CreatedAt)
                .Take(6)
                .ToListAsync();

            return View(latestRecipes);
        }
    }
}