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
    public class LinePassengersController : Controller
    {
        private readonly AppDbContext _context;

        public LinePassengersController(AppDbContext context)
        {
            _context = context;
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // ADD "FullName" to this list 👇
        public async Task<IActionResult> Create([Bind("Id,FullName,LineId,RegisteredDate,IsActive")] LinePassenger linePassenger)
        {
            if (ModelState.IsValid)
            {
                linePassenger.Id = Guid.NewGuid();
                _context.Add(linePassenger);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Optional: Fix the dropdown bug here too (change "Id" to "Title")
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ViewData["LineId"] = new SelectList(_context.Lines, "Id", "Title    ", linePassenger.LineId);
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
    }
}
