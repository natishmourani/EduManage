using Microsoft.AspNetCore.Mvc;
using EduManage.Models;
using EduManage.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EduManage.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetRole() => HttpContext.Session.GetString("Role");

        // GET: Events list
        public IActionResult Index()
        {
            var role = GetRole();
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Index", "Home");

            var events = _context.Events.OrderBy(e => e.StartDate).ToList();
            return View(events);
        }

        // GET: Create (Admin only)
        public IActionResult Create()
        {
            if (GetRole() != "Admin")
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: Create (Admin only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Event model)
        {
            if (GetRole() != "Admin")
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

            _context.Events.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: Delete (Admin only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (GetRole() != "Admin")
                return RedirectToAction("Index", "Home");

            var ev = _context.Events.Find(id);
            if (ev != null)
            {
                _context.Events.Remove(ev);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
