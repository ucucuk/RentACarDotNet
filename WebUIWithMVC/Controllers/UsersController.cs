using Microsoft.AspNetCore.Mvc;

namespace WebUIWithMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly HttpClient _httpClient;
        private string url = "";
        public IActionResult Index()
        {
            return View();
        }
    }
}
