using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_5.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using project_5.Models.Entities;
using project_5.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;

namespace project_5.Controllers
{
    public class CarListController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CarListController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarListController(ApplicationDbContext context, ILogger<CarListController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCar(AddCarViewModel model)
        {
            // Check if the selected CarModel exists
            var carModel = await _context.CarModels.FirstOrDefaultAsync(s => s.Id == model.CarModelId);
            if (carModel == null)
            {
                model.Brands = await _context.Brands.ToListAsync();
                model.CarModels = await _context.CarModels.ToListAsync();
                model.Repairs = await _context.Repairs.ToListAsync();
                return View("AddCarForm", model);
            }

            // Check if the selected Brand exists
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == model.BrandId);
            if (brand == null)
            {
                model.Brands = await _context.Brands.ToListAsync();
                model.CarModels = await _context.CarModels.ToListAsync();
                model.Repairs = await _context.Repairs.ToListAsync();
                return View("AddCarForm", model);
            }

            // Check if a car with the same Brand, Model, and Year already exists
            var existingCar = await _context.Cars
                .Include(c => c.Brand)
                .Include(c => c.CarModel)
                .FirstOrDefaultAsync(c => c.BrandId == model.BrandId && c.CarModelId == model.CarModelId && c.Year == model.Year);

            if (existingCar != null)
            {
                ModelState.AddModelError(string.Empty, "A car with the same brand, model, and year already exists.");
                model.Brands = await _context.Brands.ToListAsync();
                model.CarModels = await _context.CarModels.ToListAsync();
                model.Repairs = await _context.Repairs.ToListAsync();
                return View("AddCarForm", model);
            }

            if (model.CarPhotoFile != null && model.CarPhotoFile.Length > 0)
            {
                // Generate a unique filename
                var fileExtension = Path.GetExtension(model.CarPhotoFile.FileName);
                var fileName = Guid.NewGuid().ToString() + fileExtension;

                // Define the path where you want to store the image
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                // Ensure directory exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Combine the upload path with the file name
                var filePath = Path.Combine(uploadPath, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CarPhotoFile.CopyToAsync(stream);
                }

                // Store the relative path to the database
                model.UrlPhoto = "/uploads/" + fileName; // This will be stored in the database
            }
            else
            {
                ModelState.AddModelError("CarPhotoFile", "Please upload a car photo.");
            }


            // Create a new car object and populate it (without SalePrice)
            var newCar = new Car
            {
                Brand = brand,
                CarModel = carModel,
                Year = model.Year,
                PurchasePrice = model.PurchasePrice,  // Use the PurchasePrice entered in the form
                UrlPhoto = model.UrlPhoto,
                AvailableDate = model.AvailableDate,
                Finition = model.Finition,
                Repairs = new List<Repair>()  // Initialize the Repairs collection
            };

            // Add the car to the database
            _context.Cars.Add(newCar);
            await _context.SaveChangesAsync();  // Save the car to get its Id

            // Now that the car has been saved, associate repairs with the new car
            if (model.Repairs != null && model.Repairs.Any())
            {
                foreach (var repair in model.Repairs)
                {
                    repair.CarId = newCar.Id;  // Set the CarId for each repair
                    _context.Repairs.Add(repair);  // Add repairs to the context
                }
            }

            // Save repairs to the database
            await _context.SaveChangesAsync();  // Save both the new car and repairs

            // Calculate SalePrice (PurchasePrice + total repair price + 500)
            decimal totalRepairPrice = newCar.Repairs?.Sum(r => r.Price) ?? 0;
            decimal salePrice = newCar.PurchasePrice + totalRepairPrice + 500;

            // Update the SalePrice of the car
            newCar.SalePrice = salePrice;
            _context.Cars.Update(newCar);  // Update the car with the SalePrice
            await _context.SaveChangesAsync();  // Save the updated car

            // Redirect to the car details page after successfully adding the car
            return RedirectToAction("CarAdded", "CarList");
        }


