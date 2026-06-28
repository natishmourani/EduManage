using Microsoft.AspNetCore.Mvc;
using EduManage.Models;
using EduManage.Data;
using EduManage.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace EduManage.Controllers
{
public class StudentController : Controller
{
private readonly ApplicationDbContext _context;


    public StudentController(ApplicationDbContext context)
    {
        _context = context;
    }

    private bool IsStudent()
    {
        return HttpContext.Session.GetString("Role") == "Student";
    }

    // DASHBOARD
    public IActionResult Index()
    {
        if (!IsStudent())
            return RedirectToAction("Index", "Home");

        int studentId = HttpContext.Session.GetInt32("StudentId") ?? 0;
        var student = _context.Students.Find(studentId);
        ViewBag.Student = student;

        if (student != null)
        {
            // Calculate GPA (average mark percentage)
            var studentResults = _context.Results.Where(r => r.StudentId == studentId && r.IsPublished).ToList();
            double totalObtained = studentResults.Sum(r => r.ObtainedMarks);
            double totalMax = 0;
            foreach (var r in studentResults)
            {
                var exam = _context.Exams.Find(r.ExamId);
                if (exam != null) totalMax += exam.TotalMarks;
            }
            ViewBag.GradeAverage = totalMax > 0 ? Math.Round((totalObtained / totalMax) * 100, 1) : 100;

            // Assignments counts
            var assignments = _context.Assignments.Where(a => a.Class == student.Class).ToList();
            var assignmentIds = assignments.Select(a => a.Id).ToList();
            var submissionCount = _context.AssignmentSubmissions
                .Count(s => s.StudentId == studentId && assignmentIds.Contains(s.AssignmentId));
            
            ViewBag.PendingAssignments = assignments.Count - submissionCount;

            ViewBag.RecentAnnouncements = StudentPortalHelper
                .ActiveAnnouncementsForStudent(_context.Announcements, student)
                .OrderByDescending(a => a.CreatedAt)
                .Take(4)
                .ToList();

            ViewBag.UpcomingExams = _context.Exams
                .Where(e => e.Class == student.Class && e.Date.Date >= DateTime.Today)
                .OrderBy(e => e.Date)
                .Take(4)
                .ToList();

            ViewBag.UpcomingEvents = _context.Events
                .Where(e => e.EndDate >= DateTime.Now)
                .OrderBy(e => e.StartDate)
                .Take(4)
                .ToList();
        }
        else
        {
            ViewBag.GradeAverage = 100.0;
            ViewBag.PendingAssignments = 0;
            ViewBag.RecentAnnouncements = new List<Announcement>();
            ViewBag.UpcomingExams = new List<Exam>();
            ViewBag.UpcomingEvents = new List<Event>();
        }

        return View();
    }

    // MY DETAILS
    public IActionResult MyDetails()
    {
        if (!IsStudent())
            return RedirectToAction("Index", "Home");

        int studentId = HttpContext.Session.GetInt32("StudentId") ?? 0;
        var student = _context.Students.Find(studentId);

        if (student != null)
        {
            // --- 1. SUBJECT MARKS REDESIGN ---
            var assignedSubjects = StudentPortalHelper.GetAssignedSubjects(_context, student);
            
            // Get all published results for this student
            var results = _context.Results
                .Where(r => r.StudentId == studentId && r.IsPublished)
                .Join(_context.Exams,
                    r => r.ExamId,
                    e => e.Id,
                    (r, e) => new { Result = r, Exam = e })
                .ToList();

            var examSubjects = results.Select(r => r.Exam.Subject).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
            var allSubjects = assignedSubjects.Union(examSubjects).OrderBy(s => s).ToList();

            var subjectMarksDetails = new List<SubjectMarksRedesignViewModel>();
            foreach (var subject in allSubjects)
            {
                var subjectResults = results.Where(r => r.Exam.Subject == subject).ToList();
                int totalObtained = subjectResults.Sum(r => r.Result.ObtainedMarks);
                int totalPossible = subjectResults.Sum(r => r.Exam.TotalMarks);
                double percentage = totalPossible > 0 ? Math.Round(((double)totalObtained / totalPossible) * 100, 1) : 0;
                
                string grade = "N/A";
                if (totalPossible > 0)
                {
                    grade = percentage >= 90 ? "A+" : percentage >= 80 ? "A" : percentage >= 70 ? "B+" : percentage >= 60 ? "B" : percentage >= 50 ? "C" : "F";
                }

                var assessments = subjectResults.Select(r => new AssessmentRecordViewModel
                {
                    AssessmentName = r.Exam.Name ?? "Assessment",
                    AssessmentType = r.Exam.Name ?? "Assessment",
                    ObtainedMarks = r.Result.ObtainedMarks,
                    TotalMarks = r.Exam.TotalMarks
                }).ToList();

                subjectMarksDetails.Add(new SubjectMarksRedesignViewModel
                {
                    SubjectName = subject,
                    Assessments = assessments,
                    TotalObtainedMarks = totalObtained,
                    TotalPossibleMarks = totalPossible,
                    Percentage = percentage,
                    Grade = grade
                });
            }
            ViewBag.SubjectMarksDetails = subjectMarksDetails;

            // --- 2. ATTENDANCE REDESIGN ---
            var attendanceDetails = new List<SubjectAttendanceViewModel>();
            foreach (var subject in allSubjects)
            {
                var subjectAttendanceRecords = _context.Attendances
                    .Where(a => a.StudentId == studentId && a.Subject == subject)
                    .OrderByDescending(a => a.Date)
                    .ToList();

                int totalClasses = subjectAttendanceRecords.Count;
                int attended = subjectAttendanceRecords.Count(a => a.Status == "Present" || a.Status == "Leave");
                int missed = subjectAttendanceRecords.Count(a => a.Status == "Absent");
                double pct = totalClasses > 0 ? Math.Round(((double)attended / totalClasses) * 100, 1) : 100.0;

                var history = subjectAttendanceRecords.Select(a => new AttendanceRecordViewModel
                {
                    Date = a.Date,
                    SubjectName = subject,
                    Status = a.Status
                }).ToList();

                attendanceDetails.Add(new SubjectAttendanceViewModel
                {
                    SubjectName = subject,
                    TotalClassesConducted = totalClasses,
                    ClassesAttended = attended,
                    ClassesMissed = missed,
                    AttendancePercentage = pct,
                    History = history
                });
            }
            ViewBag.AttendanceDetails = attendanceDetails;
        }
        else
        {
            ViewBag.SubjectMarksDetails = new List<SubjectMarksRedesignViewModel>();
            ViewBag.AttendanceDetails = new List<SubjectAttendanceViewModel>();
        }

        return View(student);
    }



    // CHECK REMARKS
    public IActionResult CheckRemarks()
    {
        if (!IsStudent())
            return RedirectToAction("Index", "Home");

        int studentId = HttpContext.Session.GetInt32("StudentId") ?? 0;

        var remarks = _context.Remarks
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return View(remarks);
    }

    // CHANGE PASSWORD
    public IActionResult ChangePassword()
    {
        if (!IsStudent())
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (!IsStudent())
            return RedirectToAction("Index", "Home");

        var studentId = HttpContext.Session.GetInt32("StudentId") ?? 0;
        var student = _context.Students.Find(studentId);

        if (student == null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (student.Password != currentPassword)
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

        student.Password = newPassword;
        _context.SaveChanges();

        ViewBag.Success = "Password updated successfully!";
        return View();
    }
}


}
