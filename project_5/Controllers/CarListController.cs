using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_5.Data;
using project_5.Models.Entities;
using project_5.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace project_5.Controllers
{
    public class CarListController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarListController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Display the list of cars from the database. Links to the Cars view.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Cars()
        {
            var cars = await _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarModel)
                .ToListAsync();

            return View(cars);
        }

        /// <summary>
        /// Show details of a specific car.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CarDetails(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarModel)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        /// <summary>
        /// Show the form to add a new car.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AddCarForm()
        {
            var model = new AddCarViewModel
            {
                Brands = await _context.Brands.ToListAsync(),
                CarModels = await _context.CarModels.ToListAsync()
            };

            return View(model);
        }

        /// <summary>
        /// Add a new car and redirect to CarDetails.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddCar(AddCarViewModel model)
        {
            var carModel = await _context.CarModels.FirstOrDefaultAsync(s => s.Id == model.CarModelId);
            if (carModel == null)
            {
                model.Brands = await _context.Brands.ToListAsync();
                model.CarModels = await _context.CarModels.ToListAsync();
                return View("AddCar", model);
            }

            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == model.BrandId);
            if (brand == null)
            {
                model.Brands = await _context.Brands.ToListAsync();
                model.CarModels = await _context.CarModels.ToListAsync();
                return View("AddCar", model);
            }

            var newCar = new Car
            {
                Brand = brand,
                CarModel = carModel,
                Year = model.Year,
                SalePrice = model.SalePrice,
                UrlPhoto = model.UrlPhoto,
                PurchasePrice = 0,
                Finition = "Standard",
                AvailableDate = DateTime.Now,
                PurchaseDate = DateTime.MinValue,
                SaleDate = DateTime.Now
            };
            _context.Cars.Add(newCar);
            await _context.SaveChangesAsync();

            return RedirectToAction("CarDetails", new { id = newCar.Id });
        }

        /// <summary>
        /// Add a new brand.
        /// </summary>
        [HttpPost]
        public IActionResult AddBrand([FromBody] AddBrandViewModel model)
        {
            if (_context.Brands.Any(b => b.Name == model.BrandName))
            {
                return Conflict("Brand already exists.");
            }

            var brand = new Brand { Name = model.BrandName };
            _context.Brands.Add(brand);
            _context.SaveChanges();

            return Ok(new { id = brand.Id, name = brand.Name });
        }

        /// <summary>
        /// Add a new car model.
        /// </summary>
        [HttpPost]
        public IActionResult AddCarModel([FromBody] AddCarModelViewModel model)
        {
            if (_context.CarModels.Any(m => m.Model == model.ModelName))
            {
                return Conflict("Model already exists.");
            }

            var carModel = new CarModel { Model = model.ModelName };
            _context.CarModels.Add(carModel);
            _context.SaveChanges();

            return Ok(new { id = carModel.Id, name = carModel.Model });
        }
    }
}
