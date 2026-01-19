using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; // <--- Needed to get User Id
using Microsoft.AspNetCore.Identity;
namespace WebApplication1.Controllers
{

    [Authorize] // Require login for everything in this controller
    public class LinesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager; // <--- 2. ADD THIS FIELD
        // 3. UPDATE CONSTRUCTOR TO INJECT USERMANAGER 👇
        public LinesController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager; // <--- Assign it here
        }
        // GET: Lines
        // Allow Admins AND Authenticated Users to see the list
        [AllowAnonymous] // Or just [Authorize] if you want them logged in
        public async Task<IActionResult> Index()
        {
            return View(await _context.Lines.ToListAsync());
        }


        // GET: Lines/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var line = await _context.Lines
                .FirstOrDefaultAsync(m => m.Id == id);
            if (line == null)
            {
                return NotFound();
            }

            return View(line);
        }


        // GET: Lines/Create
        // Restrict Create/Edit/Delete to Admins only

        [Authorize(Roles = "Admin")] 
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,DriverId,SupervisorId")] Line line)
        {
            if (ModelState.IsValid)
            {
                line.Id = Guid.NewGuid();
                _context.Add(line);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Line created successfully!"; // <--- Add this
                return RedirectToAction(nameof(Index));
            }
            return View(line);
        }

        // GET: Lines/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var line = await _context.Lines.FindAsync(id);
            if (line == null)
            {
                return NotFound();
            }
            return View(line);
        }

        // POST: Lines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,DriverId,SupervisorId")] Line line)
        {
            if (id != line.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(line);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LineExists(line.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Line updated successfully!"; // <--- Add this
                return RedirectToAction(nameof(Index));
            }

            return View(line);
        }

        // GET: Lines/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var line = await _context.Lines
                .FirstOrDefaultAsync(m => m.Id == id);
            if (line == null)
            {
                return NotFound();
            }

            return View(line);
        }

        // POST: Lines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {// NEW CODE: Load the line AND the bus so EF can unlink them
            var line = await _context.Lines
                .Include(l => l.Bus) // <--- CRITICAL FIX
                .FirstOrDefaultAsync(l => l.Id == id);

            if (line != null)
            {
                _context.Lines.Remove(line);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Line deleted successfully!"; // <--- Add this
            return RedirectToAction(nameof(Index));
        }

        private bool LineExists(Guid id)
        {
            return _context.Lines.Any(e => e.Id == id);
        }
        // GET: Lines/MyAssignment
        public async Task<IActionResult> MyAssignment()
        {
            // 1. Get the current user's ID as a string
            var userIdString = _userManager.GetUserId(User);

            // 2. Convert it to a Guid (because your Database uses Guids for DriverId)
            if (Guid.TryParse(userIdString, out Guid userGuid))
            {
                var myLine = await _context.Lines
                    .Include(l => l.Bus) // Optional: Include Bus details if needed
                    .FirstOrDefaultAsync(l => l.DriverId == userGuid);

                return View(myLine);
            }

            // 3. If something goes wrong (ID isn't a Guid), just show the list or an empty view
            return RedirectToAction(nameof(Index));
        }
    }
}
