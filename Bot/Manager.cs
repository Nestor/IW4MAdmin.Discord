using System;
using System.Threading.Tasks;
using Discord;
using IW4MAdmin.Discord.Debugging;

namespace IW4MAdmin.Discord
{
    class Bot
    {
        private static Bot _bot = null;
        private DiscordClient dc;

        public ~Bot()
        {
            if (dc != null)
            {
                dc.Disconnect();
                dc.Dispose();
            }
        }

        public static Bot getBot()
        {
            if (_bot == null)
                _bot = new Bot();
            return _bot;
        }

        public void Initialize()
        {
            dc = new DiscordClient();
            dc.MessageReceived += onMessageReceived;
            dc.Log.Message += onRequestLog;

            try
            {
                dc.ExecuteAndWait(Connect);
            }

            catch (Exception e)
            {
                dc.Log.Error("Could not connect!", e);
            }
        }

        void onRequestLog(object sender, LogMessageEventArgs e)
        {
            Logging.WriteLine(e.Message + "->" + e.Exception.Message);
        }

        async void onMessageReceived(object sender, MessageEventArgs e)
        {
            
        }

        async Task Connect()
        {  
            await dc.Connect(Environment.GetEnvironmentVariable("discord-bot-token"), TokenType.Bot);
            dc.Log.Info("Connected!", null);
        }
    }
}
