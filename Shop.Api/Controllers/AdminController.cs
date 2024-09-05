using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Services;
using Shop.Models.CustomModels;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace Shop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService, IWebHostEnvironment env)
        {
            _adminService = adminService;
            _env = env;
        }

        [HttpPost]
        [Route("AdminLogin")]
        public IActionResult AdminLogin(LoginModel loginModel)
        {
            var response = _adminService.AdminLogin(loginModel);
            return Ok(response);
        }

        [HttpPost]
        [Route("SaveCategory")]
        public IActionResult SaveCategory(CategoryModel newcategory)
        {
            var data = _adminService.SaveCategory(newcategory);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetCategories")]
        public IActionResult GetCategories()
        {
            var data = _adminService.GetCategories();
            return Ok(data);
        }

        [HttpPost]
        [Route("UpdateCategory")]
        public IActionResult UpdateCategory(CategoryModel categoryToUpdate)
        {
            var data = _adminService.UpdateCategory(categoryToUpdate);
            return Ok(data);
        }

        [HttpPost]
        [Route("DeleteCategory")]
        public IActionResult DeleteCategory(CategoryModel categoryToDelete)
        {
            var data = _adminService.DeleteCategory(categoryToDelete);
            return Ok(data);
        }

        //product
        [HttpGet]
        [Route("GetProducts")]
        public IActionResult GetProducts()
        {
            var data = _adminService.GetProducts();
            return Ok(data);
        }

        [HttpPost]
        [Route("SaveProduct")]
        public IActionResult SaveProduct(ProductModel newproduct)
        {
            try
            {
                if (newproduct.FileContent != null && newproduct.FileContent.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}.png";
                    newproduct.ImageUrl = $"{fileName}";
                    var path = Path.Combine(_env.ContentRootPath, "Images", fileName);

                    using (var fs = System.IO.File.Create(path))
                    {
                        fs.Write(newproduct.FileContent, 0, newproduct.FileContent.Length);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(newproduct.ImageUrl))
                    {
                        newproduct.ImageUrl = "no-image.png";
                    }
                }

                var data = _adminService.SaveProduct(newproduct);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete]
        [Route("DeleteProduct/{Id:int}")]
        public IActionResult DeleteProduct(int Id)
        {
            var product = _adminService.GetProductById(Id);

            if (product == null)
            {
                return NotFound();
            }

            if (product.ImageUrl != "Images/no-image.png")
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), product.ImageUrl);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            var result = _adminService.DeleteProduct(Id);

            return Ok(result);
        }

        [HttpGet]
        [Route("GetProductStock")]
        public IActionResult GetProductStock()
        {
            var data = _adminService.GetProductStock();
            return Ok(data);
        }

        [HttpPost]
        [Route("UpdateProductStock")]
        public IActionResult UpdateProductStock(StockModel stock)
        {
            var data = _adminService.UpdateProductStock(stock);
            return Ok(data);
        }
    }
}

///23:27 / 37:30
///https://www.youtube.com/watch?v=NEtB0RVzPjA&list=PLWpvttYT5mY4Z_GJJULctGOFO-His_w1S&index=6