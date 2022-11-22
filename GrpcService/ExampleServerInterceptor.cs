using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
namespace GrpcService
{
    public sealed class ExampleServerInterceptor:Interceptor
    {
        private readonly ILogger<ExampleServerInterceptor> _logger;
        public ExampleServerInterceptor(ILogger<ExampleServerInterceptor> logger)
        {
            _logger = logger;
        }
        public override async Task<TResponse> UnaryServerHandler<TRequest,TResponse>(TRequest request,ServerCallContext ctx,UnaryServerMethod<TRequest,TResponse> method)
        {
            _logger.LogInformation($"Server interceptor with {ctx.Method}");
            return await method(request, ctx);
        }
    }
}
