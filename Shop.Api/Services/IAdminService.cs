using Shop.Models.CustomModels;
using Shop.Models.Models;

namespace Shop.Api.Services
{
    public interface IAdminService
    {
        ResponseModel AdminLogin(LoginModel loginModel);
        CategoryModel SaveCategory(CategoryModel newcategory);
        ProductModel SaveProduct(ProductModel newproduct);
        List<CategoryModel> GetCategories();
        List<ProductModel> GetProducts();
        bool UpdateCategory(CategoryModel categoryToUpdate);
        bool DeleteCategory(CategoryModel categoryToDelete);
        bool DeleteProduct(int Id);
        Product GetProductById(int id);
        List<StockModel> GetProductStock();
        bool UpdateProductStock(StockModel stock);
    }
}

//https://www.youtube.com/watch?v=Ckkdv8sywto&list=PLWpvttYT5mY4Z_GJJULctGOFO-His_w1S&index=6
//13:59 / 19:51