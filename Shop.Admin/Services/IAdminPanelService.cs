using Shop.Models.CustomModels;

namespace Shop.Admin.Services
{
    public interface IAdminPanelService
    {
        Task<ResponseModel> AdminLogin(LoginModel loginModel);
        Task<List<CategoryModel>> GetCategories();
        Task<List<ProductModel>> GetProducts();
        Task<CategoryModel> SaveCategory(CategoryModel newcategory);
        Task<ProductModel> SaveProduct(ProductModel newproduct);
        Task<bool> UpdateCategory(CategoryModel categoryToUpdate);
        Task<bool> DeleteCategory(CategoryModel categoryToDelete);
        Task<bool> DeleteProduct(int Id);
        Task<List<StockModel>> GetProductStock();
        Task<bool> UpdateProductStock(StockModel stock);
    }
}