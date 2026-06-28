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
    public class AnnouncementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnnouncementController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetRole() => HttpContext.Session.GetString("Role");
        private int? GetStudentId() => HttpContext.Session.GetInt32("StudentId");
        private int? GetTeacherId() => HttpContext.Session.GetInt32("TeacherId");

        // GET: Announcements
        public IActionResult Index()
        {
            var role = GetRole();
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Index", "Home");

            var announcements = new List<Announcement>();

            if (role == "Admin")
            {
                announcements = _context.Announcements.OrderByDescending(a => a.CreatedAt).ToList();
            }
            else if (role == "Teacher")
            {
                var teacherId = GetTeacherId() ?? 0;
                var teacher = _context.Teachers.Find(teacherId);
                // Teacher sees all school announcements + announcements they made (excluding expired ones)
                announcements = _context.Announcements
                    .Where(a => (a.TargetClass == "All" || a.AuthorName == teacher.FullName) && (a.ExpiryDate == null || a.ExpiryDate >= DateTime.Today))
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList();
            }
            else if (role == "Student")
            {
                var studentId = GetStudentId() ?? 0;
                var student = _context.Students.Find(studentId);
                if (student != null)
                {
                    announcements = StudentPortalHelper
                        .ActiveAnnouncementsForStudent(_context.Announcements, student)
                        .OrderByDescending(a => a.CreatedAt)
                        .ToList();
                }
            }

            return View(announcements);
        }

        // GET: Create (Admin / Teacher)
        public IActionResult Create()
        {
            var role = GetRole();
            if (role != "Admin" && role != "Teacher")
                return RedirectToAction("Index", "Home");

            ViewBag.Classes = _context.Students.Select(s => s.Class).Distinct().Where(c => !string.IsNullOrEmpty(c)).ToList();
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Announcement model)
        {
            var role = GetRole();
            if (role != "Admin" && role != "Teacher")
                return RedirectToAction("Index", "Home");

            if (model.ExpiryDate.HasValue && model.ExpiryDate.Value.Date < DateTime.Today)
            {
                ModelState.AddModelError("ExpiryDate", "Expiry date cannot be in the past.");
                ViewBag.Classes = _context.Students.Select(s => s.Class).Distinct().Where(c => !string.IsNullOrEmpty(c)).ToList();
                return View(model);
            }

            model.CreatedAt = DateTime.Now;
            
            if (role == "Admin")
            {
                model.AuthorName = "Super Admin";
                model.AuthorRole = "Admin";
            }
            else
            {
                var teacherId = GetTeacherId() ?? 0;
                var teacher = _context.Teachers.Find(teacherId);
                model.AuthorName = teacher?.FullName ?? "Teacher";
                model.AuthorRole = "Teacher";
                // Enforce target class to be their assigned class if not specified
                if (string.IsNullOrEmpty(model.TargetClass))
                {
                    model.TargetClass = teacher?.AssignedClass ?? "All";
                }
            }

            _context.Announcements.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var role = GetRole();
            if (role != "Admin" && role != "Teacher")
                return RedirectToAction("Index", "Home");

            var announcement = _context.Announcements.Find(id);
            if (announcement != null)
            {
                // Verify owner or is Admin
                if (role == "Admin" || (role == "Teacher" && announcement.AuthorRole == "Teacher"))
                {
                    _context.Announcements.Remove(announcement);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
    }
}
