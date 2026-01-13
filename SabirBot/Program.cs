using System.Dynamic;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SabirBot
{
    internal class Program
    {
        private static async Task Main()
        {
            var token = Environment.GetEnvironmentVariable("TG_TOKEN");
            using var cts = new CancellationTokenSource();
            var bot = new TelegramBotClient(token, cancellationToken: cts.Token);
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                cancellationToken: cts.Token
            );
            var me = await bot.GetMe();
            Console.WriteLine($"@{me.Username} is running... Press Escape to terminate");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }

        private static async Task HandleUpdateAsync(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
                return;

            if (update.Message!.Type != MessageType.Text)
                return;

            await HandleMessageAsync(update.Message, botClient, cancellationToken);
        }

        private static Task HandleErrorAsync(
            ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken)
        {
            Console.WriteLine(exception);
            return Task.CompletedTask;
        }

        private static async Task HandleMessageAsync(
            Message message,
            ITelegramBotClient botClient,
            CancellationToken token)
        {
            var text = message.Text!.Trim().ToLower();

            switch (text)
            {
                case "/start":
                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "об авторе", "цена" },
                    new KeyboardButton[] { "оплата", "помощь" }
                })
                    {
                        ResizeKeyboard = true
                    };

                    await botClient.SendMessage(
                        message.Chat.Id,
                        "Выберите команду:",
                        replyMarkup: replyKeyboard,
                        cancellationToken: token
                    );
                    break;
                case "/about":
                case "об авторе":
                    await botClient.SendMessage(
                        message.Chat.Id,
                        "Докторская степень из твиттера",
                        cancellationToken: token
                    );
                    return;

                case "/help":
                case "помощь":
                    await botClient.SendMessage(
                        message.Chat.Id,
                        "Вопросы: @without_excuses",
                        cancellationToken: token
                    );
                    return;

                case "/price":
                case "цены":
                case "цена":
                    await botClient.SendMessage(
                        message.Chat.Id,
                        "1 месяц — 350 ₽",
                        cancellationToken: token
                    );
                    return;

                case "/pay":
                case "оплата":
                    await botClient.SendMessage(
                        message.Chat.Id,
                        "По СПБ: 89992152855\r\nПо номеру карты (Т-банк): 2200701039906000",
                        cancellationToken: token
                    );
                    return;
            }
        }
    }
}
