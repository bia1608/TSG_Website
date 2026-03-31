using Microsoft.AspNetCore.Mvc;

namespace TSG_Website.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
