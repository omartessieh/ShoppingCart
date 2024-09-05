using Microsoft.EntityFrameworkCore;
using Shop.Api.Data;
using Shop.Models.CustomModels;
using Shop.Models.Models;

namespace Shop.Api.Services
{
    public class AdminService : IAdminService
    {
        private readonly ShoppingCartDBContext _dbContext = null;

        public AdminService(ShoppingCartDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResponseModel AdminLogin(LoginModel loginModel)
        {
            ResponseModel responseModel = new ResponseModel();

            try
            {
                var data = _dbContext.AdminInfos.Where(x => x.Email == loginModel.Email && x.Password == loginModel.Password).FirstOrDefault();

                if (data != null)
                {
                    if (data.Password == loginModel.Password)
                    {
                        responseModel.Status = true;
                        responseModel.Message = Convert.ToString(data.Id) + "|" + data.Name + "|" + data.Email;
                    }
                    else
                    {
                        responseModel.Status = false;
                        responseModel.Message = "Your Password is Incorrect";
                    }
                }
                else
                {
                    responseModel.Status = false;
                    responseModel.Message = "Email not registered. Please check your Email Id";
                }
                return responseModel;
            }
            catch (Exception ex)
            {
                responseModel.Status = false;
                responseModel.Message = "An Error has occured. Please try again!";

                return responseModel;
            }
        }

        public Product GetProductById(int id)
        {
            return _dbContext.Products.Find(id);
        }

        public CategoryModel SaveCategory(CategoryModel newcategory)
        {
            try
            {
                Category _category = new Category();
                _category.Name = newcategory.Name;
                _dbContext.Add(_category);
                _dbContext.SaveChanges();
                return newcategory;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ProductModel SaveProduct(ProductModel newProduct)
        {
            try
            {
                Product _product = new Product();
                _product.Name = newProduct.Name;
                _product.Price = newProduct.Price.Value;
                _product.ImageUrl = newProduct.ImageUrl;
                _product.CategoryId = newProduct.CategoryId.Value;
                _product.Stock = newProduct.Stock.Value;

                _dbContext.Products.Add(_product);
                _dbContext.SaveChanges();

                return newProduct;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<CategoryModel> GetCategories()
        {
            var data = _dbContext.Categories.ToList();
            List<CategoryModel> _categoryList = new List<CategoryModel>();

            foreach (var c in data)
            {
                CategoryModel _categoryModel = new CategoryModel();
                _categoryModel.Id = c.Id;
                _categoryModel.Name = c.Name;
                _categoryList.Add(_categoryModel);
            }
            return _categoryList;
        }

        public bool UpdateCategory(CategoryModel categoryToUpdate)
        {
            bool flag = false;
            var _category = _dbContext.Categories.Where(x => x.Id == categoryToUpdate.Id).First();
            if (_category != null)
            {
                _category.Name = categoryToUpdate.Name;
                _dbContext.Categories.Update(_category);
                _dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }

        public bool DeleteCategory(CategoryModel categoryToDelete)
        {
            bool flag = false;
            var _category = _dbContext.Categories.Where(x => x.Id == categoryToDelete.Id).First();
            if (_category != null)
            {
                _dbContext.Categories.Remove(_category);
                _dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }

        public List<ProductModel> GetProducts()
        {
            List<Category> categoryData = _dbContext.Categories.ToList();
            List<Product> productData = _dbContext.Products.ToList();
            List<ProductModel> _productList = new List<ProductModel>();

            foreach (var p in productData)
            {
                ProductModel _productModel = new ProductModel();
                _productModel.Id = p.Id;
                _productModel.Name = p.Name;
                _productModel.Price = p.Price;
                _productModel.Stock = p.Stock;
                _productModel.ImageUrl = p.ImageUrl;
                _productModel.CategoryId = p.CategoryId;
                _productModel.CategoryName = categoryData.Where(x => x.Id == p.CategoryId).Select(x => x.Name).FirstOrDefault();
                _productList.Add(_productModel);
            }
            return _productList;
        }

        public bool DeleteProduct(int Id)
        {
            bool flag = false;
            var _product = _dbContext.Products.Where(x => x.Id == Id).First();
            if (_product != null)
            {
                _dbContext.Products.Remove(_product);
                _dbContext.SaveChanges();
                flag = true;
            }
            return flag;
        }

        public List<StockModel> GetProductStock()
        {
            List<StockModel> productStock = new List<StockModel>();
            List<Category> categoryData = _dbContext.Categories.ToList();
            List<Product> productData = _dbContext.Products.ToList();

            foreach (var p in productData)
            {
                StockModel _productModel = new StockModel();
                _productModel.Id = p.Id;
                _productModel.Name = p.Name;
                _productModel.Stock = p.Stock;
                _productModel.CategoryName = categoryData.Where(x => x.Id == p.CategoryId).Select(x => x.Name).FirstOrDefault();
                productStock.Add(_productModel);
            }

            return productStock;
        }

        public bool UpdateProductStock(StockModel stock)
        {
            bool flag = false;
            var _product = _dbContext.Products.Where(x => x.Id == stock.Id).First();

            if (_product != null)
            {
                _product.Stock = (int)(stock.Stock + stock.NewStock);
                _dbContext.Products.Update(_product);
                _dbContext.SaveChanges();
                flag = true;
            }

            return flag;
        }
    }
}