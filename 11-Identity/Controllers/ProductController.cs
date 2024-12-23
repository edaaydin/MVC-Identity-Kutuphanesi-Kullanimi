using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _11_Identity.Controllers
{
    [Authorize(Roles = "Mudur,ceo")] // yetki ile calısması icin // rolu mudur olan erissin // acces denied? rol bilgisi uyusmuyor.
    //[Authorize(Roles = "Mudur")]
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
