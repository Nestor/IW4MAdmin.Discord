using System;

 class Start
{
    static void Main(string[] args) 
    { 
        var bot = IW4MAdmin.Discord.Bot.getBot();
        bot.Initialize();

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}

