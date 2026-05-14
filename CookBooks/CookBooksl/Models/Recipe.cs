using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookBooks.Models
{
    [Table("Recipes")]
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название рецепта")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Название от 3 до 100 символов")]
        [Display(Name = "Название рецепта")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите описание")]
        [StringLength(500, ErrorMessage = "Описание не более 500 символов")]
        [Display(Name = "Краткое описание")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите список ингредиентов")]
        [Display(Name = "Ингредиенты")]
        public string Ingredients { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите инструкцию приготовления")]
        [Display(Name = "Инструкция")]
        public string Instructions { get; set; } = string.Empty;

        [Display(Name = "Время приготовления (минуты)")]
        [Range(1, 480, ErrorMessage = "Время от 1 до 480 минут")]
        public int CookingTime { get; set; } = 30;

        [Display(Name = "Сложность")]
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Medium;

        [Display(Name = "Категория")]
        public string? Category { get; set; }

        [Display(Name = "URL изображения")]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Внешний ключ - автор рецепта
        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public virtual User? Author { get; set; }
    }

    public enum DifficultyLevel
    {
        [Display(Name = "Легко")]
        Easy,
        [Display(Name = "Средне")]
        Medium,
        [Display(Name = "Сложно")]
        Hard
    }
}