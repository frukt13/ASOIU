using Homework3.Variant26.Data;
using Homework3.Variant26.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Homework3.Variant26.Controllers;

/// <summary>Контроллер CRUD-операций с учениками.</summary>
public sealed class StudentsController : Controller
{
    /// <summary>Отображает список учеников с названиями школ.</summary>
    /// <returns>Страница со списком учеников.</returns>
    public IActionResult Index()
    {
        using var context = new AppDbContext();
        List<Student> students = context.Students
            .Include(student => student.School)
            .AsNoTracking()
            .OrderBy(student => student.Name)
            .ToList();

        return View(students);
    }

    /// <summary>Отображает форму добавления ученика.</summary>
    /// <returns>Форма добавления.</returns>
    public IActionResult Create()
    {
        using var context = new AppDbContext();
        ViewBag.Schools = BuildSchoolOptions(context);
        return View(new Student());
    }

    /// <summary>Создаёт ученика после проверки введённых данных.</summary>
    /// <param name="student">Новый ученик.</param>
    /// <returns>Переход к списку либо форма с сообщением об ошибке.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Name,AverageGrade,SchoolId")] Student student)
    {
        student.Name = student.Name?.Trim() ?? string.Empty;

        using var context = new AppDbContext();
        ValidateSchool(context, student.SchoolId);
        if (!ModelState.IsValid)
        {
            ViewBag.Schools = BuildSchoolOptions(context, student.SchoolId);
            return View(student);
        }

        context.Students.Add(student);
        context.SaveChanges();
        TempData["SuccessMessage"] = "Ученик добавлен.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Отображает форму редактирования ученика.</summary>
    /// <param name="id">Идентификатор ученика.</param>
    /// <returns>Форма редактирования либо ответ 404.</returns>
    public IActionResult Edit(int id)
    {
        using var context = new AppDbContext();
        Student? student = context.Students.Find(id);
        if (student is null)
        {
            return NotFound();
        }

        ViewBag.Schools = BuildSchoolOptions(context, student.SchoolId);
        return View(student);
    }

    /// <summary>Сохраняет изменения ученика.</summary>
    /// <param name="id">Идентификатор ученика.</param>
    /// <param name="student">Изменённый ученик.</param>
    /// <returns>Переход к списку либо форма с сообщением об ошибке.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("Id,Name,AverageGrade,SchoolId")] Student student)
    {
        if (id != student.Id)
        {
            return BadRequest();
        }

        student.Name = student.Name?.Trim() ?? string.Empty;

        using var context = new AppDbContext();
        ValidateSchool(context, student.SchoolId);
        if (!ModelState.IsValid)
        {
            ViewBag.Schools = BuildSchoolOptions(context, student.SchoolId);
            return View(student);
        }

        context.Students.Update(student);
        context.SaveChanges();
        TempData["SuccessMessage"] = "Изменения сохранены.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Отображает страницу подтверждения удаления ученика.</summary>
    /// <param name="id">Идентификатор ученика.</param>
    /// <returns>Страница подтверждения либо ответ 404.</returns>
    public IActionResult Delete(int id)
    {
        using var context = new AppDbContext();
        Student? student = context.Students
            .Include(item => item.School)
            .AsNoTracking()
            .FirstOrDefault(item => item.Id == id);

        return student is null ? NotFound() : View(student);
    }

    /// <summary>Удаляет ученика.</summary>
    /// <param name="id">Идентификатор ученика.</param>
    /// <returns>Переход к списку учеников.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        using var context = new AppDbContext();
        Student? student = context.Students.Find(id);
        if (student is null)
        {
            return NotFound();
        }

        context.Students.Remove(student);
        context.SaveChanges();
        TempData["SuccessMessage"] = "Ученик удалён.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidateSchool(AppDbContext context, int schoolId)
    {
        if (!context.Schools.Any(school => school.Id == schoolId))
        {
            ModelState.AddModelError(nameof(Student.SchoolId), "Выберите существующую школу.");
        }
    }

    private static List<SelectListItem> BuildSchoolOptions(AppDbContext context, int? selectedSchoolId = null)
    {
        return context.Schools
            .AsNoTracking()
            .OrderBy(school => school.Name)
            .Select(school => new SelectListItem
            {
                Value = school.Id.ToString(),
                Text = school.Name,
                Selected = selectedSchoolId.HasValue && school.Id == selectedSchoolId.Value
            })
            .ToList();
    }
}
