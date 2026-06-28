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
    public class StudyMaterialController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudyMaterialController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetRole() => HttpContext.Session.GetString("Role");
        private int? GetStudentId() => HttpContext.Session.GetInt32("StudentId");
        private int? GetTeacherId() => HttpContext.Session.GetInt32("TeacherId");

        // GET: Study Material List
        public IActionResult Index(string classFilter)
        {
            var role = GetRole();
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Index", "Home");

            var materials = new List<StudyMaterial>();

            if (role == "Admin" || role == "Teacher")
            {
                var query = _context.StudyMaterials.AsQueryable();
                if (!string.IsNullOrEmpty(classFilter))
                {
                    query = query.Where(m => m.Class == classFilter);
                }
                materials = query.OrderByDescending(m => m.UploadedAt).ToList();
                ViewBag.Classes = _context.Students.Select(s => s.Class).Distinct().Where(c => !string.IsNullOrEmpty(c)).ToList();
                ViewBag.SelectedClass = classFilter;
            }
            else if (role == "Student")
            {
                var studentId = GetStudentId() ?? 0;
                var student = _context.Students.Find(studentId);
                if (student != null)
                {
                    materials = _context.StudyMaterials
                        .Where(m => m.Class == student.Class)
                        .OrderByDescending(m => m.UploadedAt)
                        .ToList();
                }
            }

            var teachers = _context.Teachers.ToDictionary(t => t.Id, t => t.FullName);
            ViewBag.Teachers = teachers;

            return View(materials);
        }

        // GET: Upload (Teacher / Admin)
        public IActionResult Upload()
        {
            var role = GetRole();
            if (role != "Teacher" && role != "Admin")
                return RedirectToAction("Index", "Home");

            if (role == "Teacher")
            {
                var teacherId = GetTeacherId() ?? 0;
                var teacher = _context.Teachers.Find(teacherId);
                ViewBag.Classes = new List<string> { teacher?.AssignedClass ?? "" }.Where(c => !string.IsNullOrEmpty(c)).ToList();
            }
            else
            {
                ViewBag.Classes = _context.Students.Select(s => s.Class).Distinct().Where(c => !string.IsNullOrEmpty(c)).ToList();
            }
            return View();
        }

        // POST: Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upload(StudyMaterial model, IFormFile file)
        {
            var role = GetRole();
            if (role != "Teacher" && role != "Admin")
                return RedirectToAction("Index", "Home");

            if (role == "Teacher")
            {
                var teacherId = GetTeacherId() ?? 0;
                var teacher = _context.Teachers.Find(teacherId);
                if (teacher == null || teacher.AssignedClass != model.Class)
                {
                    return BadRequest("You are only allowed to upload study materials for your assigned class (" + teacher?.AssignedClass + ").");
                }
            }

            model.UploadedAt = DateTime.Now;
            model.TeacherId = role == "Teacher" ? (GetTeacherId() ?? 0) : 0; // 0 for Admin

            if (file != null && file.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "materials");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                model.FilePath = "/uploads/materials/" + fileName;

                _context.StudyMaterials.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Error = "Please select a file to upload.";
            if (role == "Teacher")
            {
                var teacherId = GetTeacherId() ?? 0;
                var teacher = _context.Teachers.Find(teacherId);
                ViewBag.Classes = new List<string> { teacher?.AssignedClass ?? "" }.Where(c => !string.IsNullOrEmpty(c)).ToList();
            }
            else
            {
                ViewBag.Classes = _context.Students.Select(s => s.Class).Distinct().Where(c => !string.IsNullOrEmpty(c)).ToList();
            }
            return View(model);
        }

        // POST: Delete (Admin / Teacher)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var role = GetRole();
            if (role != "Teacher" && role != "Admin")
                return RedirectToAction("Index", "Home");

            var material = _context.StudyMaterials.Find(id);
            if (material != null)
            {
                // Verify owner or is Admin
                if (role == "Admin" || (role == "Teacher" && material.TeacherId == (GetTeacherId() ?? 0)))
                {
                    if (role == "Teacher")
                    {
                        var teacherId = GetTeacherId() ?? 0;
                        var teacher = _context.Teachers.Find(teacherId);
                        if (teacher == null || teacher.AssignedClass != material.Class)
                        {
                            return BadRequest("You are only allowed to delete study materials for your assigned class (" + teacher?.AssignedClass + ").");
                        }
                    }

                    if (!string.IsNullOrEmpty(material.FilePath))
                    {
                        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", material.FilePath.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                    _context.StudyMaterials.Remove(material);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
    }
}
