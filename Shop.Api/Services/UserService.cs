using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shop.Api.Data;
using Shop.Models.CustomModels;
using Shop.Models.Models;
using Stripe;

namespace Shop.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ShoppingCartDBContext _dbContext = null;

        public UserService(ShoppingCartDBContext dbContext)
        {
            _dbContext = dbContext;
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

        public List<ProductModel> GetProductByCategoryId(int categoryId)
        {
            var data = _dbContext.Products.Where(x => x.CategoryId == categoryId).ToList();
            return data.Select(p => new ProductModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Stock = p.Stock
            }).ToList();
        }

        public ResponseModel RegisterUser(RegisterModel registerModel)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                var exist_check = _dbContext.Customers.Where(x => x.Email == registerModel.EmailId).Any();

                if (!exist_check)
                {
                    Models.Models.Customer _customer = new Models.Models.Customer();
                    _customer.Name = registerModel.Name;
                    _customer.MobileNo = registerModel.MobileNo;
                    _customer.Email = registerModel.EmailId;
                    _customer.Password = registerModel.Password;
                    _dbContext.Add(_customer);
                    _dbContext.SaveChanges();

                    LoginModel loginModel = new LoginModel()
                    {
                        Email = registerModel.EmailId,
                        Password = registerModel.Password
                    };

                    response = LoginUser(loginModel);
                }
                else
                {
                    response.Status = false;
                    response.Message = "Email already registered.";
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "An Error has occurred. Please try again !";

                return response;
            }
        }

        public ResponseModel LoginUser(LoginModel loginModel)
        {
            ResponseModel response = new ResponseModel();

            if (loginModel == null)
            {
                response.Status = false;
                response.Message = "Invalid login request.";
                return response;
            }

            try
            {
                var userData = _dbContext.Customers
                    .Where(x => x.Email == loginModel.Email)
                    .FirstOrDefault();

                if (userData != null)
                {
                    if (userData.Password != null && userData.Password == loginModel.Password)
                    {
                        response.Status = true;
                        response.Message = $"{userData.Id}|{userData.Name}|{userData.Email}";
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = "Your password is incorrect.";
                    }
                }
                else
                {
                    response.Status = false;
                    response.Message = "Email not registered. Please check your email ID.";
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }

        public ResponseModel Checkout(List<CartModel> cartItems)
        {
            string OrderId = GenerateOrderId();
            var prods = _dbContext.Products.ToList();
            try
            {
                var detail = cartItems.FirstOrDefault();
                CustomerOrder _customerOrder = new CustomerOrder();
                _customerOrder.CustomerId = detail.UserId;
                _customerOrder.OrderId = OrderId;
                _customerOrder.PaymentMode = detail.PaymentMode;
                _customerOrder.ShippingAddress = detail.ShippingAddress;
                _customerOrder.ShippingCharges = detail.ShippingCharges;
                _customerOrder.SubTotal = detail.SubTotal;
                _customerOrder.Total = detail.Total;
                _customerOrder.CreatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                _customerOrder.UpdatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                _dbContext.CustomerOrders.Add(_customerOrder);

                foreach (var items in cartItems)
                {
                    OrderDetail _orderDetail = new OrderDetail();
                    _orderDetail.OrderId = OrderId;
                    _orderDetail.ProductId = items.ProductId;
                    _orderDetail.Quantity = items.Quantity;
                    _orderDetail.Price = items.Price;
                    _orderDetail.SubTotal = items.SubTotal;
                    _orderDetail.CreatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                    _orderDetail.UpdatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                    _dbContext.OrderDetails.Add(_orderDetail);

                    var selected_product = prods.Where(x => x.Id == items.ProductId).FirstOrDefault();
                    selected_product.Stock = selected_product.Stock - items.Quantity;
                    _dbContext.Products.Update(selected_product);
                }

                _dbContext.SaveChanges();

                ResponseModel response = new ResponseModel();
                response.Message = OrderId;

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GenerateOrderId()
        {
            string OrderId = string.Empty;
            Random generator = null;

            for (int i = 0; i < 100; i++)
            {
                generator = new Random();
                OrderId = "P" + generator.Next(1, 10000000).ToString("D8");
                if (!_dbContext.CustomerOrders.Where(x => x.OrderId == OrderId).Any())
                {
                    break;
                }
            }
            return OrderId;
        }

        public List<CustomerOrder> GetOrdersByCustomerId(int customerId)
        {
            var _customerOrders = _dbContext.CustomerOrders.Where(x => x.CustomerId == customerId).OrderByDescending(x => x.Id).ToList();
            return _customerOrders;
        }

        public List<CartModel> GetOrderDetailForCustomer(int customerId, string order_number)
        {
            List<CartModel> cart_details = new List<CartModel>();

            var customer_order = _dbContext.CustomerOrders.Where(x => x.CustomerId == customerId && x.OrderId == order_number).FirstOrDefault();

            if (customer_order != null)
            {
                var order_detail = _dbContext.OrderDetails.Where(x => x.OrderId == order_number).ToList();
                var product_list = _dbContext.Products.ToList();

                foreach (var order in order_detail)
                {
                    var prod = product_list.Where(x => x.Id == order.ProductId).FirstOrDefault();
                    CartModel _cartModel = new CartModel();
                    _cartModel.ProductName = prod.Name;
                    _cartModel.Price = Convert.ToInt32(prod.Price);
                    _cartModel.ProductImage = prod.ImageUrl;
                    _cartModel.Quantity = Convert.ToInt32(order.Quantity);
                    cart_details.Add(_cartModel);
                }

                cart_details.FirstOrDefault().ShippingAddress = customer_order.ShippingAddress;
                cart_details.FirstOrDefault().ShippingCharges = Convert.ToInt32(customer_order.ShippingCharges);
                cart_details.FirstOrDefault().SubTotal = Convert.ToInt32(customer_order.SubTotal);
                cart_details.FirstOrDefault().Total = Convert.ToInt32(customer_order.Total);
                cart_details.FirstOrDefault().PaymentMode = customer_order.PaymentMode;
            }

            return cart_details;
        }

        public ResponseModel ChangePassword(PasswordModel passwordModel)
        {
            ResponseModel response = new ResponseModel();
            response.Status = true;

            var _customer = _dbContext.Customers.Where(x => x.Id == passwordModel.UserKey).FirstOrDefault();

            if (_customer != null)
            {
                _customer.Password = passwordModel.Password;
                _dbContext.Customers.Update(_customer);
                _dbContext.SaveChanges();

                response.Message = "Password update successfully !!";
            }
            else
            {
                response.Message = "User does not exist. Try again !!";
            }

            return response;
        }

        public List<string> GetShippingStatusForOrder(string order_number)
        {
            List<string> shipping_status = new List<string>();
            var order = _dbContext.CustomerOrders.Where(x => x.OrderId == order_number).FirstOrDefault();
            if (order != null && order.ShippingStatus != null)
            {
                shipping_status = order.ShippingStatus.Split("|").ToList();
            }

            return shipping_status;
        }

        public async Task<string> MakePaymentStripe(string cardnumber, string expMonth, string expYear, string cvc, decimal value)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51PvNCpRtBfbQhUNFd7L58bsicupxkeXICjabVRh04DjBeCu8i0LOKHjRXhoMwTqEXeU4YnsRV4IadtBESepLdwfB00gu1uo48N";
                var optionToken = new TokenCreateOptions
                {
                    Card = new TokenCardOptions
                    {
                        Number = cardnumber,
                        ExpMonth = expMonth,
                        ExpYear = expYear,
                        Cvc = cvc,
                    }
                };

                var serviceToken = new TokenService();
                Token stripetoken = await serviceToken.CreateAsync(optionToken);

                var customer = new Stripe.Customer
                {
                    Name = "Jackson",
                    Address = new Address
                    {
                        Country = "America",
                        City = "New york",
                        Line1 = "304 - DollarVilla, Mumbai",
                        PostalCode = "400606"
                    }
                };

                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(value),
                    Currency = "INR",
                    Description = "Test",
                    Source = stripetoken.Id,
                };

                var service = new ChargeService();
                Charge charge = await service.CreateAsync(options);

                if (charge.Paid)
                {
                    return "Success";
                }
                else
                {
                    return "Fail";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}