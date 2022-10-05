using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Logging;
using Test.Grpc.Proto;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
const int maxAttempts = 50;

var defaultRetryPolicy = new RetryPolicy
{
    MaxAttempts = maxAttempts,
    BackoffMultiplier = 1.5,
    InitialBackoff = TimeSpan.FromSeconds(2),
    MaxBackoff = TimeSpan.FromSeconds(10),
    RetryableStatusCodes = {StatusCode.Unavailable}
};

var defaultMethodConfig = new MethodConfig
{
    Names = {MethodName.Default},
    RetryPolicy = defaultRetryPolicy
};

var channelOptions = new GrpcChannelOptions
{
    MaxRetryAttempts = maxAttempts,
    LoggerFactory = LoggerFactory.Create(config =>
    {
        config.SetMinimumLevel(LogLevel.Debug);
        config.AddSimpleConsole(c =>
        {
            c.SingleLine = true;
            c.UseUtcTimestamp = true;
            c.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
        });
    }),
    
    // It works well, if you comment out lines 41 to 45.
    // But the connection timeout is very long 130 sec.
    HttpHandler = new SocketsHttpHandler
    {
        EnableMultipleHttp2Connections = true,
        ConnectTimeout = TimeSpan.FromSeconds(10)
    },
    //ThrowOperationCanceledOnCancellation = true,
    ServiceConfig = new ServiceConfig
    {
        MethodConfigs = { defaultMethodConfig }
    }
};

// My host in local network
var uri = "http://192.168.0.5:51000";
// var uri = "http://localhost:9002"; // Work well, if start server at localhost

var channel = GrpcChannel.ForAddress(uri, channelOptions);
var client = new PingService.PingServiceClient(channel);
var response = await client.PingAsync(new PingRequest {Id = "1"});

Console.WriteLine($"gRPC ping response: {response.Id}");