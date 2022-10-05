using Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<PingGrpcService>();
app.MapGet("/", () => "Test gRPC service");

app.Run();