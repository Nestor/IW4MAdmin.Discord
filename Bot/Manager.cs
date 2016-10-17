using System;
using System.Threading.Tasks;
using Discord;
using IW4MAdmin.Discord.Debugging;
using IW4MAdmin.Discord.Query;
using Discord.Modules;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;

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
            dc.Log.Message += onRequestLog;
            dc.MessageReceived += onMessageReceived;

            try
            {
                dc.Log.Info("Manager", "Attempting to load configuration file", null);
                cfg = Configuration.loadConfig("IW4MAdmin.Discord.cfg");
            }           

            catch (ConfigException e)
            {
                dc.Log.Info("Manager", "No usable configuration found", e);
                cfg = Configuration.createConfig();
                try
                {
                    Configuration.saveConfig(cfg);
                    dc.Log.Info("Manager", "Saved new bot configuration", null);
                }

                catch (ConfigException ce)
                {
                    dc.Log.Error("Manager", "Could not save new configuration file", ce);
                    dc.Log.Error(ce.Message, ce);
                }
            }

            dc.UsingCommands(c => { c.PrefixChar = cfg.CommandPrefix; c.HelpMode = HelpMode.Public; })
            .UsingPermissionLevels( (u, c) => { return (int)PermissionHelper.getPermissions(u, c); })
            .UsingModules();

            dc.Log.Info("Manager", "Adding command modules", null);
            dc.AddModule<IW4MAdminModule>("IW4MAdmin", ModuleFilter.None);

            dc.Log.Info("Manager", "Initializing connection to IW4MAdmin API", null);
            rc = RestConnection.Initialize(new Uri(cfg.IW4MAdminURI + ':' + cfg.IW4MAdminPort + "/api/events"));

            try
            {
                dc.ExecuteAndWait(Connect);
            }

            catch (Exception e)
            {
                dc.Log.Error("Manager", "Could not connect!", e);
            }
        }

        void onRequestLog(object sender, LogMessageEventArgs e)
        {
            if (e.Exception != null)
                Logging.WriteLine(e.Message + "->" + e.Exception.Message);
            else
                Logging.WriteLine(e.Message);
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
