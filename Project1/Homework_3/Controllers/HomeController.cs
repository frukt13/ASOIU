using System.Diagnostics;
using Homework3.Variant26.Models;
using Microsoft.AspNetCore.Mvc;

namespace Homework3.Variant26.Controllers;

/// <summary>Контроллер главной страницы и обработки ошибок.</summary>
public sealed class HomeController : Controller
{
    /// <summary>Отображает главную страницу приложения.</summary>
    /// <returns>Главная страница.</returns>
    public IActionResult Index() => View();

    /// <summary>Отображает информацию об ошибке.</summary>
    /// <returns>Страница ошибки.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
