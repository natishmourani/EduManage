using Microsoft.AspNetCore.Mvc;
using EduManage.Models;
using EduManage.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace EduManage.Controllers
{
public class TeacherController : Controller
{
private readonly ApplicationDbContext _context;


    public TeacherController(ApplicationDbContext context)
    {
        _context = context;
    }

    private bool IsTeacher()
    {
        return HttpContext.Session.GetString("Role") == "Teacher";
    }

    // DASHBOARD
    public IActionResult Index()
    {
        if (!IsTeacher())
            return RedirectToAction("Index", "Home");

        var teacherId = HttpContext.Session.GetInt32("TeacherId") ?? 0;
        var teacher = _context.Teachers.Find(teacherId);
        var className = teacher?.AssignedClass;
        var sectionName = teacher?.AssignedSection;

        ViewBag.Teacher = teacher;

        if (!string.IsNullOrEmpty(className))
        {
            var classStudents = _context.Students.Where(s => s.Class == className);
            if (!string.IsNullOrEmpty(sectionName))
                classStudents = classStudents.Where(s => s.Section == sectionName);

            var studentIds = classStudents.Select(s => s.Id).ToList();

            ViewBag.StudentCount = studentIds.Count;

            var attendances = _context.Attendances.Where(a => studentIds.Contains(a.StudentId)).ToList();
            var total = attendances.Count;
            var present = attendances.Count(a => a.Status == "Present" || a.Status == "Leave");
            ViewBag.AttendancePct = total > 0 ? (int)Math.Round((double)present / total * 100) : 100;

            var classAssignmentIds = _context.Assignments.Where(a => a.Class == className).Select(a => a.Id).ToList();
            ViewBag.PendingSubmissions = _context.AssignmentSubmissions
                .Count(s => s.Status == "Submitted" && classAssignmentIds.Contains(s.AssignmentId));

            ViewBag.Assignments = _context.Assignments
                .Where(a => a.Class == className)
                .OrderByDescending(a => a.DueDate)
                .Take(5)
                .ToList();

            ViewBag.Exams = _context.Exams
                .Where(e => e.Class == className)
                .OrderByDescending(e => e.Date)
                .Take(5)
                .ToList();
        }
        else
        {
            ViewBag.StudentCount = 0;
            ViewBag.AttendancePct = 100;
            ViewBag.PendingSubmissions = 0;
            ViewBag.Assignments = new List<Assignment>();
            ViewBag.Exams = new List<Exam>();
        }

        ViewBag.RecentAnnouncements = _context.Announcements
            .Where(a => (a.TargetClass == "All" || a.TargetClass == className) && (a.ExpiryDate == null || a.ExpiryDate >= DateTime.Today))
            .OrderByDescending(a => a.CreatedAt)
            .Take(4)
            .ToList();

        return View();
    }

    // VIEW DETAILS
    public IActionResult ViewDetails(string search)
    {
        if (!IsTeacher())
            return RedirectToAction("Index", "Home");

        var teacherId = HttpContext.Session.GetInt32("TeacherId") ?? 0;
        var teacher = _context.Teachers.Find(teacherId);
        var students = _context.Students.AsQueryable();

        if (!string.IsNullOrEmpty(teacher?.AssignedClass))
            students = students.Where(s => s.Class == teacher.AssignedClass);

        if (!string.IsNullOrEmpty(teacher?.AssignedSection))
            students = students.Where(s => s.Section == teacher.AssignedSection);

        var studentList = students.ToList();

        var vm = new TeacherViewModel
        {
            ClassStudents = studentList
        };

        if (!string.IsNullOrEmpty(search))
        {
            vm.SelectedStudent = studentList.FirstOrDefault(s =>
                (s.FullName != null && s.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                s.RollNumber == search);
        }
        else
        {
            vm.SelectedStudent = studentList.FirstOrDefault();
        }

        return View(vm);
    }

    // ADD REMARKS
    public IActionResult AddRemarks(string rollNo)
    {
        if (!IsTeacher())
            return RedirectToAction("Index", "Home");

        if (!string.IsNullOrEmpty(rollNo))
        {
            var student = _context.Students
                .FirstOrDefault(x => x.RollNumber == rollNo);

            ViewBag.Student = student;

            if (student != null)
            {
                ViewBag.Remarks = _context.Remarks
                    .Where(r => r.StudentId == student.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();
            }
        }

        return View();
    }

    [HttpPost]
    public IActionResult SaveRemark(
        int StudentId,
        string RemarkType,
        string Remark)
    {
        if (!IsTeacher())
            return RedirectToAction("Index", "Home");

        var teacherName = HttpContext.Session.GetString("TeacherName") ?? "Teacher";

        var newRemark = new Remark
        {
            StudentId = StudentId,
            TeacherName = teacherName,
            RemarkType = RemarkType,
            RemarkText = Remark,
            CreatedAt = DateTime.Now
        };

        _context.Remarks.Add(newRemark);
        _context.SaveChanges();

        var student = _context.Students.Find(StudentId);

        ViewBag.Student = student;

        ViewBag.Remarks = _context.Remarks
            .Where(r => r.StudentId == StudentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        ViewBag.Success = "Remark Save Ho Gaya!";

        return View("AddRemarks");
    }

    // CHANGE PASSWORD
    public IActionResult ChangePassword()
    {
        if (!IsTeacher())
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (!IsTeacher())
            return RedirectToAction("Index", "Home");

        var teacherId = HttpContext.Session.GetInt32("TeacherId") ?? 0;
        var teacher = _context.Teachers.Find(teacherId);

        if (teacher == null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (teacher.Password != currentPassword)
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

        teacher.Password = newPassword;
        _context.SaveChanges();

        ViewBag.Success = "Password updated successfully!";
        return View();
    }
}


}
