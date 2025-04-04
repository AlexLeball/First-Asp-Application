using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_5.Data;
using project_5.Models.Entities;
using project_5.Models.ViewModels;

[Authorize(Roles = "Admin")] // Optional: restrict access
public class RepairsController : Controller
{
    private readonly ApplicationDbContext _context;

    public RepairsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Repairs/Create
    public async Task<IActionResult> Create(int carId)
    {
        var car = await _context.Cars.FindAsync(carId);
        if (car == null) return NotFound();

        var model = new RepairViewModel
        {
            CarId = carId,
        };
        return View(model);
    }

    // POST: Repairs/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RepairViewModel model)
    {
        if (ModelState.IsValid)
        {
            var repair = new Repair
            {
                CarId = model.CarId,
                Price = model.Price,
                Description = model.Description
            };

            _context.Repairs.Add(repair);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Cars", new { id = model.CarId }); // Redirect to Car Details page
        }

        return View(model);
    }
}
