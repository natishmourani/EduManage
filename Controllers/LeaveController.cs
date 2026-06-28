using Microsoft.AspNetCore.Mvc;
using EduManage.Models;
using EduManage.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EduManage.Controllers
{
    public class LeaveController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetRole() => HttpContext.Session.GetString("Role");
        private int? GetStudentId() => HttpContext.Session.GetInt32("StudentId");
        private int? GetTeacherId() => HttpContext.Session.GetInt32("TeacherId");

        // GET: Leave Requests list
        public IActionResult Index()
        {
            var role = GetRole();
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Index", "Home");

            var requests = new List<LeaveRequest>();

            if (role == "Admin")
            {
                requests = _context.LeaveRequests.OrderByDescending(l => l.RequestedAt).ToList();
            }
            else if (role == "Teacher")
            {
                var teacherId = GetTeacherId() ?? 0;
                requests = _context.LeaveRequests
                    .Where(l => l.RequesterId == teacherId && l.RequesterRole == "Teacher")
                    .OrderByDescending(l => l.RequestedAt)
                    .ToList();
            }
            else if (role == "Student")
            {
                var studentId = GetStudentId() ?? 0;
                requests = _context.LeaveRequests
                    .Where(l => l.RequesterId == studentId && l.RequesterRole == "Student")
                    .OrderByDescending(l => l.RequestedAt)
                    .ToList();
            }

            return View(requests);
        }

        // GET: Submit Leave Request (Student / Teacher)
        public IActionResult SubmitRequest()
        {
            var role = GetRole();
            if (role != "Student" && role != "Teacher")
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: Submit Leave Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitRequest(LeaveRequest model)
        {
            var role = GetRole();
            if (role != "Student" && role != "Teacher")
                return RedirectToAction("Index", "Home");

            if (model.StartDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("StartDate", "Start date cannot be in the past.");
                return View(model);
            }
            if (model.EndDate.Date < model.StartDate.Date)
            {
                ModelState.AddModelError("EndDate", "End date must be on or after start date.");
                return View(model);
            }

            model.RequestedAt = DateTime.Now;
            model.Status = "Pending";
            model.RequesterRole = role;

            if (role == "Student")
            {
                var id = GetStudentId() ?? 0;
                var student = _context.Students.Find(id);
                model.RequesterId = id;
                model.RequesterName = student?.FullName ?? "Student";
            }
            else
            {
                var id = GetTeacherId() ?? 0;
                var teacher = _context.Teachers.Find(id);
                model.RequesterId = id;
                model.RequesterName = teacher?.FullName ?? "Teacher";
            }

            _context.LeaveRequests.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: Approve or Reject Request (Admin only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveReject(int id, string status)
        {
            if (GetRole() != "Admin")
                return RedirectToAction("Index", "Home");

            var request = _context.LeaveRequests.Find(id);
            if (request != null && (status == "Approved" || status == "Rejected"))
            {
                request.Status = status;
                _context.SaveChanges();

                // If approved student leave, auto mark in Attendance table as Leave
                if (status == "Approved" && request.RequesterRole == "Student")
                {
                    for (var date = request.StartDate.Date; date <= request.EndDate.Date; date = date.AddDays(1))
                    {
                        var existing = _context.Attendances.FirstOrDefault(a => a.StudentId == request.RequesterId && a.Date == date);
                        if (existing != null)
                        {
                            existing.Status = "Leave";
                            existing.Remarks = $"Leave approved: {request.Reason}";
                        }
                        else
                        {
                            var record = new Attendance
                            {
                                StudentId = request.RequesterId,
                                Date = date,
                                Status = "Leave",
                                Remarks = $"Leave approved: {request.Reason}"
                            };
                            _context.Attendances.Add(record);
                        }
                    }
                    _context.SaveChanges();

                    // Recalculate Student Overall Attendance Percentage
                    var student = _context.Students.Find(request.RequesterId);
                    if (student != null)
                    {
                        var totalDays = _context.Attendances.Count(a => a.StudentId == student.Id);
                        var presentDays = _context.Attendances.Count(a => a.StudentId == student.Id && (a.Status == "Present" || a.Status == "Leave"));
                        student.Attendance = totalDays > 0 ? (int)Math.Round((double)presentDays / totalDays * 100) : 100;
                        _context.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}
