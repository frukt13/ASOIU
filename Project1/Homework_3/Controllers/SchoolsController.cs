using Homework3.Variant26.Data;
using Homework3.Variant26.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homework3.Variant26.Controllers;

/// <summary>Контроллер CRUD-операций со справочником школ.</summary>
public sealed class SchoolsController : Controller
{
    /// <summary>Отображает список школ.</summary>
    /// <returns>Страница со списком школ.</returns>
    public IActionResult Index()
    {
        using var context = new AppDbContext();
        List<School> schools = context.Schools
            .AsNoTracking()
            .OrderBy(school => school.Name)
            .ToList();

        return View(schools);
    }

    /// <summary>Отображает форму создания школы.</summary>
    /// <returns>Форма создания.</returns>
    public IActionResult Create() => View(new School());

    /// <summary>Создаёт школу после проверки введённых данных.</summary>
    /// <param name="school">Новая школа.</param>
    /// <returns>Переход к списку либо форма с сообщением об ошибке.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Name")] School school)
    {
        school.Name = school.Name?.Trim() ?? string.Empty;

        if (!ModelState.IsValid)
        {
            return View(school);
        }

        using var context = new AppDbContext();
        if (context.Schools.Any(item => item.Name == school.Name))
        {
            ModelState.AddModelError(nameof(School.Name), "Школа с таким названием уже существует.");
            return View(school);
        }

        context.Schools.Add(school);
        context.SaveChanges();
        TempData["SuccessMessage"] = "Школа добавлена.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Отображает форму редактирования школы.</summary>
    /// <param name="id">Идентификатор школы.</param>
    /// <returns>Форма редактирования либо ответ 404.</returns>
    public IActionResult Edit(int id)
    {
        using var context = new AppDbContext();
        School? school = context.Schools.Find(id);
        return school is null ? NotFound() : View(school);
    }

    /// <summary>Сохраняет изменения школы.</summary>
    /// <param name="id">Идентификатор школы.</param>
    /// <param name="school">Изменённая школа.</param>
    /// <returns>Переход к списку либо форма с сообщением об ошибке.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,Name")] School school)
    {
        if (id != school.Id)
        {
            return BadRequest();
        }

        school.Name = school.Name?.Trim() ?? string.Empty;
        if (!ModelState.IsValid)
        {
            return View(school);
        }

        using var context = new AppDbContext();
        if (context.Schools.Any(item => item.Name == school.Name && item.Id != school.Id))
        {
            ModelState.AddModelError(nameof(School.Name), "Школа с таким названием уже существует.");
            return View(school);
        }

        context.Schools.Update(school);
        context.SaveChanges();
        TempData["SuccessMessage"] = "Изменения сохранены.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Отображает страницу подтверждения удаления школы.</summary>
    /// <param name="id">Идентификатор школы.</param>
    /// <returns>Страница подтверждения либо ответ 404.</returns>
    public IActionResult Delete(int id)
    {
        using var context = new AppDbContext();
        School? school = context.Schools
            .AsNoTracking()
            .FirstOrDefault(item => item.Id == id);

        if (school is null)
        {
            return NotFound();
        }

        ViewBag.HasStudents = context.Students.Any(student => student.SchoolId == id);
        return View(school);
    }

    /// <summary>Удаляет школу, если с ней не связаны ученики.</summary>
    /// <param name="id">Идентификатор школы.</param>
    /// <returns>Переход к списку школ.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        using var context = new AppDbContext();
        School? school = context.Schools.Find(id);
        if (school is null)
        {
            return NotFound();
        }

        if (context.Students.Any(student => student.SchoolId == id))
        {
            TempData["ErrorMessage"] = "Удаление запрещено: в школе есть связанные ученики.";
            return RedirectToAction(nameof(Index));
        }

        context.Schools.Remove(school);
        context.SaveChanges();
        TempData["SuccessMessage"] = "Школа удалена.";
        return RedirectToAction(nameof(Index));
    }
}
