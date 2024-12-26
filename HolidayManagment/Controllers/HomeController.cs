using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public ActionResult Index()
    {
        ViewBag.Message = "Welcome to the Leave Management System!";
        return View();
    }
}
