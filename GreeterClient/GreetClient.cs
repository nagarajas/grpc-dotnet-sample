using System;
using Grpc.Core;
using Helloworld;
using System.Threading;
using System.Threading.Tasks;

namespace GreeterClient
{
    public class GreetClient
    {
        private readonly Greeter.GreeterClient client;

        public GreetClient(Greeter.GreeterClient greeterClient)
        {
            client = greeterClient;
        }

        public void SayHello()
        {
            String user = "Shiva";
            var reply = client.SayHello(new HelloRequest { Name = user });
            Console.WriteLine($"Greeting: {reply.Message}" );
        }

        public async Task SayHelloToAllAsync()
        {
            using (var call = client.SayHelloWithClientStream())
            {
                 foreach (var item in new string[]{ "Shiva", "Rama", "Lakshmana" })
                 {
                     await call.RequestStream.WriteAsync(new HelloRequest(){ Name = item });
                     await Task.Delay(1000);
                 }   

                 await call.RequestStream.CompleteAsync();
                 var reply = await call.ResponseAsync;

                 Console.WriteLine($"Greeting: {reply.Message}");
            }
        }

        public async Task GetHelloToAllAsync()
        {
            var request = new HelloRequest (){ Name = "Shiva, Rame, Lakshmana"};
            using (var call = client.SayHelloWithServerStream(request))
            {
                var responseStream = call.ResponseStream;
                var cts = new CancellationTokenSource().Token;
                while (await responseStream.MoveNext(cts))
                {
                    var reply = responseStream.Current;
                    Console.WriteLine($"Greeting: {reply.Message}");
                }
            }
        }

        public async Task GetHelloToAsync()
        {
            using (var call = client.SayHelloWithBidirectionalStream())
            {
                    var responseTask = Task.Run(async () =>
                        {
                            var cts = new CancellationTokenSource().Token;
                            while (await call.ResponseStream.MoveNext(cts))
                            {
                                var reply = call.ResponseStream.Current;
                                Console.WriteLine($"Greeting: {reply.Message}");
                            }
                        });
                        
                        foreach (var name in new string[]{ "Shiva", "Rama", "Lakshmana" })
                        {
                            await call.RequestStream.WriteAsync(new HelloRequest(){ Name = name });
                        }
                        await call.RequestStream.CompleteAsync();
                        await responseTask;
            }
        }
    }   
}