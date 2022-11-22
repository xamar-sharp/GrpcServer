using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System;
using Grpc.Core;
namespace GrpcService.Services
{
    public sealed class TelemetryService : Telemetry.TelemetryBase
    {
        private readonly ILogger<TelemetryService> _logger;
        public TelemetryService(ILogger<TelemetryService> logger)
        {
            _logger = logger;
        }
        public override async Task<MemoryMark> ComputeClientUsageDelta(IAsyncStreamReader<MemoryInfo> requestStream, ServerCallContext context)
        {
            List<MemoryInfo> memories = new List<MemoryInfo>(2);
            await foreach (var info in requestStream.ReadAllAsync())
            {
                memories.Add(info);
            }
            return new MemoryMark()
            {
                PhysicalMemoryDelta = Convert.ToInt64(memories.Average(e => e.PhysicalMemory)),
                VirtualMemoryDelta = Convert.ToInt64(memories.Average(e => e.VirtualMemory))
            };
        }
        public override async Task<ServerUsage> RequestServerUsage(RequestOptions request, ServerCallContext context)
        {
            _logger.LogInformation("Server usage invoked!");
            await Task.Yield();
            return new ServerUsage() { PhysicalUsage = Process.GetCurrentProcess().WorkingSet64, VirtualUsage =Process.GetCurrentProcess().VirtualMemorySize64};
        }
    }
}
