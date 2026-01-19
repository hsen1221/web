using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Required for UserManager
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

        // 1. ADD THIS FIELD 👇
        private readonly UserManager<AppUser> _userManager;

        // 2. UPDATE CONSTRUCTOR TO INJECT USERMANAGER 👇
        public LinePassengersController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager; // 3. ASSIGN IT HERE 👇
        }

        // GET: LinePassengers
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.LinePassengers.Include(l => l.Line);
            return View(await appDbContext.ToListAsync());
        }

        // GET: LinePassengers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var linePassenger = await _context.LinePassengers
                .Include(l => l.Line)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (linePassenger == null)
            {
                return NotFound();
            }

            return View(linePassenger);
        }

        // GET: LinePassengers/Create
        public IActionResult Create()
        {
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title");
            return View();
        }

        // POST: LinePassengers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // User must be logged in
        public async Task<IActionResult> Create([Bind("LineId")] LinePassenger linePassenger)
        {
            // 1. Get the ID of the currently logged-in user
            // NOW THIS WORKS because _userManager is defined
            var currentUserId = _userManager.GetUserId(User);

            // 2. Prevent duplicate sign-ups
            bool alreadyJoined = _context.LinePassengers.Any(p =>
                p.LineId == linePassenger.LineId &&
                p.AppUserId == currentUserId);

            if (alreadyJoined)
            {
                ModelState.AddModelError("", "You have already joined this line.");
            }

            if (ModelState.IsValid)
            {
                // 3. Set the relationship
                linePassenger.Id = Guid.NewGuid();
                linePassenger.AppUserId = currentUserId;
                linePassenger.IsActive = true;
                linePassenger.RegisteredDate = DateTime.UtcNow;

                // We often don't need "FullName" from the form if we can just grab it from the User account,
                // but if your model requires it, you might need to fill it here or make it optional.
                // For now, let's assume we copy it from the user account to satisfy the [Required] attribute:
                var appUser = await _userManager.FindByIdAsync(currentUserId);
                linePassenger.FullName = appUser?.FullName ?? "Unknown";

                _context.Add(linePassenger);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyLines));
            }

            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title", linePassenger.LineId);
            return View(linePassenger);
        }

        // GET: LinePassengers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var linePassenger = await _context.LinePassengers.FindAsync(id);
            if (linePassenger == null)
            {
                return NotFound();
            }
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title", linePassenger.LineId);
            return View(linePassenger);
        }

        // POST: LinePassengers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FullName,LineId,RegisteredDate,IsActive")] LinePassenger linePassenger)
        {
            if (id != linePassenger.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(linePassenger);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LinePassengerExists(linePassenger.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title", linePassenger.LineId);
            return View(linePassenger);
        }

        // GET: LinePassengers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var linePassenger = await _context.LinePassengers
                .Include(l => l.Line)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (linePassenger == null)
            {
                return NotFound();
            }

            return View(linePassenger);
        }

        // POST: LinePassengers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var linePassenger = await _context.LinePassengers.FindAsync(id);
            if (linePassenger != null)
            {
                _context.LinePassengers.Remove(linePassenger);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LinePassengerExists(Guid id)
        {
            return _context.LinePassengers.Any(e => e.Id == id);
        }

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
    }
}