using SignalRHub.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<AlertHub>("alert-hub");

app.Run();