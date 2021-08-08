using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tgbot_calc
{
    class Program
    {
        static TelegramBotClient Bot;
        static string BOT_TOKEN = "TELEGRAM_BOT_TOKEN";

        static async Task Main()
        {
            var cts = new CancellationTokenSource();

            try
            {
                Bot = new TelegramBotClient(BOT_TOKEN);

                var me = await Bot.GetMeAsync();
                Console.WriteLine($"Start listening for @{me.Username}");

                //Console.WriteLine($"{telegram_bot.Calc.calculate("2+4")}");

                await Bot.ReceiveAsync(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                cts.Cancel();
            }
        }

        static async Task BotOnMessageReceived(Message message)
        {
            Console.WriteLine($"Receive message type: {message.Type}");

            if (message.Type != MessageType.Text)
                return;

            //await Bot.SendTextMessageAsync(message.Chat.Id, $"Received {message.Text}");
            await Bot.SendTextMessageAsync(message.Chat.Id, $"{tgbot_calc.Calc.calculate(message.Text)}");

        }

        static async Task HandleUpdateAsync(ITelegramBotClient arg1, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message)
                return;

            try
            {
                await BotOnMessageReceived(update.Message);
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(Bot, exception, cancellationToken);
            }
        }

        static Task HandleErrorAsync(ITelegramBotClient arg1, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);

            return Task.CompletedTask;
        }
    }
}
