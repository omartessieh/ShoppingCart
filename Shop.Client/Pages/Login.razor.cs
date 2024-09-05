using Microsoft.AspNetCore.Components;
using System.Net.NetworkInformation;
using Shop.Models.CustomModels;
using Shop.Client.Services;
using Blazored.LocalStorage;
using Blazored.SessionStorage;

namespace Shop.Client.Pages
{
    public partial class Login : ComponentBase
    {

        //https://www.youtube.com/watch?v=V1fYSDHjN24&list=PLWpvttYT5mY4Z_GJJULctGOFO-His_w1S&index=12
        //0:00 / 11:15

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

        public LoginModel loginModel { get; set; }

        public string alertMessage { get; set; }

        bool flag = false;

        protected override Task OnInitializedAsync()
        {
            loginModel = new LoginModel();
            return base.OnInitializedAsync();
        }

        private async Task Login_Click()
        {
            try
            {
                var response = await clientPanelService.LoginUser(loginModel);

                if (response != null)
                {
                    var userResponseArray = response.Message?.Split("|");

                    if (userResponseArray != null && userResponseArray.Length >= 3)
                    {
                        await LocalStorage.SetItemAsync("userkey", userResponseArray[0]);
                        await LocalStorage.SetItemAsync("username", userResponseArray[1]);
                        await LocalStorage.SetItemAsync("useremail", userResponseArray[2]);
                        await notify.InvokeAsync();
                        navManager.NavigateTo("/");
                    }
                    else
                    {
                        alertMessage = userResponseArray[0];
                    }
                }
                else
                {
                    alertMessage = "Login failed. Please try again.";
                }
            }
            catch (Exception ex)
            {
                alertMessage = $"An error occurred: {ex.Message}";
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (clientPanelService != null)
                {
                    var result = await LocalStorage.GetItemAsync<string>("userkey");

                    if (!string.IsNullOrEmpty(result))
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }

                    var login_session = flag;

                    if (login_session)
                    {
                        navManager.NavigateTo("/");
                    }

                    var checkoutAlert = await LocalStorage.GetItemAsync<string>("checkoutAlert");

                    if (!string.IsNullOrEmpty(checkoutAlert))
                    {
                        this.alertMessage = checkoutAlert;
                        StateHasChanged();
                    }
                }
                else
                {
                    alertMessage = "ClientPanelService is not available.";
                }
            }
        }
    }
}