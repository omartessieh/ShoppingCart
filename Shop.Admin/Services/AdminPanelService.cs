using Azure;
using Shop.Models.CustomModels;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Shop.Admin.Services
{
    public class AdminPanelService : IAdminPanelService
    {
        private readonly HttpClient _httpClient;

        public AdminPanelService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseModel> AdminLogin(LoginModel loginModel)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/admin/AdminLogin", loginModel);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(responseContent))
                    {
                        return new ResponseModel { Status = false, Message = "Empty response from server" };
                    }

                    return JsonSerializer.Deserialize<ResponseModel>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    return new ResponseModel { Status = false, Message = "Login failed" };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel { Status = false, Message = "An error occurred. Please try again." };
            }
        }

        public async Task<List<CategoryModel>> GetCategories()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admin/GetCategories");

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

        public async Task<List<ProductModel>> GetProducts()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admin/GetProducts");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var products = JsonSerializer.Deserialize<List<ProductModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return products;
            }
            catch (Exception e)
            {
                return new List<ProductModel>();
            }
        }

        public async Task<CategoryModel> SaveCategory(CategoryModel newcategory)
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/SaveCategory", newcategory);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CategoryModel>();
        }

        public async Task<ProductModel> SaveProduct(ProductModel newproduct)
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/SaveProduct", newproduct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductModel>();
        }

        public async Task<bool> UpdateCategory(CategoryModel categoryToUpdate)
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/UpdateCategory", categoryToUpdate);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<bool>();
                return result;
            }
            return false;
        }

        public async Task<bool> DeleteCategory(CategoryModel categoryToDelete)
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/DeleteCategory", categoryToDelete);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<bool>();
                return result;
            }
            return false;
        }

        public async Task<bool> DeleteProduct(int Id)
        {
            var response = await _httpClient.DeleteAsync($"api/admin/DeleteProduct/{Id}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<bool>();
                return result;
            }
            return false;
        }

        public async Task<List<StockModel>> GetProductStock()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/admin/GetProductStock");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var stocks = JsonSerializer.Deserialize<List<StockModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return stocks;
            }
            catch (Exception e)
            {
                return new List<StockModel>();
            }
        }

        public async Task<bool> UpdateProductStock(StockModel stock)
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/UpdateProductStock", stock);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<bool>();
            return result;
        }
    }
}