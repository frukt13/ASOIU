using Homework3.Variant26.Models;

namespace Homework3.Variant26.Data;

/// <summary>Создаёт базу данных и добавляет начальные данные.</summary>
public static class DbInitializer
{
    /// <summary>Создаёт базу данных и заполняет пустые таблицы.</summary>
    /// <param name="context">Контекст базы данных.</param>
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();
        EnsureSchools(context);
        EnsureStudents(context);
    }

    private static void EnsureSchools(AppDbContext context)
    {
        if (context.Schools.Any())
        {
            return;
        }

        context.Schools.AddRange(
            new School { Name = "Школа № 12" },
            new School { Name = "Гимназия № 4" },
            new School { Name = "Лицей № 7" },
            new School { Name = "Школа № 25" });

        context.SaveChanges();
    }

    private static void EnsureStudents(AppDbContext context)
    {
        if (context.Students.Any())
        {
            return;
        }

        Dictionary<string, int> ids = context.Schools
            .ToDictionary(school => school.Name, school => school.Id);

        context.Students.AddRange(
            new Student { Name = "Иванов Алексей Сергеевич", AverageGrade = 4.72, SchoolId = ids["Лицей № 7"] },
            new Student { Name = "Петрова Мария Андреевна", AverageGrade = 4.91, SchoolId = ids["Гимназия № 4"] },
            new Student { Name = "Смирнов Дмитрий Олегович", AverageGrade = 3.84, SchoolId = ids["Школа № 12"] },
            new Student { Name = "Кузнецова Анна Игоревна", AverageGrade = 4.35, SchoolId = ids["Школа № 25"] },
            new Student { Name = "Соколов Максим Павлович", AverageGrade = 4.58, SchoolId = ids["Лицей № 7"] },
            new Student { Name = "Попова Елена Викторовна", AverageGrade = 4.16, SchoolId = ids["Школа № 12"] },
            new Student { Name = "Лебедев Артём Николаевич", AverageGrade = 3.93, SchoolId = ids["Школа № 25"] },
            new Student { Name = "Новикова Софья Романовна", AverageGrade = 4.67, SchoolId = ids["Гимназия № 4"] },
            new Student { Name = "Морозов Кирилл Денисович", AverageGrade = 4.24, SchoolId = ids["Лицей № 7"] },
            new Student { Name = "Волкова Дарья Максимовна", AverageGrade = 4.48, SchoolId = ids["Школа № 12"] },
            new Student { Name = "Фёдоров Михаил Алексеевич", AverageGrade = 3.76, SchoolId = ids["Школа № 25"] },
            new Student { Name = "Михайлова Полина Евгеньевна", AverageGrade = 4.79, SchoolId = ids["Гимназия № 4"] });

        context.SaveChanges();
    }
}
