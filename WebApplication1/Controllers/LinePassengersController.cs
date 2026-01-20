using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class LinePassengersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public LinePassengersController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: LinePassengers/MyLines
        // Passengers can still SEE their own trips here
        [Authorize]
        public async Task<IActionResult> MyLines()
        {
            var currentUserId = _userManager.GetUserId(User);

            var myTrips = await _context.LinePassengers
                .Include(lp => lp.Line)
                .Where(lp => lp.AppUserId == currentUserId)
                .ToListAsync();

            return View(myTrips);
        }

        // GET: LinePassengers (Global List - Optional: Restrict to Admin if you want)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.LinePassengers.Include(l => l.Line);
            return View(await appDbContext.ToListAsync());
        }

        // GET: LinePassengers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var linePassenger = await _context.LinePassengers
                .Include(l => l.Line)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (linePassenger == null) return NotFound();

            return View(linePassenger);
        }

        // ============================================================
        // ADMIN ONLY SECTION: Adding/Editing Passengers
        // ============================================================

        // GET: LinePassengers/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title");
            // Load Users so Admin can pick one
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: LinePassengers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("LineId,FullName,AppUserId")] LinePassenger linePassenger)
        {
            // Check if this specific user is already on the line
            bool alreadyJoined = _context.LinePassengers.Any(p =>
                p.LineId == linePassenger.LineId &&
                p.AppUserId == linePassenger.AppUserId);

            if (alreadyJoined)
            {
                ModelState.AddModelError("", "This user is already on this line.");
            }

            if (ModelState.IsValid)
            {
                linePassenger.Id = Guid.NewGuid();
                linePassenger.IsActive = true;
                linePassenger.RegisteredDate = DateTime.UtcNow;

                _context.Add(linePassenger);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Passenger added successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title", linePassenger.LineId);
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Email", linePassenger.AppUserId);
            return View(linePassenger);
        }

        // GET: LinePassengers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var linePassenger = await _context.LinePassengers.FindAsync(id);
            if (linePassenger == null) return NotFound();

            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title", linePassenger.LineId);
            // Don't forget to load the user list for Edit too, or just keep the hidden field approach
            return View(linePassenger);
        }

        // POST: LinePassengers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FullName,LineId,RegisteredDate,IsActive,AppUserId")] LinePassenger linePassenger)
        {
            if (id != linePassenger.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(linePassenger);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LinePassengerExists(linePassenger.Id)) return NotFound();
                    else throw;
                }
                TempData["SuccessMessage"] = "Passenger details updated.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title", linePassenger.LineId);
            return View(linePassenger);
        }

        // GET: LinePassengers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var linePassenger = await _context.LinePassengers
                .Include(l => l.Line)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (linePassenger == null) return NotFound();

            return View(linePassenger);
        }

        // POST: LinePassengers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var linePassenger = await _context.LinePassengers.FindAsync(id);
            if (linePassenger != null)
            {
                _context.LinePassengers.Remove(linePassenger);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Passenger removed from line successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool LinePassengerExists(Guid id)
        {
            return _context.LinePassengers.Any(e => e.Id == id);
        }
    }
}