using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using PubNub.Async.Autofac;
using PubNub.Async.Extensions;
using PubNub.Async.Services.Subscribe;
using PubNub.Async.Tests.App.Properties;

namespace PubNub.Async.Tests.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Configuring PubNub.Async...");

            var builder = new ContainerBuilder();
            builder.RegisterModule<PubNubAsyncModule>();
            builder.Build();

            Console.WriteLine("Please stand by.");
            PubNub.Configure(c =>
            {
                c.PublishKey = Settings.Default.PublishKey;
                c.SubscribeKey = Settings.Default.SubscribeKey;
            });

            Console.WriteLine($"Subscribing to {Settings.Default.Channel}1");
            var subscribeResult1 = $"{Settings.Default.Channel}1"
                .Subscribe<Message>(Handler1)
                .Result;

            if (!subscribeResult1.Success)
            {
                Console.Error.WriteLine("Something went wrong");
                Console.WriteLine("Press any key to exit.");
                Console.Read();
                Environment.Exit(1);
            }

            Console.WriteLine($"Subscribing to {Settings.Default.Channel}2");
            var subscribeResult2 = $"{Settings.Default.Channel}2"
                .Subscribe<Message>(Handler2)
                .Result;

            if (!subscribeResult2.Success)
            {
                Console.Error.WriteLine("Something went wrong");
                Console.WriteLine("Press any key to exit.");
                Console.Read();
                Environment.Exit(1);
            }

            Console.CancelKeyPress += ConsoleCancelHandler;
            Console.WriteLine("Success.  Send messages or input CTRL+C to cancel.");

            Task.Run(async () =>
            {
                while (true)
                {
                    var input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        var pubResponse1 = await $"{Settings.Default.Channel}1".Publish(new Message {Text = input});
                        if (!pubResponse1.Success)
                        {
                            Console.Error.WriteLine("Publish failed");
                            Console.Error.WriteLine(JsonConvert.SerializeObject(pubResponse1, Formatting.Indented));
                            Console.WriteLine("Press any key to exit.");
                            Console.Read();
                            Environment.Exit(1);
                        }
                        var pubResponse2 = await $"{Settings.Default.Channel}2".Publish(new Message { Text = input });
                        if (!pubResponse2.Success)
                        {
                            Console.Error.WriteLine("Publish failed");
                            Console.Error.WriteLine(JsonConvert.SerializeObject(pubResponse2, Formatting.Indented));
                            Console.WriteLine("Press any key to exit.");
                            Console.Read();
                            Environment.Exit(1);
                        }
                    }
                }
            }).Wait();
        }
        
        private static void ConsoleCancelHandler(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            Environment.Exit(0);
        }

        private static async Task Handler1(MessageReceivedEventArgs<Message> args)
        {
            var priorForeground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;

            Console.WriteLine($"{args.Message.Text} [{args.Sent}]");

            Console.ForegroundColor = priorForeground;
        }

        private static async Task Handler2(MessageReceivedEventArgs<Message> args)
        {
            var priorForeground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;

            Console.WriteLine($"{args.Message.Text} [{args.Sent}]");

            Console.ForegroundColor = priorForeground;
        }
    }

    class Message
    {
        public string Text { get; set; }
    }
}
