using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    // GET: Home
    public ActionResult Index()
    {
        // You can also pass dynamic data to the view (e.g., user info or messages).
        ViewBag.Message = "Welcome to the Leave Management System!";
        return View();
    }
}
