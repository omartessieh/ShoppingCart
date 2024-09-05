using Microsoft.AspNetCore.Components;
using System.Net.NetworkInformation;
using Shop.Models.CustomModels;
using Shop.Client.Services;
using Blazored.LocalStorage;
using Blazored.SessionStorage;

namespace Shop.Client.Pages
{
    public partial class Register : ComponentBase
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

        public RegisterModel registerModel { get; set; }

        public string alertMessage { get; set; }

        bool flag = false;

        protected override Task OnInitializedAsync()
        {
            registerModel = new RegisterModel();
            return base.OnInitializedAsync();
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

        private async Task Register_Click()
        {
            var response = await clientPanelService.RegisterUser(registerModel);

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
                    alertMessage = "Invalid response format.";
                }
            }
            else
            {
                alertMessage = "Registration failed. Please try again.";
            }
        }
    }
}