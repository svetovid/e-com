using btm.web.app.Actors;
using btm.web.app.Client.Pages;
using btm.web.app.Components;
using btm.web.app.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
PaasActorSystem.ActivateActorSystem();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseWebAssemblyDebugging();
}
else
{
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Payment).Assembly);

app.MapHub<PaymentHub>("/hubs/payment");

app.Run();
