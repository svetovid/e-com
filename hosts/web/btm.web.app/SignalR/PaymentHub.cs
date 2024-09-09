using System;
using Akka.Actor;
using Microsoft.AspNetCore.SignalR;
using btm.shared.Messages;
using btm.web.app.Actors;

namespace btm.web.app.SignalR
{
  public class PaymentHub : Hub
  {
    private readonly IActorRef _bridgeActor;
    private readonly IHubContext<PaymentHub> _hubContext;
    private readonly ILogger<PaymentHub> _logger;

    public PaymentHub(IHubContext<PaymentHub> hubContext, ILoggerFactory loggerFactory)
    {
      _bridgeActor = PaasActorSystem.GetSignalRBridgeActor(Send);
      _hubContext = hubContext;

      _logger = loggerFactory.CreateLogger<PaymentHub>();
    }

    public void Deposit(int amount, string customerReference)
    {
      _logger.LogInformation("Make {amount} EUR Deposit", amount);

      var request = new PaymentRequest("test.com",
          new PaymentInformation("EUR", amount, "VISA", DateTime.UtcNow, DateTime.UtcNow, Guid.NewGuid().ToString(), string.Empty, 0, 1),
          new CustomerInformation("SE", customerReference),
          Context.ConnectionId);

      _bridgeActor.Tell(request);
    }

    private void Send(string connectionId, PaymentStatus message)
    {
      _hubContext.Clients.Client(connectionId).SendAsync("UpdateStatus", message);
    }
  }
}