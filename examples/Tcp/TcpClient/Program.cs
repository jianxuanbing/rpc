﻿using AspectCore.Extensions.DependencyInjection;
using DotNetty.Buffers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tars.Net.Clients;
using Tars.Net.Codecs;
using Tars.Net.Configurations;
using Tars.Net.DotNetty;
using Tars.Net.Extensions.AspectCore;
using TcpCommon;

namespace TcpClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                var service = new ServiceCollection()
                    .AddSingleton<IDecoder<IByteBuffer>, TestDecoder>()
                    .AddSingleton<IEncoder<IByteBuffer>, TestEncoder>()
                    .AddSingleton<IContentDecoder, TestContentDecoder>()
                    .AddSingleton<IConfiguration>(i => builder.AddJsonFile("app.json").Build())
                    .AddLogging(j => j.AddConsole())
                    .AddTarsClient(opt =>
                    {
                        opt.UseAspectCore();
                    })
                    .AddConfiguration()
                    .AddLibuvTcpClient()
                    .BuildAspectCoreServiceProvider();

                var rpc = service.GetRequiredService<IHelloRpc>();
                var result = string.Empty;
                result = rpc.Hello(0, "Hello Victor");
                Console.WriteLine(result);
                result = "HelloHolder";
                rpc.HelloHolder(1, out result);
                Console.WriteLine(result);
                result = await rpc.HelloTask(2, "HelloTask Vic");
                Console.WriteLine(result);
                //result = "Oneway";
                //rpc.HelloOneway(3, "Oneway Vic");
                Console.WriteLine(result);
                result = await rpc.HelloValueTask(4, "HelloValueTask Vic");
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }
    }
}