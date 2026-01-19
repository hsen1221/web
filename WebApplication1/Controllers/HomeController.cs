using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using DataAccess;
using DataAccess.Entities;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;

        public HomeController(ILogger<HomeController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Load lines with related data
            var lines = await _db.Lines
                .Include(l => l.Bus)
                .Include(l => l.Stops)
                .Include(l => l.LinePassengers)
                .ToListAsync();

            // 2. FIX: Fetch Driver Names manually so the View can display them
            var driverIds = lines.Select(l => l.DriverId.ToString()).ToList();

            var drivers = await _db.Users
                .Where(u => driverIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.FullName ?? u.Email);

            // 3. Pass this dictionary to the View
            ViewBag.DriverNames = drivers;

            return View(lines);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}