using Grpc.Core;
using Grpc.Net.Client;
using GrpcCalculator;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

using var channel = GrpcChannel.ForAddress("http://localhost:5001");
var client = new Calculator.CalculatorClient(channel);
var request = new BinaryRequest { X = 20, Y = 5 };

await Execute("SUM", () => client.SUMAsync(request).ResponseAsync);
await Execute("SUB", () => client.SUBAsync(request).ResponseAsync);
await Execute("MUL", () => client.MULAsync(request).ResponseAsync);
await Execute("DIV", () => client.DIVAsync(request).ResponseAsync);
await Execute("FACT", () => client.FACTAsync(new FactorialRequest { X = 5 }).ResponseAsync);
await Execute("DIV by zero", () => client.DIVAsync(new BinaryRequest { X = 10, Y = 0 }).ResponseAsync);
await Execute("FACT overflow", () => client.FACTAsync(new FactorialRequest { X = 13 }).ResponseAsync);

static async Task Execute(string name, Func<Task<CalculationReply>> operation)
{
    try
    {
        var response = await operation();
        Console.WriteLine($"{name}: {response.Result}");
    }
    catch (RpcException exception)
    {
        Console.WriteLine($"{name}: gRPC error {exception.StatusCode} - {exception.Status.Detail}");
    }
}
