using Microsoft.AspNetCore.Mvc;
using EduManage.Models;
using EduManage.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using System.Text;

namespace EduManage.Controllers
{
public class AdminController : Controller
{
private readonly ApplicationDbContext _context;


    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("Role") == "Admin";
    }

    private static string GenerateUniquePassword(ApplicationDbContext context)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
        var random = Random.Shared;

        for (var attempt = 0; attempt < 20; attempt++)
        {
            var builder = new StringBuilder("Pwd@");
            for (var i = 0; i < 8; i++)
                builder.Append(chars[random.Next(chars.Length)]);

            var password = builder.ToString();
            if (!context.Students.Any(s => s.Password == password) &&
                !context.Teachers.Any(t => t.Password == password))
                return password;
        }

        return "Pwd@" + Guid.NewGuid().ToString("N")[..8];
    }

    private static string SanitizeUsernamePart(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "user";

        var cleaned = new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLower();
        return string.IsNullOrEmpty(cleaned) ? "user" : cleaned;
    }

    // DASHBOARD
    public IActionResult Index()
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        var stats = new DashboardStats
        {
            TotalStudents = _context.Students.Count(),
            TotalTeachers = _context.Teachers.Count(),
            PendingLeaves = _context.LeaveRequests.Count(l => l.Status == "Pending"),
            TotalEvents = _context.Events.Count()
        };

        var classes = _context.Students.Select(s => s.Class).Distinct().Where(c => !string.IsNullOrEmpty(c)).ToList();
        foreach (var c in classes)
        {
            var studentCount = _context.Students.Count(s => s.Class == c);
            var studentIds = _context.Students.Where(s => s.Class == c).Select(s => s.Id).ToList();
            var attendances = _context.Attendances.Where(a => studentIds.Contains(a.StudentId)).ToList();
            var total = attendances.Count;
            var present = attendances.Count(a => a.Status == "Present" || a.Status == "Leave");
            var pct = total > 0 ? (int)Math.Round((double)present / total * 100) : 100;

            stats.ClassStats.Add(new ClassStat
            {
                ClassName = c,
                StudentCount = studentCount,
                AttendanceRate = pct
            });
        }

        ViewBag.Stats = stats;
        ViewBag.RecentAnnouncements = _context.Announcements.OrderByDescending(a => a.CreatedAt).Take(4).ToList();
        ViewBag.UpcomingEvents = _context.Events.Where(e => e.EndDate >= DateTime.Now).OrderBy(e => e.StartDate).Take(4).ToList();

        // Retrieve view-powered metrics using SQL views (joins & subqueries)
        ViewBag.AssignmentProgress = _context.AssignmentProgresses.ToList();
        ViewBag.StudentOverviews = _context.StudentDashboardOverviews.ToList();

        return View();
    }

    // ADD STUDENT
    public IActionResult AddStudent()
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        ViewBag.AllStudents = _context.Students.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddStudent(Student model)
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        if (model.DateOfBirth.HasValue && model.DateOfBirth.Value > DateTime.Today)
        {
            ModelState.AddModelError("DateOfBirth", "Date of Birth cannot be in the future.");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.AllStudents = _context.Students.ToList();
            return View(model);
        }

        if (string.IsNullOrEmpty(model.RollNumber))
        {
            var lastStudent = _context.Students
                .Where(s => s.RollNumber != null && s.RollNumber.StartsWith("STU"))
                .OrderByDescending(s => s.RollNumber)
                .FirstOrDefault();

            int nextNum = 1;
            if (lastStudent != null && lastStudent.RollNumber!.Length >= 6)
            {
                var numericPart = lastStudent.RollNumber.Substring(3);
                if (int.TryParse(numericPart, out int parsedNum))
                {
                    nextNum = parsedNum + 1;
                }
            }
            model.RollNumber = $"STU{nextNum:D3}";
        }

        // Trim inputs
        model.Username = model.Username?.Trim();
        model.Password = model.Password?.Trim();

        // If admin provided username/password use them; otherwise auto-generate and show to admin
        var usernameWasGenerated = false;
        var passwordWasGenerated = false;

        if (string.IsNullOrEmpty(model.Username))
        {
            var baseName = SanitizeUsernamePart((model.FullName ?? "user").Split(' ')[0]);
            var suffix = model.RollNumber.StartsWith("STU") ? model.RollNumber.Substring(3) : DateTime.Now.ToString("HHmmssfff");
            var generated = $"{baseName}{suffix}";

            // Ensure uniqueness
            int counter = 1;
            while (_context.Students.Any(s => s.Username == generated) || _context.Teachers.Any(t => t.Username == generated))
            {
                generated = $"{baseName}{suffix}_{counter}";
                counter++;
            }
            model.Username = generated;
            usernameWasGenerated = true;
        }
        else
        {
            if (_context.Students.Any(s => s.Username == model.Username) || _context.Teachers.Any(t => t.Username == model.Username))
            {
                ModelState.AddModelError("Username", "This username is already taken.");
                ViewBag.AllStudents = _context.Students.ToList();
                return View(model);
            }
        }

        if (string.IsNullOrEmpty(model.Password))
        {
            var numericPart = model.RollNumber.StartsWith("STU") ? model.RollNumber.Substring(3) : "123";
            model.Password = $"pwd@student{numericPart}";
            passwordWasGenerated = true;
        }

        _context.Students.Add(model);
        _context.SaveChanges();

        ViewBag.Success = "Student Added Successfully";
        ViewBag.SavedUsername = model.Username;
        ViewBag.SavedPassword = model.Password;
        ViewBag.UsernameWasGenerated = usernameWasGenerated;
        ViewBag.PasswordWasGenerated = passwordWasGenerated;
        ViewBag.AllStudents = _context.Students.ToList();

        return View(new Student());
    }

    // DELETE STUDENT
    public IActionResult DeleteStudent()
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        return View(_context.Students.ToList());
    }

    [HttpPost]
    public IActionResult DeleteStudentConfirm(int id)
    {
        var student = _context.Students.Find(id);

        if (student != null)
        {
            _context.Students.Remove(student);
            _context.SaveChanges();
        }

        return RedirectToAction("DeleteStudent");
    }

    // CHANGE DETAILS
    public IActionResult ChangeDetails(string rollNo)
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        ViewBag.RollNo = rollNo;

        if (!string.IsNullOrEmpty(rollNo))
        {
            var student = _context.Students
                .FirstOrDefault(x => x.RollNumber == rollNo);

            if (student == null)
                ViewBag.NotFound = true;

            return View(student);
        }

        return View(null as Student);
    }

    [HttpPost]
    public IActionResult UpdateStudent(Student model)
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        if (model.DateOfBirth.HasValue && model.DateOfBirth.Value > DateTime.Today)
        {
            ViewBag.Success = "Error: Date of Birth cannot be in the future.";
            var current = _context.Students.FirstOrDefault(x => x.Id == model.Id);
            return View("ChangeDetails", current);
        }

        var existing = _context.Students
            .FirstOrDefault(x => x.Id == model.Id);

        if (existing != null)
        {
            existing.FullName = model.FullName;
            existing.FatherName = model.FatherName;
            existing.DateOfBirth = model.DateOfBirth;
            existing.Gender = model.Gender;
            existing.Class = model.Class;
            existing.Section = model.Section;
            existing.Contact = model.Contact;
            existing.Address = model.Address;
            existing.Password = model.Password;

            _context.SaveChanges();
        }

        ViewBag.Success = "Details Successfully Updated";

        return View("ChangeDetails", existing);
    }

    // ADD TEACHER
    public IActionResult AddTeacher()
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        ViewBag.Subjects = _context.Subjects.OrderBy(s => s.Name).ToList();
        ViewBag.AllTeachers = _context.Teachers.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddTeacher(Teacher model)
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
        {
            ViewBag.Subjects = _context.Subjects.OrderBy(s => s.Name).ToList();
            ViewBag.AllTeachers = _context.Teachers.ToList();
            return View(model);
        }

        model.Role ??= "Teacher";
        model.Username ??= string.Empty;
        model.Password ??= string.Empty;

        _context.Teachers.Add(model);
        _context.SaveChanges();

        ViewBag.Success = "Teacher Added Successfully";
        ViewBag.Subjects = _context.Subjects.OrderBy(s => s.Name).ToList();
        ViewBag.AllTeachers = _context.Teachers.ToList();

        return View();
    }

    // VIEW ALL STUDENTS
    public IActionResult StudentList()
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        var students = _context.Students.ToList();

        return View(students);
    }

    // CHANGE TEACHER DETAILS
    public IActionResult ChangeTeacher(string username)
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        ViewBag.Username = username;
        ViewBag.Subjects = _context.Subjects.OrderBy(s => s.Name).ToList();

        if (!string.IsNullOrEmpty(username))
        {
            var teacher = _context.Teachers.FirstOrDefault(x => x.Username == username);

            if (teacher == null)
                ViewBag.NotFound = true;

            return View("ChangeTeacher", teacher);
        }

        return View("ChangeTeacher", null as Teacher);
    }

    [HttpPost]
    public IActionResult UpdateTeacher(Teacher model)
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        var existing = _context.Teachers.FirstOrDefault(x => x.Id == model.Id);

        if (existing != null)
        {
            existing.FullName = model.FullName;
            existing.Subject = model.Subject;
            existing.AssignedClass = model.AssignedClass;
            existing.AssignedSection = model.AssignedSection;
            existing.Contact = model.Contact;
            existing.Username = model.Username;
            existing.Password = model.Password;

            _context.SaveChanges();
        }

        ViewBag.Success = "Details Successfully Updated";
        ViewBag.Subjects = _context.Subjects.OrderBy(s => s.Name).ToList();

        return View("ChangeTeacher", existing);
    }

    // VIEW ALL TEACHERS
    public IActionResult TeacherList()
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        var teachers = _context.Teachers.ToList();

        return View(teachers);
    }

    // DELETE TEACHER
    public IActionResult DeleteTeacher()
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        return View(_context.Teachers.ToList());
    }

    [HttpPost]
    public IActionResult DeleteTeacher(int id)
    {
        var teacher = _context.Teachers.Find(id);

        if (teacher != null)
        {
            _context.Teachers.Remove(teacher);
            _context.SaveChanges();
        }

        return RedirectToAction("DeleteTeacher");
    }

    // CHANGE PASSWORD (ADMIN ONLY FOR THEMSELVES)
    public IActionResult ChangePassword()
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (!IsAdmin())
            return RedirectToAction("Index", "Home");

        var adminId = HttpContext.Session.GetInt32("AdminId") ?? 0;
        var admin = _context.Admins.Find(adminId);

        if (admin == null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (admin.Password != currentPassword)
        {
            ViewBag.Error = "Current password is incorrect.";
            return View();
        }

        if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 4)
        {
            ViewBag.Error = "New password must be at least 4 characters long.";
            return View();
        }

        if (newPassword != confirmPassword)
        {
            ViewBag.Error = "New password and confirmation do not match.";
            return View();
        }

        admin.Password = newPassword;
        _context.SaveChanges();

        ViewBag.Success = "Password updated successfully!";
        return View();
    }
}
}
