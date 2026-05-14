using System.ComponentModel.DataAnnotations;

namespace CookBooks.Models
{
    public class CreateRecipeViewModel
    {
        [Required(ErrorMessage = "Введите название рецепта")]
        [StringLength(100, MinimumLength = 3)]
        [Display(Name = "Название рецепта")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите описание")]
        [StringLength(500)]
        [Display(Name = "Краткое описание")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите ингредиенты")]
        [Display(Name = "Ингредиенты (каждый с новой строки)")]
        public string Ingredients { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите инструкцию")]
        [Display(Name = "Инструкция (каждый шаг с новой строки)")]
        public string Instructions { get; set; } = string.Empty;

        [Display(Name = "Время приготовления (минуты)")]
        [Range(1, 480)]
        public int CookingTime { get; set; } = 30;

        [Display(Name = "Сложность")]
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;

        [Display(Name = "Категория")]
        public string? Category { get; set; }

        [Display(Name = "URL изображения (опционально)")]
        public string? ImageUrl { get; set; }
    }

    public class RecipeDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public int CookingTime { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public bool IsAuthor { get; set; }
    }
}