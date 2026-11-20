using Grpc.Core;
using GrpcCalculator;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5001, listenOptions => listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
});

var app = builder.Build();
app.MapGrpcService<CalculatorService>();
app.MapGet("/", () => "gRPCServer01 is running");
app.Run();

public class CalculatorService : Calculator.CalculatorBase
{
    public override Task<CalculationReply> SUM(BinaryRequest request, ServerCallContext context)
        => Task.FromResult(new CalculationReply { Result = request.X + request.Y });

    public override Task<CalculationReply> SUB(BinaryRequest request, ServerCallContext context)
        => Task.FromResult(new CalculationReply { Result = request.X - request.Y });

    public override Task<CalculationReply> MUL(BinaryRequest request, ServerCallContext context)
        => Task.FromResult(new CalculationReply { Result = request.X * request.Y });

    public override Task<CalculationReply> DIV(BinaryRequest request, ServerCallContext context)
    {
        if (request.Y == 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Division by zero is not allowed."));
        return Task.FromResult(new CalculationReply { Result = request.X / request.Y });
    }

    public override Task<CalculationReply> FACT(FactorialRequest request, ServerCallContext context)
    {
        if (request.X < 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Factorial is not defined for negative integers."));

        int result = 1;
        for (int i = 2; i <= request.X; i++)
        {
            if (result > int.MaxValue / i)
                throw new RpcException(new Status(StatusCode.OutOfRange, "Factorial result exceeds the Int32 range."));
            result *= i;
        }

        return Task.FromResult(new CalculationReply { Result = result });
    }
}
