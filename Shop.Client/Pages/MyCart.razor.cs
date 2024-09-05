using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Shop.Client.Services;
using Shop.Models.CustomModels;
using Blazored.SessionStorage;

namespace Shop.Client.Pages
{
    public partial class MyCart : ComponentBase
    {
        [Inject]
        public NavigationManager? navManager { get; set; }

        [Inject]
        public ILocalStorageService LocalStorage { get; set; }

        [Inject]
        public IClientPanelService clientPanelService { get; set; }

        [CascadingParameter]
        public EventCallback notify { get; set; }

        public List<CartModel> myCart { get; set; } = new List<CartModel>();
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMode { get; set; } = "Cash on Delivery";
        public int ShippingCharges { get; set; } = 100;
        public int SubTotal { get; set; }
        public int finalTotal { get; set; }
        string Imageslink { get; set; } = "https://localhost:7123/GetImages";
        public string userName { get; set; } = string.Empty;
        public string userEmail { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;

        private bool IsUserLoggedIn = false;

        protected override async Task OnInitializedAsync()
        {
            var storage_username = await LocalStorage.GetItemAsync<string>("username");
            var storage_useremail = await LocalStorage.GetItemAsync<string>("useremail");

            ShippingCharges = 25;
            SubTotal = 0;
            PaymentMode = "Cash on Delivery";
            ShippingAddress = null;
            Error = null;

            if (!string.IsNullOrEmpty(storage_username))
                userName = storage_username;

            if (!string.IsNullOrEmpty(storage_useremail))
                userEmail = storage_useremail;

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var result = await LocalStorage.GetItemAsync<List<CartModel>>("myCart");

                if (result != null)
                {
                    myCart = result;

                    foreach (var cart_item in myCart)
                    {
                        SubTotal += (cart_item.Price * cart_item.Quantity);
                    }

                    finalTotal = SubTotal + ShippingCharges;

                    if (myCart.Any())
                    {
                        ShippingAddress = myCart.First().ShippingAddress ?? string.Empty;
                    }

                    StateHasChanged();
                }
            }
        }

        private async Task RemoveFromCart_Click(CartModel cartItem)
        {
            myCart.Remove(cartItem);
            await LocalStorage.SetItemAsync("myCart", myCart);
            onQuantityChange();
            await notify.InvokeAsync();
        }

        private void onQuantityChange()
        {
            SubTotal = myCart.Sum(cart_item => cart_item.Price * cart_item.Quantity);
            finalTotal = SubTotal + ShippingCharges;
        }

        private async Task Checkout_Click()
        {
            if (myCart != null && myCart.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(ShippingAddress))
                {
                    Error = "Please enter a valid Shipping Address";
                    return;
                }

                foreach (var item in myCart)
                {
                    item.ShippingAddress = ShippingAddress;
                    item.ShippingCharges = ShippingCharges;
                    item.SubTotal = SubTotal;
                    item.PaymentMode = PaymentMode;
                    item.Total = finalTotal;

                    var userKey = await LocalStorage.GetItemAsync<string>("userkey");
                    if (!string.IsNullOrEmpty(userKey))
                    {
                        item.UserId = Convert.ToInt32(userKey);
                    }
                }

                var response = await clientPanelService.Checkout(myCart);
                if (response != null)
                {
                    myCart.Clear();
                    await LocalStorage.SetItemAsync("myCart", myCart);
                    await LocalStorage.SetItemAsync("latestOrder", response.Message);
                    await notify.InvokeAsync();
                    navManager.NavigateTo("myaccount");
                }
                else
                {
                    navManager.NavigateTo("login");
                }
            }
            else
            {
                await LocalStorage.SetItemAsync("checkoutAlert", "** Note : Your cart is empty!!");
                navManager.NavigateTo("login");
            }
        }

        private async Task Clear_Click()
        {
            myCart.Clear();
            await LocalStorage.RemoveItemAsync("myCart");
            await notify.InvokeAsync();
        }
    }
}