using CookBooks.Models;
using CookBooks.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CookBooks.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            await context.Database.EnsureCreatedAsync();

            // Создаем тестового пользователя-админа, если нет ни одного
            if (!context.Users.Any())
            {
                var adminUser = new User
                {
                    UserName = "Шеф-повар",
                    Email = "chef@cookbooks.com",
                    PasswordHash = PasswordHelper.Hash("123456"),
                    CreatedAt = DateTime.Now,
                    Role = "Admin"  // 👈 Админ!
                };
                context.Users.Add(adminUser);
                await context.SaveChangesAsync();

                // Добавляем тестовые рецепты
                if (!context.Recipes.Any())
                {
                    var recipes = new[]
                    {
                        new Recipe
                        {
                            Title = "Черничные вафли",
                            Description = "Побалуйте себя пышными золотистыми вафлями с начинкой из сочной черники",
                            Ingredients = "1 стакан муки\n2 столовые ложки сахара\n1 чайная ложка разрыхлителя\n1/2 чайной ложки соды\n1/4 чайной ложки соли\n3/4 стакана пахты\n1/4 стакана молока\n2 столовые ложки масла\n1 яйцо\n1 чайная ложка ванилина\n1 чашка черники",
                            Instructions = "1. В большой миске смешайте муку, сахар, разрыхлитель, соду и соль.\n2. В отдельной миске взбейте пахту, молоко, масло, яйцо и ванилин.\n3. Соедините влажные и сухие ингредиенты.\n4. Аккуратно добавьте чернику.\n5. Жарьте на разогретой вафельнице до золотистого цвета.\n6. Подавайте с кленовым сиропом.",
                            CookingTime = 20,
                            Difficulty = DifficultyLevel.Easy,
                            Category = "Завтрак",
                            ImageUrl = "/assets/img/rec__item.png",
                            AuthorId = adminUser.Id,
                            CreatedAt = DateTime.Now.AddDays(-5)
                        },
                        new Recipe
                        {
                            Title = "Салат Капрезе",
                            Description = "Классический итальянский салат из свежих томатов, моцареллы и базилика",
                            Ingredients = "Помидоры - 4 шт\nМоцарелла - 200г\nБазилик свежий - 1 пучок\nОливковое масло - 3 ст.л.\nБальзамический уксус - 1 ст.л.\nСоль и перец по вкусу",
                            Instructions = "1. Нарежьте помидоры и моцареллу кружочками.\n2. Выложите на тарелку, чередуя помидоры и моцареллу.\n3. Добавьте листья базилика.\n4. Полейте оливковым маслом и уксусом.\n5. Посолите и поперчите по вкусу.",
                            CookingTime = 15,
                            Difficulty = DifficultyLevel.Easy,
                            Category = "Салаты",
                            ImageUrl = "/assets/img/check_card.png",
                            AuthorId = adminUser.Id,
                            CreatedAt = DateTime.Now.AddDays(-3)
                        },
                        new Recipe
                        {
                            Title = "Томатный суп",
                            Description = "Ароматный и полезный томатный суп с базиликом",
                            Ingredients = "Помидоры - 1 кг\nЛук - 1 шт\nЧеснок - 2 зубчика\nОливковое масло - 2 ст.л.\nТоматная паста - 1 ст.л.\nОвощной бульон - 500 мл\nБазилик - по вкусу",
                            Instructions = "1. Обжарьте лук и чеснок на масле.\n2. Добавьте помидоры и томатную пасту.\n3. Влейте бульон и варите 20 минут.\n4. Измельчите блендером.\n5. Подавайте с базиликом.",
                            CookingTime = 40,
                            Difficulty = DifficultyLevel.Medium,
                            Category = "Супы",
                            ImageUrl = "/assets/img/check_card.png",
                            AuthorId = adminUser.Id,
                            CreatedAt = DateTime.Now.AddDays(-1)
                        }
                    };

                    context.Recipes.AddRange(recipes);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}