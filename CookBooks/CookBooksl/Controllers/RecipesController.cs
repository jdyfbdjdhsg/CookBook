using CookBooks.Data;
using CookBooks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookBooks.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RecipesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Recipes
        public async Task<IActionResult> Index()
        {
            var recipes = await _db.Recipes
                .Include(r => r.Author)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return View(recipes);
        }

        // GET: /Recipes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _db.Recipes
                .Include(r => r.Author)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            var userId = User.Identity?.IsAuthenticated == true
                ? int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0")
                : 0;

            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "User";
            bool isAdmin = userRole == "Admin";

            var viewModel = new RecipeDetailsViewModel
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Ingredients = recipe.Ingredients,
                Instructions = recipe.Instructions,
                CookingTime = recipe.CookingTime,
                Difficulty = recipe.Difficulty,
                Category = recipe.Category,
                ImageUrl = recipe.ImageUrl,
                CreatedAt = recipe.CreatedAt,
                AuthorName = recipe.Author?.UserName ?? "Неизвестный автор",
                AuthorId = recipe.AuthorId,
                IsAuthor = (User.Identity?.IsAuthenticated == true && userId == recipe.AuthorId) || isAdmin  // 👈 Админ может всё!
            };

            return View(viewModel);
        }

        // GET: /Recipes/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Recipes/Create
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(string title, string description, string ingredients, string instructions, int cookingTime, int difficulty, string category, string imageUrl)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(ingredients) || string.IsNullOrEmpty(instructions))
            {
                ViewBag.Error = "Заполните все обязательные поля";
                return View();
            }

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

            var recipe = new Recipe
            {
                Title = title,
                Description = description,
                Ingredients = ingredients,
                Instructions = instructions,
                CookingTime = cookingTime > 0 ? cookingTime : 30,
                Difficulty = (DifficultyLevel)difficulty,
                Category = string.IsNullOrEmpty(category) ? null : category,
                ImageUrl = string.IsNullOrEmpty(imageUrl) ? "/assets/img/rec__item.png" : imageUrl,
                AuthorId = userId,
                CreatedAt = DateTime.Now
            };

            _db.Recipes.Add(recipe);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Рецепт успешно добавлен!";
            return RedirectToAction(nameof(Details), new { id = recipe.Id });
        }

        // GET: /Recipes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var recipe = await _db.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "User";
            bool isAdmin = userRole == "Admin";

            // Админ может редактировать любые рецепты
            if (recipe.AuthorId != userId && !isAdmin)
            {
                return Forbid();
            }

            ViewBag.RecipeId = id;
            ViewBag.SelectedDifficulty = (int)recipe.Difficulty;
            return View(recipe);
        }

        // POST: /Recipes/Edit/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, string title, string description, string ingredients, string instructions, int cookingTime, int difficulty, string category, string imageUrl)
        {
            var recipe = await _db.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "User";
            bool isAdmin = userRole == "Admin";

            // Админ может редактировать любые рецепты
            if (recipe.AuthorId != userId && !isAdmin)
            {
                return Forbid();
            }

            recipe.Title = title;
            recipe.Description = description;
            recipe.Ingredients = ingredients;
            recipe.Instructions = instructions;
            recipe.CookingTime = cookingTime;
            recipe.Difficulty = (DifficultyLevel)difficulty;
            recipe.Category = string.IsNullOrEmpty(category) ? null : category;
            recipe.ImageUrl = string.IsNullOrEmpty(imageUrl) ? recipe.ImageUrl : imageUrl;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Рецепт обновлен!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /Recipes/Delete/5
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var recipe = await _db.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "User";
            bool isAdmin = userRole == "Admin";

            // Админ может удалять любые рецепты
            if (recipe.AuthorId != userId && !isAdmin)
            {
                return Forbid();
            }

            _db.Recipes.Remove(recipe);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Рецепт удален!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Recipes/MyRecipes
        [Authorize]
        public async Task<IActionResult> MyRecipes()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var recipes = await _db.Recipes
                .Include(r => r.Author)
                .Where(r => r.AuthorId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(recipes);
        }

        // GET: /Recipes/Profile
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _db.Users.FindAsync(userId);
            var recipes = await _db.Recipes
                .Include(r => r.Author)
                .Where(r => r.AuthorId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.UserName = user?.UserName;
            ViewBag.IsAdmin = user?.Role == "Admin";
            return View(recipes);
        }
    }
}