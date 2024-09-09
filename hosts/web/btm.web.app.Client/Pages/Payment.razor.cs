using btm.shared.Messages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace btm.web.app.Client.Pages
{
    public partial class Payment : ComponentBase
    {
        private string _customerReference = Guid.NewGuid().ToString();

        private HubConnection? hubConnection;

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [SupplyParameterFromForm]
        private PaymentAmount? Model { get; set; }

        private string Messages { get; set; } = string.Empty;

        protected override void OnInitialized() => Model ??= new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                hubConnection = new HubConnectionBuilder()
                  .WithUrl(NavigationManager.ToAbsoluteUri("/hubs/payment"))
                  .Build();

                hubConnection.On<PaymentStatus>("UpdateStatus", message =>
                {
                    Messages += $"Payment <b>{message.PaymentReference}</b> status: <b>{message.Status}</b> at {DateTime.Now.ToLongTimeString()} <br/>";
                    StateHasChanged();
                });

                await hubConnection.StartAsync();
            }
        }

        private async Task Deposit()
        {
            await hubConnection!.InvokeAsync("Deposit", Model?.Amount, _customerReference);
        }
    }

    public class PaymentAmount
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EUR";
    }
}
