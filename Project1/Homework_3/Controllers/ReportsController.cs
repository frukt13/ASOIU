using Homework3.Variant26.Data;
using Homework3.Variant26.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homework3.Variant26.Controllers;

/// <summary>Контроллер LINQ-отчёта.</summary>
public sealed class ReportsController : Controller
{
    /// <summary>Формирует и отображает три раздела отчёта.</summary>
    /// <returns>Страница отчёта.</returns>
    public IActionResult Index()
    {
        using var context = new AppDbContext();

        List<StudentReportRow> students = context.Students
            .Include(student => student.School)
            .AsNoTracking()
            .OrderBy(student => student.Name)
            .Select(student => new StudentReportRow
            {
                StudentName = student.Name,
                SchoolName = student.School!.Name,
                AverageGrade = student.AverageGrade
            })
            .ToList();

        List<SchoolCountReportRow> schoolCounts = context.Students
            .GroupBy(student => student.School!.Name)
            .Select(group => new SchoolCountReportRow
            {
                SchoolName = group.Key,
                StudentCount = group.Count()
            })
            .OrderBy(row => row.SchoolName)
            .ToList();

        List<SchoolAverageReportRow> schoolAverages = context.Students
            .GroupBy(student => student.School!.Name)
            .Select(group => new SchoolAverageReportRow
            {
                SchoolName = group.Key,
                AverageGrade = group.Average(student => student.AverageGrade)
            })
            .OrderByDescending(row => row.AverageGrade)
            .ToList();

        return View(new ReportsViewModel
        {
            Students = students,
            SchoolCounts = schoolCounts,
            SchoolAverages = schoolAverages
        });
    }
}
