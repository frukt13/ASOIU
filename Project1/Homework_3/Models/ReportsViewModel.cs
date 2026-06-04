namespace Homework3.Variant26.Models;

/// <summary>Модель страницы отчёта.</summary>
public sealed class ReportsViewModel
{
    /// <summary>Полный список учеников.</summary>
    public IReadOnlyList<StudentReportRow> Students { get; init; } = Array.Empty<StudentReportRow>();

    /// <summary>Количество учеников по школам.</summary>
    public IReadOnlyList<SchoolCountReportRow> SchoolCounts { get; init; } = Array.Empty<SchoolCountReportRow>();

    /// <summary>Средние баллы по школам.</summary>
    public IReadOnlyList<SchoolAverageReportRow> SchoolAverages { get; init; } = Array.Empty<SchoolAverageReportRow>();
}

/// <summary>Строка полного списка учеников.</summary>
public sealed class StudentReportRow
{
    /// <summary>ФИО ученика.</summary>
    public string StudentName { get; init; } = string.Empty;

    /// <summary>Название школы.</summary>
    public string SchoolName { get; init; } = string.Empty;

    /// <summary>Средний балл ученика.</summary>
    public double AverageGrade { get; init; }
}

/// <summary>Строка отчёта о количестве учеников по школе.</summary>
public sealed class SchoolCountReportRow
{
    /// <summary>Название школы.</summary>
    public string SchoolName { get; init; } = string.Empty;

    /// <summary>Количество учеников.</summary>
    public int StudentCount { get; init; }
}

/// <summary>Строка отчёта о среднем балле учеников по школе.</summary>
public sealed class SchoolAverageReportRow
{
    /// <summary>Название школы.</summary>
    public string SchoolName { get; init; } = string.Empty;

    /// <summary>Средний балл учеников школы.</summary>
    public double AverageGrade { get; init; }
}
