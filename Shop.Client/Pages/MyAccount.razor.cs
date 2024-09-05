using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Shop.Client.Services;
using Shop.Models.CustomModels;
using Shop.Models.Models;

namespace Shop.Client.Pages
{
    public partial class MyAccount : ComponentBase
    {
        [Inject]
        public NavigationManager? navManager { get; set; }

        [Inject]
        public ILocalStorageService LocalStorage { get; set; }

        [Inject]
        public IClientPanelService clientPanelService { get; set; }

        [Inject]
        public ISessionStorageService sessionStorage { get; set; }

        [CascadingParameter]
        public EventCallback notify { get; set; }

        public List<string> shippingUpdates { get; set; }
        public List<CartModel> myCart { get; set; }
        public string shippingAddress { get; set; }
        public int shippingCharges { get; set; }
        public string paymentMode { get; set; }
        public int subTotal { get; set; }
        public int finalTotal { get; set; }
        string Imageslink { get; set; } = "https://localhost:7123/GetImages";

        private List<ProductModel> products;
        private List<CustomerOrder> customerOrders;
        private PasswordModel passwordModel;
        private ResponseModel response;

        private string headerText = "MyAccount";
        private int userkey = 0;
        private string userName = string.Empty;
        private string userEmail = string.Empty;
        private string alertMessage = string.Empty;
        private bool myAccountFlag = true;
        private bool changePasswordFlag = false;
        private bool orderHistoryFlag = false;
        public bool showTrackOrder = false;
        public bool showOrderDetail = false;
        public bool showAlertMessage = false;

        protected override Task OnInitializedAsync()
        {
            products = new List<ProductModel>();
            customerOrders = new List<CustomerOrder>();
            passwordModel = new PasswordModel();
            return base.OnInitializedAsync();
        }

        private async Task ChangeActiveMenu(string menu)
        {
            headerText = menu;
            switch (menu)
            {
                case "MyAccount":
                    myAccountFlag = true;
                    changePasswordFlag = false;
                    orderHistoryFlag = false;
                    break;

                case "ChangePassword":
                    myAccountFlag = false;
                    changePasswordFlag = true;
                    orderHistoryFlag = false;
                    break;

                case "OrderHistory":
                    myAccountFlag = false;
                    changePasswordFlag = false;
                    orderHistoryFlag = true;
                    await GetOrdersByCustomerId(userkey);
                    break;
            }
        }

        private async Task GetOrdersByCustomerId(int customerId)
        {
            if (customerOrders.Count == 0)
            {
                customerOrders = await clientPanelService.GetOrdersByCustomerId(customerId);
            }
        }

        private void ToggleAlertMessage()
        {
            showAlertMessage = showAlertMessage == true ? false : true;
        }

        private void ToggleOrderDetailPopup()
        {
            showOrderDetail = showOrderDetail == true ? false : true;
        }

        private void ToggleTrackOrderPopup()
        {
            showTrackOrder = showTrackOrder == true ? false : true;
        }

        private async Task GetOrderDetailForCustomer(int customerId, string order_number)
        {
            myCart = await clientPanelService.GetOrderDetailForCustomer(customerId, order_number);

            if (myCart != null && myCart.Count > 0)
            {
                shippingAddress = myCart.FirstOrDefault().ShippingAddress;
                shippingCharges = myCart.FirstOrDefault().ShippingCharges;
                subTotal = myCart.FirstOrDefault().SubTotal;
                finalTotal = myCart.FirstOrDefault().Total;
                paymentMode = myCart.FirstOrDefault().PaymentMode;
            }
        }

        private async Task Detail_Click(string order_number)
        {
            await GetOrderDetailForCustomer(userkey, order_number);
            ToggleOrderDetailPopup();
        }
        private async Task Track_Click(string order_number)
        {
            shippingUpdates = await clientPanelService.GetShippingStatusForOrder(order_number);
            ToggleOrderDetailPopup();
        }

        private async Task ChangePassword_Click()
        {
            if (passwordModel.Password != passwordModel.ConfirmPassword)
            {
                alertMessage = "Password & Confirm password are not same";
                ToggleAlertMessage();
            }
            else
            {
                passwordModel.UserKey = userkey;
                response = await clientPanelService.ChangePassword(passwordModel);
                if (response.Status)
                {
                    passwordModel = new PasswordModel();
                    alertMessage = response.Message;
                    ToggleAlertMessage();
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var session_userKey = await LocalStorage.GetItemAsync<string>("userkey");
                var session_userName = await LocalStorage.GetItemAsync<string>("username");
                var session_userEmail = await LocalStorage.GetItemAsync<string>("useremail");

                if (session_userKey != null && session_userName != null && session_userEmail != null)
                {
                    userkey = Convert.ToInt32(session_userKey);
                    userName = session_userName;
                    userEmail = session_userEmail;
                }

                var session_latestOrder = await LocalStorage.GetItemAsync<string>("latestOrder");

                if (session_latestOrder != null)
                {
                    await LocalStorage.RemoveItemAsync("latestOrder");
                    await GetOrdersByCustomerId(userkey);
                    await ChangeActiveMenu("OrderHistory");
                }

                StateHasChanged();
            }
        }
    }
}