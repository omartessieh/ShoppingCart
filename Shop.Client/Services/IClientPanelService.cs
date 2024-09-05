using Shop.Models.CustomModels;
using Shop.Models.Models;

namespace Shop.Client.Services
{
    public interface IClientPanelService
    {
        Task<List<CategoryModel>> GetCategories();
        Task<List<ProductModel>> GetProductByCategoryId(int categoryId);
        Task<ResponseModel> RegisterUser(RegisterModel registerModel);
        Task<ResponseModel> LoginUser(LoginModel loginModel);
        Task<ResponseModel> Checkout(List<CartModel> cartItems);
        Task<List<CustomerOrder>> GetOrdersByCustomerId(int customerId);
        Task<List<string>> GetShippingStatusForOrder(string order_number);
        Task<List<CartModel>> GetOrderDetailForCustomer(int customerId, string orderNumber);
        Task<ResponseModel> ChangePassword(PasswordModel passwordModel);
    }
}