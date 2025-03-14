using Microsoft.AspNetCore.Mvc;
using project_5.Models.Entities;
using System.Collections.Generic;
using System.Linq;

namespace project_5.Controllers
{
    public class CarListController : Controller
    {
        // Static car list to persist while app is running
        private static List<Car> cars = new List<Car>
        {
            new Car
            {
                Id = 1,
                Brand = new Brand { Id = 1, Name = "Toyota" },
                CarModel = new CarModel { Id = 1, Model = "Corolla" },
                Year = 2020,
                UrlPhoto = "https://ligierautomotive.com/wp-content/uploads/2017/12/concept-car.jpg",
                Finition = "Red"
            },
            new Car
            {
                Id = 2,
                Brand = new Brand { Id = 2, Name = "Honda" },
                CarModel = new CarModel { Id = 2, Model = "Civic" },
                Year = 2022,
                UrlPhoto = "https://ligierautomotive.com/wp-content/uploads/2017/12/concept-car.jpg",
                Finition = "Black"
            },
            new Car
            {
                Id = 3,
                Brand = new Brand { Id = 3, Name = "BMW" },
                CarModel = new CarModel { Id = 3, Model = "M3" },
                Year = 2021,
                UrlPhoto = "https://ligierautomotive.com/wp-content/uploads/2017/12/concept-car.jpg",
                Finition = "Blue"
            }
        };

        // Display the list of cars
        [HttpGet]
        public IActionResult Cars()
        {
            return View(cars);
        }

        // Show details of a specific car
        [HttpGet]
        public IActionResult CarDetails(int id)
        {
            var car = cars.FirstOrDefault(c => c.Id == id);

            if (car == null)
            {
                return NotFound(); // Return a 404 error if the car is not found
            }

            return View(car);
        }

        // Show form to add a new car
        [HttpGet]
        public IActionResult AddCar()
        {
            return View();
        }

        // Handle form submission for adding a new car
        [HttpPost]
        public IActionResult AddCar(Car newCar)
        {
            if (ModelState.IsValid)
            {
                newCar.Id = cars.Count + 1; // Assign a new ID
                cars.Add(newCar);
                return RedirectToAction("Cars"); // Redirect to the car list
            }

            return View(newCar); // Return the form with validation errors
        }
    }
}
