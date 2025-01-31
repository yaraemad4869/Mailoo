﻿using Mailo.Data;
using Mailoo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mailoo.Controllers
{
    public class ColorController : Controller
    {
        private readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var colors = _context.Colors.ToList();
            return View(colors);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Color color)
        {
            if (ModelState.IsValid)
            {
                _context.Colors.Add(color);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(color);
        }

        public IActionResult Edit(int id)
        {
            var color = _context.Colors.Find(id);
            if (color == null)
                return NotFound();

            return View(color);
        }

        [HttpPost]
        public IActionResult Edit(Color color)
        {
            if (ModelState.IsValid)
            {
                _context.Colors.Update(color);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(color);
        }


        public IActionResult Delete(int id)
        {
            var color = _context.Colors.Find(id);
            if (color == null)
                return NotFound();

            return View(color);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var color = _context.Colors.Find(id); 
            if (color == null)
                return NotFound();

            _context.Colors.Remove(color);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
