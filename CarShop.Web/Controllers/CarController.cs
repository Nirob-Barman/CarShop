﻿using CarShop.Application.DTOs.Car;
using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        private readonly IBrandService _brandService;
        public CarController(ICarService carService, IBrandService brandService)
        {
            _carService = carService;
            _brandService = brandService;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _carService.GetAllCarsAsync();
            return View(cars);
        }
        
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = await _brandService.GetAllBrandsAsync();
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(CarDto model, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = await _brandService.GetAllBrandsAsync();
                return View(model);
            }

            if (image != null && image.Length > 0)
            {
                var titleFileSafe = model.Title!.ToLowerInvariant().Replace(" ", "_");

                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "car");
                if (!Directory.Exists(uploadsRoot))
                {
                    Directory.CreateDirectory(uploadsRoot);
                }
                
                string fileName = $"{titleFileSafe}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Optionally: store relative path in model (e.g., for use in views or saving in DB)
                model.ImageUrl = Path.Combine("/uploads", "car", fileName).Replace("\\", "/");
            }

            await _carService.CreateCarAsync(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            ViewBag.Brands = await _brandService.GetAllBrandsAsync();
            return View(car);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CarDto model, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Brands = await _brandService.GetAllBrandsAsync();
                return View(model);
            }


            var car = await _carService.GetCarByIdAsync(id);

            // Check if there is a new image uploaded and if the car already has an image
            if (image != null && image.Length > 0)
            {
                // Remove the previous image if it exists
                if (!string.IsNullOrEmpty(car.ImageUrl))
                {
                    var previousImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", car.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(previousImagePath))
                    {
                        System.IO.File.Delete(previousImagePath);
                    }
                }

                // Process and save the new image
                var titleFileSafe = model.Title!.ToLowerInvariant().Replace(" ", "_");

                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "car");
                if (!Directory.Exists(uploadsRoot))
                {
                    Directory.CreateDirectory(uploadsRoot);
                }

                string fileName = $"{titleFileSafe}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Update the ImageUrl in the model
                model.ImageUrl = Path.Combine("/uploads", "car", fileName).Replace("\\", "/");
            }
            else
            {
                // If no new image was uploaded, retain the existing imageUrl
                model.ImageUrl = car.ImageUrl;
            }


            await _carService.UpdateCarAsync(id, model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);

            if (!string.IsNullOrEmpty(car.ImageUrl))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", car.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            await _carService.DeleteCarAsync(id);
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            return View(car);
        }

        [AllowAnonymous]
        public async Task<IActionResult> AllCars(string? brandName, int page = 1)
        {
            int pageSize = 10;
            int? brandId = null;

            if (!string.IsNullOrWhiteSpace(brandName))
            {
                var brand = await _brandService.GetBrandByNameAsync(brandName);
                if (brand != null)
                {
                    brandId = brand.Id;
                    ViewBag.SelectedBrand = brand.Name;
                }
            }

            var allCars = brandId.HasValue
                ? await _carService.GetCarsByBrandIdAsync(brandId.Value)
                : await _carService.GetAllCarsAsync();

            int totalCars = allCars.Count();
            var paginatedCars = allCars
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCars / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.Brands = await _brandService.GetAllBrandsAsync();

            return View(paginatedCars);
        }

    }
}
