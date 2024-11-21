using Microsoft.AspNetCore.Mvc;

namespace Notes.Areas.Web.Controllers
{
    // Контроллер для представлений
    [Area("Web")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
