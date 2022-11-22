using System.Threading.Tasks;
using Grpc.Core;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
namespace GrpcService.Services
{
    public sealed class MessagingService:Messaging.MessagingBase
    {
        private readonly ILogger<MessagingService> _logger;
        public MessagingService(ILogger<MessagingService> logger)
        {
            _logger = logger;
        }
        public override async Task Speak(IAsyncStreamReader<ClientMessage> requestStream, IServerStreamWriter<ServerMessage> responseStream, ServerCallContext context)
        {
            var task = Task.Run(async () =>
            {
                while (await requestStream.MoveNext())
                {
                    _logger.LogInformation(requestStream.Current.Text);
                }
            });
            for(int x = 0;x< 5; x++)
            {
                await responseStream.WriteAsync(new ServerMessage() { Text = "Server: " + x });
                await Task.Delay(1);
            }
        }
        public override async Task LoadFile(FileAbout request, IServerStreamWriter<PartialData> responseStream, ServerCallContext context)
        {
            if (!File.Exists(request.FileName))
            {
                await responseStream.WriteAsync(new PartialData() { Bucket = await Google.Protobuf.ByteString.FromStreamAsync(new MemoryStream(new byte[] {0})) });
                return;
            }
            byte[] summary = await File.ReadAllBytesAsync(request.FileName);
            int lastStart = 0;
            int lastEnd = summary.Length / 5;
            for(int x = 0; x < 5; x++)
            {
                await responseStream.WriteAsync(new PartialData() { Bucket = await Google.Protobuf.ByteString.FromStreamAsync(new MemoryStream(summary[lastStart..lastEnd])) });
                _logger.LogInformation($"Was sent: {lastEnd - lastStart} bytes!");
                int prevLastEnd = lastEnd;
                lastStart = lastEnd + 1;
                lastEnd = prevLastEnd * 2 + 1;
            }
        }
    }
}
