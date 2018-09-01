using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace KworkTelegramBot
{
    static class TelegramBotHandler
    {
        static TelegramBotClient _client;
        static ChatId _chatUsername;

        public static void Restart()
        {
            _client = new TelegramBotClient( AppSettings.Inst.BotToken);
            _chatUsername = new ChatId(AppSettings.Inst.Chanel);
        }

        public static async Task Send(string text)
        {
            try
            {
                await _client.SendTextMessageAsync(chatId: _chatUsername, text: text);
                WindowLogger.Log("Sended to telegram successfully.");
            }
            catch(Exception ex)
            {
                WindowLogger.Log("Error while sending to telegram.");
                WindowLogger.Log(ex.ToString());
            }
        }

        public static async Task Send(KworkRecord rec)
        {
            string text = rec.Title + "\n\n" + rec.Text + "\n\n" + rec.Info;
            await Send(text);
        }

        public static async Task Send(List<KworkRecord> records)
        {
            foreach (var rec in records)
                await Send(rec);
        }
    }
}
