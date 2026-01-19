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
        [Authorize(Roles = "Admin")] // <--- CHANGE THIS (Was just [Authorize])
        public IActionResult Create()
        {
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title");
            // We need a list of users so the Admin can pick one!
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }
        // POST: LinePassengers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // <--- CHANGE THIS
                                     // Add "AppUserId" to the bind list so we can receive the selected user
        public async Task<IActionResult> Create([Bind("LineId,FullName,AppUserId")] LinePassenger linePassenger)
        {
            // REMOVED: var currentUserId = _userManager.GetUserId(User); 
            // We now use the AppUserId sent from the form (selected by Admin)

            // Check if the selected user is already in the line
            bool alreadyJoined = _context.LinePassengers.Any(p =>
                p.LineId == linePassenger.LineId &&
                p.AppUserId == linePassenger.AppUserId); // <--- Use the selected ID

            if (alreadyJoined)
            {
                ModelState.AddModelError("", "This user is already on this line.");
            }

            if (ModelState.IsValid)
            {
                linePassenger.Id = Guid.NewGuid();
                // linePassenger.AppUserId is already set from the form!
                linePassenger.IsActive = true;
                linePassenger.RegisteredDate = DateTime.UtcNow;

                _context.Add(linePassenger);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Passenger added successfully!";
                return RedirectToAction(nameof(Index)); // Go back to global list (Admin view)
            }

            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title", linePassenger.LineId);
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Email", linePassenger.AppUserId); // Reload user list if error
            return View(linePassenger);
        }
        // GET: LinePassengers/Edit/5
        [Authorize(Roles = "Admin")] // <--- CHANGE THIS
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
        [Authorize(Roles = "Admin")] // <--- CHANGE THIS

        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FullName,LineId,RegisteredDate,IsActive,AppUserId")] LinePassenger linePassenger)
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
                TempData["SuccessMessage"] = "Passenger details updated."; // <--- Add this
                return RedirectToAction(nameof(Index));
            }
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title", linePassenger.LineId);
            return View(linePassenger);
        }

        // GET: LinePassengers/Delete/5
        [Authorize(Roles = "Admin")] // <--- CHANGE THIS
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
        [Authorize(Roles = "Admin")] // <--- CHANGE THIS
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var linePassenger = await _context.LinePassengers.FindAsync(id);
            if (linePassenger != null)
            {
                _context.LinePassengers.Remove(linePassenger);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Passenger removed from line successfully."; // <--- Add this
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