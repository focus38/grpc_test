using Grpc.Core;
using Test.Grpc.Proto;

namespace Server.Services;

public class PingGrpcService : Test.Grpc.Proto.PingService.PingServiceBase
{
    public override async Task<PingReply> Ping(PingRequest request, ServerCallContext context)
    {
        return await Task.FromResult(new PingReply {Id = $"Reply_{request.Id}"});
    }
}