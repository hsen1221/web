using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities;

namespace WebApplication1.Controllers
{
    public class StopsController : Controller
    {
        private readonly AppDbContext _context;

        public StopsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Stops
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Stops.Include(s => s.Line);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Stops/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stop = await _context.Stops
                .Include(s => s.Line)
                .FirstOrDefaultAsync(m => m.StopId == id);
            if (stop == null)
            {
                return NotFound();
            }

            return View(stop);
        }

        // GET: Stops/Create
        public IActionResult Create()
        {
            //old ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Id");
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title");

            return View();
        }

        // POST: Stops/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StopId,Title,LineId")] Stop stop)
        {
            if (ModelState.IsValid)
            {
                stop.StopId = Guid.NewGuid();
                _context.Add(stop);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Bus stop added successfully!"; // <--- Add this
                return RedirectToAction(nameof(Index));
            }
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title", stop.LineId);
            return View(stop);
        }

        // GET: Stops/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stop = await _context.Stops.FindAsync(id);
            if (stop == null)
            {
                return NotFound();
            }
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title", stop.LineId);
            return View(stop);
        }

        // POST: Stops/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("StopId,Title,LineId")] Stop stop)
        {
            if (id != stop.StopId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StopExists(stop.StopId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Bus stop updated successfully!"; // <--- Add this
                return RedirectToAction(nameof(Index));
            }
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title", stop.LineId);
            return View(stop);
        }

        // GET: Stops/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stop = await _context.Stops
                .Include(s => s.Line)
                .FirstOrDefaultAsync(m => m.StopId == id);
            if (stop == null)
            {
                return NotFound();
            }

            return View(stop);
        }

        // POST: Stops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var stop = await _context.Stops.FindAsync(id);
            if (stop != null)
            {
                _context.Stops.Remove(stop);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Bus stop deleted successfully!"; // <--- Add this
            return RedirectToAction(nameof(Index));
        }

        private bool StopExists(Guid id)
        {
            return _context.Stops.Any(e => e.StopId == id);
        }
    }
}
