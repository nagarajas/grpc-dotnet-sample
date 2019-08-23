// Copyright 2015 gRPC authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Grpc.Core;
using Helloworld;
using System.Threading;
using System.Threading.Tasks;

namespace GreeterClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            Task.Run(async ()=> await EstablishAndInvokeMethodsAsync());
            Console.ReadKey();
        }

        public async static Task EstablishAndInvokeMethodsAsync()
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);
            var client = new GreetClient(new Greeter.GreeterClient(channel));

            Console.WriteLine("-------------------------------------------------------\n\n");
            client.SayHello();

            Console.WriteLine("-------------------------------------------------------\n\n");
            await client.SayHelloToAllAsync();

            Console.WriteLine("-------------------------------------------------------\n\n");
            await client.GetHelloToAllAsync();

            Console.WriteLine("-------------------------------------------------------\n\n");
            await client.GetHelloToAsync();
           
            await channel.ShutdownAsync();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
