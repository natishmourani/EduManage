using Microsoft.AspNetCore.Mvc;
using EduManage.Models;
using EduManage.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EduManage.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetRole() => HttpContext.Session.GetString("Role");
        private int? GetStudentId() => HttpContext.Session.GetInt32("StudentId");
        private int? GetTeacherId() => HttpContext.Session.GetInt32("TeacherId");

        // GET: Mark Attendance (Teacher/Admin)
        public IActionResult Mark(string className, string subject, DateTime? date)
        {
            var role = GetRole();
            if (role != "Teacher" && role != "Admin")
                return RedirectToAction("Index", "Home");

            var selectedDate = (date ?? DateTime.Today).Date;
            if (selectedDate > DateTime.Today)
            {
                return BadRequest("Attendance date cannot be in the future.");
            }
            ViewBag.SelectedDate = selectedDate;

            // Enforce teacher's assigned class and subject
            if (role == "Teacher")
            {
                var teacherId = GetTeacherId() ?? 0;
                var teacher = _context.Teachers.Find(teacherId);
                className = teacher?.AssignedClass ?? "";
                subject = teacher?.Subject ?? "";
            }

            ViewBag.ClassName = className;
            ViewBag.Subject = subject;
            ViewBag.Classes = _context.Students.Select(s => s.Class).Distinct().Where(c => !string.IsNullOrEmpty(c)).ToList();

            if (role == "Admin" && !string.IsNullOrEmpty(className))
            {
                var subjects = _context.Teachers
                    .Where(t => t.AssignedClass == className)
                    .Select(t => t.Subject)
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Distinct()
                    .ToList();
                if (!subjects.Any())
                {
                    subjects = _context.Teachers
                        .Select(t => t.Subject)
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Distinct()
                        .ToList();
                }
                ViewBag.Subjects = subjects;
            }

            var students = new List<Student>();
            var attendanceRecords = new Dictionary<int, Attendance>();

            if (!string.IsNullOrEmpty(className))
            {
                var studentsQuery = _context.Students.Where(s => s.Class == className);
                if (role == "Teacher")
                {
                    var teacherId = GetTeacherId() ?? 0;
                    var teacher = _context.Teachers.Find(teacherId);
                    if (!string.IsNullOrEmpty(teacher?.AssignedSection))
                        studentsQuery = studentsQuery.Where(s => s.Section == teacher.AssignedSection);
                }

                students = studentsQuery.ToList();
                var studentIds = students.Select(s => s.Id).ToList();
                
                var records = _context.Attendances
                    .Where(a => a.Date == selectedDate && studentIds.Contains(a.StudentId) && a.Subject == subject)
                    .ToList();

                foreach (var r in records)
                {
                    attendanceRecords[r.StudentId] = r;
                }
            }

            ViewBag.AttendanceRecords = attendanceRecords;
            return View(students);
        }

        // POST: Save/Update Attendance (Teacher/Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(string className, string subject, DateTime date, Dictionary<int, string> statuses, Dictionary<int, string> remarks)
        {
            var role = GetRole();
            if (role != "Teacher" && role != "Admin")
                return RedirectToAction("Index", "Home");

            var selectedDate = date.Date;
            if (selectedDate > DateTime.Today)
            {
                return BadRequest("Attendance date cannot be in the future.");
            }

            if (role == "Teacher")
            {
                var teacherId = GetTeacherId() ?? 0;
                var teacher = _context.Teachers.Find(teacherId);
                className = teacher?.AssignedClass ?? "";
                subject = teacher?.Subject ?? "";

                // Restrict updating attendance only to students of teacher's assigned class/section
                var allowedStudentIds = _context.Students
                    .Where(s => s.Class == teacher.AssignedClass && (string.IsNullOrEmpty(teacher.AssignedSection) || s.Section == teacher.AssignedSection))
                    .Select(s => s.Id)
                    .ToList();

                var keysToKeep = statuses.Keys.Intersect(allowedStudentIds).ToList();
                var filteredStatuses = new Dictionary<int, string>();
                var filteredRemarks = new Dictionary<int, string>();
                foreach (var k in keysToKeep)
                {
                    filteredStatuses[k] = statuses[k];
                    if (remarks.ContainsKey(k))
                        filteredRemarks[k] = remarks[k];
                }
                statuses = filteredStatuses;
                remarks = filteredRemarks;
            }

            if (string.IsNullOrEmpty(subject))
            {
                return BadRequest("Subject is required to save attendance.");
            }

            foreach (var studentId in statuses.Keys)
            {
                var status = statuses[studentId];
                var remark = remarks.ContainsKey(studentId) ? remarks[studentId] : null;

                var existing = _context.Attendances
                    .FirstOrDefault(a => a.StudentId == studentId && a.Date == selectedDate && a.Subject == subject);

                if (existing != null)
                {
                    existing.Status = status;
                    existing.Remarks = remark;
                }
                else
                {
                    var record = new Attendance
                    {
                        StudentId = studentId,
                        Date = selectedDate,
                        Status = status,
                        Remarks = remark,
                        Subject = subject
                    };
                    _context.Attendances.Add(record);
                }

                _context.SaveChanges();

                // Recalculate Student Overall Attendance Percentage
                var student = _context.Students.Find(studentId);
                if (student != null)
                {
                    var totalDays = _context.Attendances.Count(a => a.StudentId == studentId);
                    var presentDays = _context.Attendances.Count(a => a.StudentId == studentId && (a.Status == "Present" || a.Status == "Leave"));
                    
                    student.Attendance = totalDays > 0 ? (int)Math.Round((double)presentDays / totalDays * 100) : 100;
                    _context.SaveChanges();
                }
            }

            ViewBag.Success = "Attendance marked/updated successfully.";
            return RedirectToAction("Mark", new { className, subject, date = selectedDate.ToString("yyyy-MM-dd") });
        }

        // GET: Attendance History (Teacher/Admin)
        public IActionResult History(string className)
        {
            var role = GetRole();
            if (role != "Teacher" && role != "Admin")
                return RedirectToAction("Index", "Home");

            if (role == "Teacher")
            {
                var teacherId = GetTeacherId() ?? 0;
                var teacher = _context.Teachers.Find(teacherId);
                className = teacher?.AssignedClass ?? "";
            }

            ViewBag.ClassName = className;
            ViewBag.Classes = _context.Students.Select(s => s.Class).Distinct().Where(c => !string.IsNullOrEmpty(c)).ToList();

            var historyData = new List<object>();

            if (!string.IsNullOrEmpty(className))
            {
                var students = _context.Students.Where(s => s.Class == className).ToList();
                var studentIds = students.Select(s => s.Id).ToList();

                var records = _context.Attendances
                    .Where(a => studentIds.Contains(a.StudentId))
                    .OrderByDescending(a => a.Date)
                    .ToList();

                var grouped = records.GroupBy(r => r.Date).OrderByDescending(g => g.Key);
                foreach (var g in grouped)
                {
                    historyData.Add(new {
                        Date = g.Key,
                        Present = g.Count(r => r.Status == "Present"),
                        Absent = g.Count(r => r.Status == "Absent"),
                        Leave = g.Count(r => r.Status == "Leave"),
                        Total = students.Count
                    });
                }
            }

            ViewBag.HistoryData = historyData;
            return View();
        }

        // GET: My Attendance (Student only)
        public IActionResult MyAttendance()
        {
            if (GetRole() != "Student")
                return RedirectToAction("Index", "Home");

            var studentId = GetStudentId() ?? 0;
            var student = _context.Students.Find(studentId);
            
            var records = _context.Attendances
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.Date)
                .ToList();

            ViewBag.Student = student;
            return View(records);
        }

        // GET: Admin Reports & Statistics
        public IActionResult Reports()
        {
            if (GetRole() != "Admin")
                return RedirectToAction("Index", "Home");

            var classes = _context.Students.Select(s => s.Class).Distinct().Where(c => !string.IsNullOrEmpty(c)).ToList();
            var classStats = new List<ClassAttendanceStat>();

            foreach (var cls in classes)
            {
                var students = _context.Students.Where(s => s.Class == cls).ToList();
                var studentIds = students.Select(s => s.Id).ToList();
                
                var totalRecords = _context.Attendances.Count(a => studentIds.Contains(a.StudentId));
                var presentRecords = _context.Attendances.Count(a => studentIds.Contains(a.StudentId) && (a.Status == "Present" || a.Status == "Leave"));

                double avg = totalRecords > 0 ? Math.Round((double)presentRecords / totalRecords * 100, 1) : 100.0;

                classStats.Add(new ClassAttendanceStat {
                    ClassName = cls ?? "Unknown",
                    StudentCount = students.Count,
                    AverageAttendance = avg
                });
            }

            return View(classStats);
        }
    }

    public class ClassAttendanceStat
    {
        public string ClassName { get; set; } = "";
        public int StudentCount { get; set; }
        public double AverageAttendance { get; set; }
    }
}
