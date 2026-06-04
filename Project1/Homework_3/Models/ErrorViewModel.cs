namespace Homework3.Variant26.Models;

/// <summary>Модель страницы ошибки.</summary>
public sealed class ErrorViewModel
{
    /// <summary>Идентификатор запроса.</summary>
    public string? RequestId { get; set; }

    /// <summary>Определяет, нужно ли показывать идентификатор запроса.</summary>
    public bool ShowRequestId => !string.IsNullOrWhiteSpace(RequestId);
}
