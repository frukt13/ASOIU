using System.ComponentModel.DataAnnotations;

namespace Homework3.Variant26.Models;

/// <summary>Школа (справочная таблица, сторона «один»).</summary>
public sealed class School
{
    /// <summary>Идентификатор школы (первичный ключ).</summary>
    public int Id { get; set; }

    /// <summary>Название школы.</summary>
    [Required(ErrorMessage = "Введите название школы.")]
    [StringLength(160, MinimumLength = 2, ErrorMessage = "Название школы должно содержать от 2 до 160 символов.")]
    [Display(Name = "Название школы")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Навигационное свойство: ученики этой школы.</summary>
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
