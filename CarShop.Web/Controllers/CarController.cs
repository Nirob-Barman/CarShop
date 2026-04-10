using CarShop.Application.DTOs.Car;
using CarShop.Application.DTOs.File;
using CarShop.Application.Interfaces;
using CarShop.Web.ViewModels.Car;
using CarShop.Web.ViewModels.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CarController : Controller
    {
        private readonly ICarService _carService;
        private readonly IBrandService _brandService;
        private readonly ICommentService _commentService;
        private readonly IWishlistService _wishlistService;
        private readonly IStockAlertService _stockAlertService;

        public CarController(
            ICarService carService,
            IBrandService brandService,
            ICommentService commentService,
            IWishlistService wishlistService,
            IStockAlertService stockAlertService)
        {
            _carService = carService;
            _brandService = brandService;
            _commentService = commentService;
            _wishlistService = wishlistService;
            _stockAlertService = stockAlertService;
        }
        
        public async Task<IActionResult> Index()
        {
            var result = await _carService.GetAllCarsAsync();
            if (!result.Success)
            {
                //TempData["ErrorMessage"] = result.Message ?? "Failed to load cars.";
                return View(new List<CarViewModel>());
            }
            var vm = CarMapper.ToViewModels(result.Data!);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var brandResult = await _brandService.GetAllBrandsAsync();
            ViewBag.Brands = BrandMapper.ToViewModels(brandResult.Data!);
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(CarViewModel model, IFormFile image)
        {
            var brandResult = await _brandService.GetAllBrandsAsync();            
            ViewBag.Brands = BrandMapper.ToViewModels(brandResult.Data!);

            if (!ModelState.IsValid)
                return View(model);

            FileUploadDto? fileDto = null;
            if (image != null && image.Length > 0)
            {
                fileDto = new FileUploadDto
                {
                    Content = image.OpenReadStream(),
                    FileName = image.FileName,
                    ContentType = image.ContentType,
                    Size = image.Length
                };
            }

            var dto = CarMapper.ToDto(model);
            var result = await _carService.CreateCarAsync(dto, fileDto);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }

            //TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var carResult = await _carService.GetCarByIdAsync(id);
            var brandResult = await _brandService.GetAllBrandsAsync();

            //if (!carResult.Success || carResult.Data == null)
            //{
            //    TempData["ErrorMessage"] = carResult.Message ?? "Car not found.";
            //    return RedirectToAction("Index");
            //}

            ViewBag.Brands = BrandMapper.ToViewModels(brandResult.Data!);
            var vm = CarMapper.ToViewModel(carResult.Data!);
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, CarViewModel model, IFormFile? image)
        {
            var brandResult = await _brandService.GetAllBrandsAsync();
            ViewBag.Brands = brandResult.Data;

            if (!ModelState.IsValid)
                return View(model);

            var existingCarResult = await _carService.GetCarByIdAsync(id);
            if (!existingCarResult.Success || existingCarResult.Data == null)
            {
                TempData["ErrorMessage"] = existingCarResult.Message ?? "Car not found.";
                return RedirectToAction("Index");
            }

            var existingCar = existingCarResult.Data;

            FileUploadDto? fileDto = null;

            if (image != null && image.Length > 0)
            {
                fileDto = new FileUploadDto
                {
                    Content = image.OpenReadStream(),
                    FileName = image.FileName,
                    ContentType = image.ContentType,
                    Size = image.Length
                };
            }

            var dto = CarMapper.ToDto(model);
            var result = await _carService.UpdateCarAsync(id, dto, fileDto);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return View(model);
            }

            //TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var carResult = await _carService.GetCarByIdAsync(id);
            if (!carResult.Success || carResult.Data == null)
            {
                TempData["ErrorMessage"] = carResult.Message ?? "Car not found.";
                return RedirectToAction("Index");
            }

            var deleteResult = await _carService.DeleteCarAsync(id);
            //if (!deleteResult.Success)
            //    TempData["ErrorMessage"] = deleteResult.Message;
            //else
            //    TempData["SuccessMessage"] = deleteResult.Message;

            return RedirectToAction("Index");
        }

        
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var carResult = await _carService.GetCarByIdAsync(id);
            if (!carResult.Success || carResult.Data == null)
            {
                TempData["ErrorMessage"] = carResult.Message ?? "Car not found.";
                return RedirectToAction("AllCars");
            }

            // Track recently viewed (cookie — up to 8 IDs, 30-day expiry)
            const string cookieName = "RecentlyViewed";
            var existing = Request.Cookies[cookieName] ?? "";
            var ids = existing.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Where(x => int.TryParse(x, out _))
                              .Select(int.Parse)
                              .Where(x => x != id)
                              .Prepend(id)
                              .Take(8)
                              .ToList();
            Response.Cookies.Append(cookieName, string.Join(',', ids), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                IsEssential = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
            });

            var vm = CarMapper.ToViewModel(carResult.Data);

            var commentResult = await _commentService.GetCommentsByCarIdAsync(id);
            ViewBag.Comments = commentResult.Success ? commentResult.Data
                : new List<CarShop.Application.DTOs.Comment.CommentDto>();

            var avgResult = await _commentService.GetAverageRatingAsync(id);
            ViewBag.AverageRating = avgResult.Data;

            if (User.Identity?.IsAuthenticated == true)
            {
                var wishResult  = await _wishlistService.IsInWishlistAsync(id);
                ViewBag.IsInWishlist = wishResult.Data;

                var alertResult = await _stockAlertService.GetUserAlertsAsync();
                ViewBag.IsStockAlerted = alertResult.Data?.Any(a => a.CarId == id) ?? false;

                var reviewedResult = await _commentService.HasUserReviewedAsync(id);
                ViewBag.HasReviewed = reviewedResult.Data;
            }

            return View(vm);
        }



        [AllowAnonymous]
        public async Task<IActionResult> AllCars(string? brandName, string? keyword, decimal? minPrice, decimal? maxPrice, string? sortBy, int page = 1)
        {
            var searchDto = new CarShop.Application.DTOs.Car.CarSearchDto
            {
                BrandName = brandName,
                Keyword = keyword,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sortBy ?? "newest",
                Page = page,
                PageSize = 10
            };

            var result = await _carService.SearchCarsAsync(searchDto);
            var brandList = await _brandService.GetAllBrandsAsync();

            ViewBag.Brands = BrandMapper.ToViewModels(brandList.Data!);
            ViewBag.TotalPages = result.Data?.TotalPages ?? 1;
            ViewBag.CurrentPage = page;
            ViewBag.SearchKeyword = keyword;
            ViewBag.SelectedBrand = brandName;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortBy = sortBy ?? "newest";

            var cars = CarMapper.ToViewModels(result.Data?.Items ?? Enumerable.Empty<CarDto>());
            return View(cars);
        }
    }
}
