using System.ComponentModel.DataAnnotations;

namespace Homework3.Variant26.Models;

/// <summary>Ученик (основная таблица, сторона «много»).</summary>
public sealed class Student
{
    /// <summary>Идентификатор ученика (первичный ключ).</summary>
    public int Id { get; set; }

    /// <summary>Идентификатор школы (внешний ключ).</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Выберите школу.")]
    [Display(Name = "Школа")]
    public int SchoolId { get; set; }

    /// <summary>Навигационное свойство: школа ученика.</summary>
    public School? School { get; set; }

    /// <summary>ФИО ученика.</summary>
    [Required(ErrorMessage = "Введите ФИО ученика.")]
    [StringLength(180, MinimumLength = 3, ErrorMessage = "ФИО должно содержать от 3 до 180 символов.")]
    [Display(Name = "ФИО ученика")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Средний балл успеваемости ученика.</summary>
    [Range(0, double.MaxValue, ErrorMessage = "Средний балл не может быть отрицательным.")]
    [Display(Name = "Средний балл")]
    public double AverageGrade { get; set; }
}
