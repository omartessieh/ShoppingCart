using Shop.Models.CustomModels;
using System.Text.Json;
using Blazored.LocalStorage;
using System.Net.Http.Json;
using Shop.Models.Models;
namespace Shop.Client.Services
{
    public class ClientPanelService : IClientPanelService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService localStorage;

        public ClientPanelService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CategoryModel>> GetCategories()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/user/GetCategories");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var categories = JsonSerializer.Deserialize<List<CategoryModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return categories;
            }
            catch (Exception e)
            {
                return new List<CategoryModel>();
            }
        }

        public async Task<List<ProductModel>> GetProductByCategoryId(int categoryId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/user/GetProductByCategoryId?categoryId={categoryId}");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var products = JsonSerializer.Deserialize<List<ProductModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return products ?? new List<ProductModel>();
            }
            catch (Exception e)
            {
                return new List<ProductModel>();
            }
        }

        public async Task<ResponseModel> RegisterUser(RegisterModel registerModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/user/RegisterUser", registerModel);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<ResponseModel>();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        public async Task<ResponseModel> LoginUser(LoginModel loginModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/user/LoginUser", loginModel);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ResponseModel>();
                }
                else
                {
                    throw new ApplicationException($"HTTP Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        public async Task<ResponseModel> Checkout(List<CartModel> cartItems)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/user/Checkout", cartItems);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ResponseModel>();
                }
                else
                {
                    throw new ApplicationException($"HTTP Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        public async Task<List<CustomerOrder>> GetOrdersByCustomerId(int customerId)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/user/GetOrdersByCustomerId/?customerId=" + customerId);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var customerOrder = JsonSerializer.Deserialize<List<CustomerOrder>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return customerOrder;
            }
            catch (Exception e)
            {
                return new List<CustomerOrder>();
            }
        }

        public async Task<List<CartModel>> GetOrderDetailForCustomer(int customerId, string orderNumber)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<CartModel>>($"api/user/GetOrderDetailForCustomer?customerId={customerId}&order_number={orderNumber}");
                return response ?? new List<CartModel>();
            }
            catch (HttpRequestException ex)
            {
                return new List<CartModel>();
            }
        }

        public async Task<List<string>> GetShippingStatusForOrder(string orderNumber)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<string>>($"api/user/GetShippingStatusForOrder?order_number={orderNumber}");
                return response ?? new List<string>();
            }
            catch (HttpRequestException ex)
            {
                return new List<string>();
            }
        }

        public async Task<ResponseModel> ChangePassword(PasswordModel passwordModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/user/ChangePassword", passwordModel);

                response.EnsureSuccessStatusCode();

                var responseModel = await response.Content.ReadFromJsonAsync<ResponseModel>();
                return responseModel;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return new ResponseModel
                {
                    Status = false,
                    Message = "An error occurred while changing the password."
                };
            }
        }
    }
}