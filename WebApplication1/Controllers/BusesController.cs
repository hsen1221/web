using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization; // Required namespace

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BusesController : Controller
    {

        private readonly AppDbContext _context;
        
        public BusesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Buses
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Buses.Include(b => b.Line);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Buses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bus = await _context.Buses
                .Include(b => b.Line)
                .FirstOrDefaultAsync(m => m.BusId == id);
            if (bus == null)
            {
                return NotFound();
            }

            return View(bus);
        }

        // GET: Buses/Create
        // GET: Buses/Create
        //public IActionResult Create()
        //{
        //    ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Id"); // <--- THE CULPRIT
        //    return View();
        //}
        // GET: Buses/Create
        public IActionResult Create()
        {
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title"); // <--- THE CULPRIT
            return View();
        }

        // POST: Buses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BusId,Capacity,LineId")] Bus bus)
        {
            // 1. THE CHECK: Does this line already have a bus?
            if (_context.Buses.Any(b => b.LineId == bus.LineId))
            {
                ModelState.AddModelError("LineId", "This line already ha    s a bus assigned. Please select a different line.");
            }

            if (ModelState.IsValid)
            {
                bus.BusId = Guid.NewGuid();
                _context.Add(bus);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Bus added successfully!"; // <--- Add this
                return RedirectToAction(nameof(Index));
            }

            // 2. THE FIX: Use "Title" here so it doesn't turn back into a GUID
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title", bus.LineId);
            return View(bus);
        }
        // GET: Buses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bus = await _context.Buses.FindAsync(id);
            if (bus == null)
            {
                return NotFound();
            }
            // Old
            //ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Id", bus.LineId);

            // New
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title", bus.LineId);
            return View(bus);
        }

        // POST: Buses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BusId,Capacity,LineId")] Bus bus)
        {
            if (id != bus.BusId)
            {
                return NotFound();
            }
            // Check if the Line is taken by a bus that is NOT this one
            if (_context.Buses.Any(b => b.LineId == bus.LineId && b.BusId != bus.BusId))
            {
                ModelState.AddModelError("LineId", "This line is already assigned to another bus.");
            }
            // <--- END CHECK

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusExists(bus.BusId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Bus updated successfully!"; // <--- Add this
                return RedirectToAction(nameof(Index));
            }
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Id", bus.LineId);
            return View(bus);
        }

        // GET: Buses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bus = await _context.Buses
                .Include(b => b.Line)
                .FirstOrDefaultAsync(m => m.BusId == id);
            if (bus == null)
            {
                return NotFound();
            }

            return View(bus);
        }

        // POST: Buses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var bus = await _context.Buses.FindAsync(id);
            if (bus != null)
            {
                _context.Buses.Remove(bus);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Bus deleted successfully!"; // <--- Add this
            return RedirectToAction(nameof(Index));
        }

        private bool BusExists(Guid id)
        {
            return _context.Buses.Any(e => e.BusId == id);
        }
    }
}