        /// <summary>
        /// Add a new brand.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddBrand([FromBody] AddBrandViewModel model)
        {
            if (_context.Brands.Any(b => b.Name == model.BrandName))
            {
                return Conflict("Brand already exists.");
            }

            var brand = new Brand { Name = model.BrandName };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            // Return the updated list of brands to update the dropdown dynamically
            var brands = await _context.Brands.ToListAsync();
            return Json(new { success = true, brands = brands });
        }


        /// <summary>
        /// Add a new car model.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddCarModel([FromBody] AddCarModelViewModel model)
        {
            if (_context.CarModels.Any(m => m.Model == model.ModelName))
            {
                return Conflict("Model already exists.");
            }

            var carModel = new CarModel { Model = model.ModelName };
            _context.CarModels.Add(carModel);
            await _context.SaveChangesAsync();

            // Return the updated list of car models to update the dropdown dynamically
            var carModels = await _context.CarModels.ToListAsync();
            return Json(new { success = true, carModels = carModels });
        }


        /// <summary>
        /// delete a car.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            // Trouver la voiture par ID
            var car = await _context.Cars
                                    .Include(c => c.Brand)
                                    .Include(c => c.CarModel)
                                    .FirstOrDefaultAsync(c => c.Id == id);

            // Si la voiture n'existe pas, retourner une erreur 404
            if (car == null)
            {
                return NotFound();
            }

            // Supprimer la voiture de la base de données
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            // Rediriger vers la liste des voitures après la suppression
            return RedirectToAction("CarDeleted", "CarList");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditCar(int id)
        {
            // Fetch the car along with the associated Brand and CarModel
            var car = await _context.Cars
                                    .Include(c => c.Brand)  // Include the Brand navigation property
                                    .Include(c => c.CarModel)  // Include the CarModel navigation property
                                    .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            // Create a ViewModel for the form
            var model = new EditCarViewModel
            {
                Id = car.Id,
                BrandId = car.Brand.Id, // Pass the current BrandId to the view
                CarModelId = car.CarModel.Id,
                Year = car.Year,
                SalePrice = car.SalePrice,
                UrlPhoto = car.UrlPhoto,

                // Pass a list of available brands to the view
                Brands = await _context.Brands.ToListAsync(),
                CarModels = await _context.CarModels.ToListAsync()
            };

            return View(model);
        }

        /// <summary>
        /// Mettre à jour les informations d'une voiture.
        /// </summary>
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditCar(EditCarViewModel model)
        {
            _logger.LogInformation("EditCar POST request started for CarId: {CarId}", model.Id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model is invalid. Returning to the view.");
                model.Brands = await _context.Brands.ToListAsync();
                model.CarModels = await _context.CarModels.ToListAsync();
                return View(model);
            }

            var car = await _context.Cars
                                    .Include(c => c.Brand)
                                    .Include(c => c.CarModel)
                                    .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (car == null)
            {
                return NotFound();
            }

            // Update editable fields (keep old values if unchanged)
            car.BrandId = model.BrandId;
            car.CarModelId = model.CarModelId;
            car.Year = model.Year;
            car.SalePrice = model.SalePrice;
            car.Finition = model.Finition;

            // Optional photo upload (same logic as AddCar)
            if (model.CarPhotoFile != null && model.CarPhotoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileExtension = Path.GetExtension(model.CarPhotoFile.FileName);
                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CarPhotoFile.CopyToAsync(fileStream);
                }

                car.UrlPhoto = "/uploads/" + uniqueFileName;
            }

            _context.Entry(car).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction("CarDetails", new { id = car.Id });
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkCarAsSold(int Id, DateTime SaleDate)
        {
            var car = await _context.Cars.FindAsync(Id);
            if (car == null)
            {
                return NotFound();
            }

            car.SaleDate = SaleDate;
            await _context.SaveChangesAsync();

            return RedirectToAction("CarDetails", new { id = Id });
        }


        public IActionResult CarAdded()
        {
            return View("CarAdded", "CarList"); // Cela doit retourner la vue "CarAdded.cshtml"
        }

        public IActionResult CarDeleted()
        {
            return View("CarDeleted", "CarList"); // Cela doit retourner la vue "CarDeleted.cshtml"
        }
    }
}
