using Microsoft.AspNetCore.Mvc;
using EduManage.Models;
using EduManage.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EduManage.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessageController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetRole() => HttpContext.Session.GetString("Role");
        private int? GetStudentId() => HttpContext.Session.GetInt32("StudentId");
        private int? GetTeacherId() => HttpContext.Session.GetInt32("TeacherId");

        // GET: Inbox
        public IActionResult Inbox()
        {
            var role = GetRole();
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Index", "Home");

            int currentUserId = 0;
            if (role == "Student") currentUserId = GetStudentId() ?? 0;
            else if (role == "Teacher") currentUserId = GetTeacherId() ?? 0;
            else if (role == "Admin") return RedirectToAction("Monitor");

            var inboxMessages = _context.Messages
                .Where(m => m.ReceiverId == currentUserId && m.ReceiverRole == role)
                .OrderByDescending(m => m.SentAt)
                .ToList();

            return View(inboxMessages);
        }

        // GET: Sent
        public IActionResult Sent()
        {
            var role = GetRole();
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Index", "Home");

            int currentUserId = 0;
            if (role == "Student") currentUserId = GetStudentId() ?? 0;
            else if (role == "Teacher") currentUserId = GetTeacherId() ?? 0;
            else if (role == "Admin") return RedirectToAction("Monitor");

            var sentMessages = _context.Messages
                .Where(m => m.SenderId == currentUserId && m.SenderRole == role)
                .OrderByDescending(m => m.SentAt)
                .ToList();

            return View(sentMessages);
        }

        // GET: Send Message
        public IActionResult Send()
        {
            var role = GetRole();
            if (role != "Student" && role != "Teacher")
                return RedirectToAction("Index", "Home");

            if (role == "Student")
            {
                // Students can message teachers
                ViewBag.Recipients = _context.Teachers
                    .Select(t => new { Id = t.Id, Name = t.FullName + " (Teacher)" })
                    .ToList();
            }
            else
            {
                // Teachers can message students
                ViewBag.Recipients = _context.Students
                    .Select(s => new { Id = s.Id, Name = s.FullName + " (Student - Roll: " + s.RollNumber + ")" })
                    .ToList();
            }

            return View();
        }

        // POST: Send Message
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Send(int receiverId, string subject, string body)
        {
            var role = GetRole();
            if (role != "Student" && role != "Teacher")
                return RedirectToAction("Index", "Home");

            int senderId = 0;
            string senderName = "";
            string receiverRole = "";
            string receiverName = "";

            if (role == "Student")
            {
                senderId = GetStudentId() ?? 0;
                var student = _context.Students.Find(senderId);
                senderName = student?.FullName ?? "Student";

                receiverRole = "Teacher";
                var teacher = _context.Teachers.Find(receiverId);
                receiverName = teacher?.FullName ?? "Teacher";
            }
            else
            {
                senderId = GetTeacherId() ?? 0;
                var teacher = _context.Teachers.Find(senderId);
                senderName = teacher?.FullName ?? "Teacher";

                receiverRole = "Student";
                var student = _context.Students.Find(receiverId);
                receiverName = student?.FullName ?? "Student";
            }

            var message = new Message
            {
                SenderId = senderId,
                SenderRole = role,
                SenderName = senderName,
                ReceiverId = receiverId,
                ReceiverRole = receiverRole,
                ReceiverName = receiverName,
                Subject = subject,
                Body = body,
                SentAt = DateTime.Now,
                IsRead = false
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            return RedirectToAction("Sent");
        }

        // GET: Monitor Messages (Admin only)
        public IActionResult Monitor()
        {
            if (GetRole() != "Admin")
                return RedirectToAction("Index", "Home");

            var allMessages = _context.Messages.OrderByDescending(m => m.SentAt).ToList();
            return View(allMessages);
        }
    }
}
