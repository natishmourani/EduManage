using Microsoft.AspNetCore.Mvc;
using EduManage.Models;
using EduManage.Data;
using EduManage.Helpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EduManage.Controllers
{
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetRole() => HttpContext.Session.GetString("Role");
        private int? GetStudentId() => HttpContext.Session.GetInt32("StudentId");
        private int? GetTeacherId() => HttpContext.Session.GetInt32("TeacherId");

        // GET: Exams List
        public IActionResult Index()
        {
            var role = GetRole();
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Index", "Home");

            if (role == "Student")
                return RedirectToAction("MyResults");

            var exams = new List<Exam>();

            if (role == "Admin")
            {
                exams = _context.Exams.OrderByDescending(e => e.Date).ToList();
            }
            else if (role == "Teacher")
            {
                var teacherId = GetTeacherId() ?? 0;
                var teacher = _context.Teachers.Find(teacherId);
                if (teacher != null)
                {
                    exams = _context.Exams
                        .Where(e => e.Class == teacher.AssignedClass)
                        .OrderByDescending(e => e.Date)
                        .ToList();
                }
            }

            // Get number of results entered and publish status
            var examStatusMap = new Dictionary<int, (int entered, bool published)>();
            foreach (var exam in exams)
            {
                var results = _context.Results.Where(r => r.ExamId == exam.Id).ToList();
                var entered = results.Count;
                var published = results.Any() && results.All(r => r.IsPublished);
                examStatusMap[exam.Id] = (entered, published);
            }

            ViewBag.StatusMap = examStatusMap;
            return View(exams);
        }

        // GET: Create Exam (Admin only)
        public IActionResult CreateExam()
        {
            if (GetRole() != "Admin")
                return RedirectToAction("Index", "Home");

            ViewBag.Classes = _context.Students.Select(s => s.Class).Distinct().Where(c => !string.IsNullOrEmpty(c)).ToList();
            return View();
        }

        // POST: Create Exam
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateExam(Exam model)
        {
            if (GetRole() != "Admin")
                return RedirectToAction("Index", "Home");

            if (model.Date.Date < DateTime.Today)
            {
                ModelState.AddModelError("Date", "Exam Date cannot be in the past.");
                ViewBag.Classes = _context.Students.Select(s => s.Class).Distinct().Where(c => !string.IsNullOrEmpty(c)).ToList();
                return View(model);
            }

            _context.Exams.Add(model);
            _context.SaveChanges();

            var announcement = new Announcement
            {
                Title = $"Exam Announcement: {model.Name}",
                Content = $"Exam '{model.Name}' for {model.Subject} is scheduled on {model.Date:dddd, dd MMMM yyyy}. Total marks: {model.TotalMarks}. Please prepare accordingly.",
                TargetClass = model.Class,
                AuthorName = "Super Admin",
                AuthorRole = "Admin",
                CreatedAt = DateTime.Now,
                ExpiryDate = model.Date.Date
            };
            _context.Announcements.Add(announcement);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Enter Marks (Teacher / Admin)
        public IActionResult EnterMarks(int examId)
        {
            var role = GetRole();
            if (role != "Teacher" && role != "Admin")
                return RedirectToAction("Index", "Home");

            var exam = _context.Exams.Find(examId);
            if (exam == null)
                return NotFound();

            if (DateTime.Today < exam.Date.Date)
            {
                return BadRequest("Cannot enter marks before the exam date.");
            }

            var students = _context.Students.Where(s => s.Class == exam.Class).ToList();
            var results = _context.Results.Where(r => r.ExamId == examId).ToDictionary(r => r.StudentId, r => r);

            ViewBag.Exam = exam;
            ViewBag.Results = results;

            return View(students);
        }

        // POST: Save Marks
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveMarks(int examId, Dictionary<int, int> obtainedMarks, Dictionary<int, string> remarks)
        {
            var role = GetRole();
            if (role != "Teacher" && role != "Admin")
                return RedirectToAction("Index", "Home");

            var exam = _context.Exams.Find(examId);
            if (exam == null)
                return NotFound();

            if (DateTime.Today < exam.Date.Date)
            {
                return BadRequest("Cannot enter marks before the exam date.");
            }

            foreach (var studentId in obtainedMarks.Keys)
            {
                var marks = obtainedMarks[studentId];
                var remark = remarks.ContainsKey(studentId) ? remarks[studentId] : "";

                // Calculate Grade
                double pct = ((double)marks / exam.TotalMarks) * 100;
                string grade = pct >= 90 ? "A+" : pct >= 80 ? "A" : pct >= 70 ? "B+" : pct >= 60 ? "B" : pct >= 50 ? "C" : "F";

                var existing = _context.Results.FirstOrDefault(r => r.ExamId == examId && r.StudentId == studentId);

                if (existing != null)
                {
                    existing.ObtainedMarks = marks;
                    existing.Grade = grade;
                    existing.Remarks = remark;
                }
                else
                {
                    var result = new Result
                    {
                        ExamId = examId,
                        StudentId = studentId,
                        ObtainedMarks = marks,
                        Grade = grade,
                        Remarks = remark,
                        IsPublished = false // Initially unpublished until Admin publishes
                    };
                    _context.Results.Add(result);
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Publish Results (Admin only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PublishResults(int examId)
        {
            if (GetRole() != "Admin")
                return RedirectToAction("Index", "Home");

            var results = _context.Results.Where(r => r.ExamId == examId).ToList();
            foreach (var r in results)
            {
                r.IsPublished = true;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: My Results (Student only)
        public IActionResult MyResults()
        {
            if (GetRole() != "Student")
                return RedirectToAction("Index", "Home");

            var studentId = GetStudentId() ?? 0;
            var student = _context.Students.Find(studentId);

            var results = _context.Results
                .Where(r => r.StudentId == studentId && r.IsPublished)
                .ToList();

            var examIds = results.Select(r => r.ExamId).ToList();
            var exams = _context.Exams.Where(e => examIds.Contains(e.Id)).ToDictionary(e => e.Id, e => e);

            ViewBag.Exams = exams;
            ViewBag.Student = student;

            if (student != null)
            {
                ViewBag.UpcomingExams = _context.Exams
                    .Where(e => e.Class == student.Class && e.Date.Date >= DateTime.Today)
                    .OrderBy(e => e.Date)
                    .ToList();

                ViewBag.ExamAnnouncements = StudentPortalHelper
                    .ActiveAnnouncementsForStudent(_context.Announcements, student)
                    .Where(a => a.Title != null && a.Title.StartsWith("Exam Announcement:"))
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList();
            }
            else
            {
                ViewBag.UpcomingExams = new List<Exam>();
                ViewBag.ExamAnnouncements = new List<Announcement>();
            }

            return View(results);
        }
    }
}
