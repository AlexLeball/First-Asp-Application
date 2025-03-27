using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_5.Data;
using project_5.Models.Entities;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
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
            return RedirectToAction("Cars");
        }

        /// <summary>
        /// Afficher le formulaire pour éditer une voiture.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditCar(int id)
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

            // Créer un modèle de vue avec les informations de la voiture
            var model = new EditCarViewModel
            {
                Id = car.Id,
                BrandId = car.Brand.Id,
                CarModelId = car.CarModel.Id,
                Year = car.Year,
                SalePrice = car.SalePrice,
                UrlPhoto = car.UrlPhoto,
                // Ajoutez ici d'autres propriétés nécessaires pour l'édition
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
            // Vérifier si le modèle est valide
            if (ModelState.IsValid)
            {
                // Trouver la voiture par ID
                var car = await _context.Cars
                                        .Include(c => c.Brand)
                                        .Include(c => c.CarModel)
                                        .FirstOrDefaultAsync(c => c.Id == model.Id);

                // Si la voiture n'existe pas, retourner une erreur 404
                if (car == null)
                {
                    return NotFound();
                }

                // Mettre à jour les informations de la voiture
                car.Brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == model.BrandId);
                car.CarModel = await _context.CarModels.FirstOrDefaultAsync(m => m.Id == model.CarModelId);
                car.Year = model.Year;
                car.SalePrice = model.SalePrice;
                car.UrlPhoto = model.UrlPhoto;

                // Sauvegarder les changements dans la base de données
                await _context.SaveChangesAsync();

                // Rediriger vers la page de détails de la voiture ou vers la liste des voitures
                return RedirectToAction("CarDetails", new { id = car.Id });
            }

            // Si le modèle est invalide, retourner la vue avec les erreurs
            model.Brands = await _context.Brands.ToListAsync();
            model.CarModels = await _context.CarModels.ToListAsync();
            return View(model);
        }



    }
}
