using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;

namespace WantedPersonsBot
{
    class Program
    { 

        static void Main(string[] args)
        {
            TelegramBotMethods.wPersons = JsonSerializer.Deserialize<List<WantedPerson>>(System.IO.File.ReadAllText("../../../data/mvswantedperson_1.json"));
            TelegramBotMethods.photos = JsonSerializer.Deserialize<List<Photo>>(System.IO.File.ReadAllText("../../../data/mvswantedperson_photo_304.json"));
            
            var botClient = new TelegramBotClient("5581982426:AAFbUOMF7047IB0LAFF9ZdQvUYK9lOxwO0g");

            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() 
            };
            
            botClient.StartReceiving(
                updateHandler: TelegramBotMethods.HandleUpdateAsync,
                errorHandler: TelegramBotMethods.HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = botClient.GetMeAsync().Result;

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cts.Cancel();

        }
    }
}
