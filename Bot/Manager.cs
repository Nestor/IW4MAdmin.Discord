using System;
using System.Collections.Generic;
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
        private List<Server> connectedServers;
        private LinkedList<string> eventQueue;
        private Message eventMessage;
        private int messagesReceived = 0;

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
            connectedServers = new List<Server>();
            eventQueue = new LinkedList<string>();
            dc = new DiscordClient();
            dc.Log.Message += onRequestLog;
            dc.MessageReceived += onMessageReceived;
            dc.ServerAvailable += (s, e) => { dc.Log.Info("Manager", "Connected to server " + e.Server.Id, null); connectedServers.Add(e.Server); };

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
                Task.Run(queryAPI);
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
                Logging.WriteLine(e.Message + " -> " + e.Exception.Message);
            else
                Logging.WriteLine(e.Message);
        }

        async void onMessageReceived(object sender, MessageEventArgs e)
        {
            messagesReceived++;

            if (messagesReceived > 5)
            {
                eventMessage = await connectedServers[0].DefaultChannel.SendMessage(" ");
                messagesReceived = 0;
            }
        }

        async Task Connect()
        {
            dc.Log.Info("Connect", "IW4MAdmin.Discord is connecting to Discord API", null);
            await dc.Connect(cfg.BotToken, TokenType.Bot);
        }

        private async Task queryAPI()
        {
            while (true)
            {
                if(connectedServers.Count > 0)
                    rc.makeRequest("", null, async response => { if (response == null) return;  await onEventReceived(response.Content); });
                await Task.Delay(5000);
            }
        }

        private async Task onEventReceived(string content)
        {
            RestEvent e;
            try
            {
                e = EventDeserialize.deserializeEvent(content);
            }

            catch (NullReferenceException)
            {
                return;
            }

            switch (e.Type)
            {
                case RestEvent.eType.NOTIFICATION:
                    if (e.Title == "Disconnect")
                        eventQueue.AddLast("```Diff\n-" + e.Target + " has disconnected\n```");
                    else if (e.Title == "Connect")
                        eventQueue.AddLast("```Diff\n+" + e.Target + " has connected\n```");
                    else if (e.Title == "Chat")
                        eventQueue.AddLast("```Apache\n" + e.Origin + ": \"" + e.Message + "\"\n```");
                    break;
                case RestEvent.eType.ALERT:
                    await connectedServers[0].DefaultChannel.SendMessage(e.Message + " - " + e.Origin + " @here");
                    break;
            }

            if (eventQueue.Count > 8)
                eventQueue.RemoveFirst();

            string queueString = "";
            var enumer = eventQueue.GetEnumerator();
            while (enumer.MoveNext())
                queueString += enumer.Current + "\n";

            if (eventMessage == null)
                eventMessage = await connectedServers[0].DefaultChannel.SendMessage(" ");
           // await eventMessage.Edit(queueString);
        }
    }
}
