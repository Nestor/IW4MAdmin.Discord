using System;
using System.Threading.Tasks;
using Discord;
using Discord.Modules;
using Discord.Commands;
using Discord.Commands.Permissions.Levels;

namespace IW4MAdmin.Discord
{
    class IW4MAdminModule : IModule
    {
        private ModuleManager mg;
        private DiscordClient dc;

        void IModule.Install(ModuleManager manager)
        {
            mg = manager;
            dc = manager.Client;


            mg.CreateCommands("", g =>
            {
                g.CreateCommand("status")
               .Description("Query all running servers")
               .Alias("s")
               .MinPermissions(1)
               .Do(e => { onStatusRequest(e); });




            });
        }


        private static async Task onStatusReceived(CommandEventArgs e, string content)
        {
            await e.User.SendMessage(content);
        }

        private static void onStatusRequest(CommandEventArgs e)
        {
            Query.RestConnection.getConn().makeRequest("", new System.Collections.Generic.Dictionary<string, object> { { "status", "true" } }, async response => { await onStatusReceived(e, response.Content); });
        }
    }
}
