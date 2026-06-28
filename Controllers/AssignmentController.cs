using Microsoft.AspNetCore.Mvc;
using EduManage.Models;
using EduManage.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace EduManage.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetRole() => HttpContext.Session.GetString("Role");
        private int? GetStudentId() => HttpContext.Session.GetInt32("StudentId");
        private int? GetTeacherId() => HttpContext.Session.GetInt32("TeacherId");

        // GET: Index (Lists assignments based on role)
        public IActionResult Index()
        {
            var role = GetRole();
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Index", "Home");

            var assignments = new List<Assignment>();

            if (role == "Admin")
            {
                assignments = _context.Assignments.OrderByDescending(a => a.CreatedAt).ToList();
            }
            else if (role == "Teacher")
            {
                var teacherId = GetTeacherId() ?? 0;
                assignments = _context.Assignments
                    .Where(a => a.TeacherId == teacherId)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList();
            }
            else if (role == "Student")
            {
                var studentId = GetStudentId() ?? 0;
                var student = _context.Students.Find(studentId);
                if (student != null)
                {
                    assignments = _context.Assignments
                        .Where(a => a.Class == student.Class)
                        .OrderByDescending(a => a.CreatedAt)
                        .ToList();
                }
            }

            // For students, find which assignments have already been submitted
            if (role == "Student")
            {
                var studentId = GetStudentId() ?? 0;
                var submissions = _context.AssignmentSubmissions
                    .Where(s => s.StudentId == studentId)
                    .ToDictionary(s => s.AssignmentId, s => s.Status);

                ViewBag.Submissions = submissions;
            }

            return View(assignments);
        }

        // GET: Create (Teacher only)
        public IActionResult Create()
        {
            if (GetRole() != "Teacher")
                return RedirectToAction("Index", "Home");

            var teacherId = GetTeacherId() ?? 0;
            var teacher = _context.Teachers.Find(teacherId);
            ViewBag.AssignedClass = teacher?.AssignedClass;

            return View();
        }

        // POST: Create (Teacher only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Assignment model, IFormFile? file)
        {
            if (GetRole() != "Teacher")
                return RedirectToAction("Index", "Home");

            var teacherId = GetTeacherId() ?? 0;
            var teacher = _context.Teachers.Find(teacherId);

            if (model.DueDate < DateTime.Now)
            {
                ModelState.AddModelError("DueDate", "Due Date cannot be in the past.");
                ViewBag.AssignedClass = teacher?.AssignedClass;
                return View(model);
            }

            model.TeacherId = teacherId;
            model.CreatedAt = DateTime.Now;

            if (file != null && file.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "assignments");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                model.FilePath = "/uploads/assignments/" + fileName;
            }

            _context.Assignments.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Details (Assignment Details & Submissions)
        public IActionResult Details(int id)
        {
            var role = GetRole();
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Index", "Home");

            var assignment = _context.Assignments.Find(id);
            if (assignment == null)
                return NotFound();

            var teacher = _context.Teachers.Find(assignment.TeacherId);
            ViewBag.TeacherName = teacher?.FullName ?? "Unknown Teacher";

            if (role == "Teacher" || role == "Admin")
            {
                // Find all students in this class
                var classStudents = _context.Students.Where(s => s.Class == assignment.Class).ToList();
                var submissions = _context.AssignmentSubmissions.Where(s => s.AssignmentId == id).ToList();

                var submissionMap = submissions.ToDictionary(s => s.StudentId, s => s);
                ViewBag.ClassStudents = classStudents;
                ViewBag.SubmissionMap = submissionMap;
            }
            else if (role == "Student")
            {
                var studentId = GetStudentId() ?? 0;
                var submission = _context.AssignmentSubmissions
                    .FirstOrDefault(s => s.AssignmentId == id && s.StudentId == studentId);
                
                ViewBag.Submission = submission;
            }

            return View(assignment);
        }

        // POST: Submit (Student only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(int assignmentId, IFormFile file)
        {
            if (GetRole() != "Student")
                return RedirectToAction("Index", "Home");

            var studentId = GetStudentId() ?? 0;
            var assignment = _context.Assignments.Find(assignmentId);
            if (assignment == null)
                return NotFound();

            if (file != null && file.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "submissions");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var existing = _context.AssignmentSubmissions
                    .FirstOrDefault(s => s.AssignmentId == assignmentId && s.StudentId == studentId);

                if (existing != null)
                {
                    existing.FilePath = "/uploads/submissions/" + fileName;
                    existing.SubmissionDate = DateTime.Now;
                    existing.Status = "Submitted";
                }
                else
                {
                    var submission = new AssignmentSubmission
                    {
                        AssignmentId = assignmentId,
                        StudentId = studentId,
                        SubmissionDate = DateTime.Now,
                        FilePath = "/uploads/submissions/" + fileName,
                        Status = "Submitted"
                    };
                    _context.AssignmentSubmissions.Add(submission);
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = assignmentId });
        }

        // POST: Grade (Teacher only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Grade(int submissionId, string grade, string feedback)
        {
            if (GetRole() != "Teacher")
                return RedirectToAction("Index", "Home");

            var submission = _context.AssignmentSubmissions.Find(submissionId);
            if (submission == null)
                return NotFound();

            submission.Grade = grade;
            submission.Feedback = feedback;
            submission.Status = "Graded";

            _context.SaveChanges();

            return RedirectToAction("Details", new { id = submission.AssignmentId });
        }
    }
}
