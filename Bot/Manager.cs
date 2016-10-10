using System;
using System.Threading.Tasks;
using Discord;
using IW4MAdmin.Discord.Debugging;
using IW4MAdmin.Discord.Query;

namespace IW4MAdmin.Discord
{
    class Bot
    {
        private static Bot _bot = null;
        private DiscordClient dc;
        private Configuration.ConfigFile cfg;
        private RestConnection rc;

        ~Bot()
        {
            if (dc != null)
            {
                dc.Disconnect();
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
                cfg = Configuration.loadConfig("IW4MAdmin.Discord.cfg");
            }           

            catch (ConfigException e)
            {
                dc.Log.Info("No usable configuration found", e);
                cfg = Configuration.createConfig();
                try
                {
                    Configuration.saveConfig(cfg);
                    dc.Log.Info("Saved new bot configuration", null);
                }

                catch (ConfigException ce)
                {
                    dc.Log.Error(ce.Message, ce);
                }
            }

            rc = RestConnection.Initialize(new Uri(cfg.IW4MAdminURI + ':' + cfg.IW4MAdminPort + "/api/events"));

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
            await dc.Connect(cfg.BotToken, TokenType.Bot);
            dc.Log.Info("IW4MAdmin.Discord has connected", null);
        }
    }
}
