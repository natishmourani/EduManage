using EduManage.Data;
using EduManage.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduManage.Controllers
{
public class HomeController : Controller
{
private readonly ApplicationDbContext _context;


    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        var username = model.Username?.Trim();
        var password = model.Password?.Trim();

        if (model.Role == "Admin")
        {
            var admin = _context.Admins
                .FirstOrDefault(x =>
                    x.Username == username &&
                    x.Password == password);

            if (admin != null)
            {
                HttpContext.Session.SetString("Role", "Admin");
                HttpContext.Session.SetInt32("AdminId", admin.Id);
                HttpContext.Session.SetString("AdminName", admin.FullName ?? "Super Admin");
                HttpContext.Session.SetString("AdminRole", admin.Role ?? "Full access");

                return RedirectToAction("Index", "Admin");
            }
        }

        if (model.Role == "Teacher")
        {
            var teacher = _context.Teachers
                .FirstOrDefault(x =>
                    x.Username == username &&
                    x.Password == password);

            if (teacher != null)
            {
                HttpContext.Session.SetString("Role", "Teacher");
                HttpContext.Session.SetInt32("TeacherId", teacher.Id);
                HttpContext.Session.SetString("TeacherName", teacher.FullName ?? "");
                HttpContext.Session.SetString("TeacherSubject", teacher.Subject ?? "");
                HttpContext.Session.SetString("TeacherClass", teacher.AssignedClass ?? "");
                HttpContext.Session.SetString("TeacherSection", teacher.AssignedSection ?? "");

                return RedirectToAction("Index", "Teacher");
            }
        }

        if (model.Role == "Student")
        {
            var student = _context.Students
                .FirstOrDefault(x =>
                    x.Username == username &&
                    x.Password == password);

            if (student != null)
            {
                HttpContext.Session.SetString("Role", "Student");
                HttpContext.Session.SetInt32("StudentId", student.Id);
                HttpContext.Session.SetString("StudentName", student.FullName ?? "");

                return RedirectToAction("Index", "Student");
            }
        }

        ViewBag.Error = "Invalid Login";
        return View("Index");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}

}
