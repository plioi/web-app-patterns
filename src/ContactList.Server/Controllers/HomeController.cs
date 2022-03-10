using Microsoft.AspNetCore.Mvc;

namespace ContactList.Server.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "Contacts");

    public IActionResult Error() => View();
}
