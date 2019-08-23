using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Helloworld;

namespace GreeterServer
{
    public class GreeterImpl : Greeter.GreeterBase
    {
        // Server side handler of the SayHello RPC
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
        }

        public override async Task<HelloReply> SayHelloWithClientStream(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            var names = "";
            var cts = new CancellationTokenSource().Token;
            while (await requestStream.MoveNext(cts))
            {
                  names += " " + requestStream.Current.Name;
            }

            return new HelloReply() { Message = $"Hello {names}"};
        }

        public override async Task SayHelloWithServerStream(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            string[] names = request.Name.Split(',');
            foreach (var name in names)
            {
                await responseStream.WriteAsync(new HelloReply { Message = "Hello " + name });
            }
        }

        public override async Task SayHelloWithBidirectionalStream(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            var cts = new CancellationTokenSource().Token;
            while (await requestStream.MoveNext(cts))
            {
                var request = requestStream.Current;
                await responseStream.WriteAsync(new HelloReply { Message = "Hello " + request.Name });
            }
        }
    }
}
