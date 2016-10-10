using System;
using System.IO;
using Newtonsoft.Json;

namespace IW4MAdmin.Discord
{
    class Configuration
    {
        public struct ConfigFile
        {
            public char CommandPrefix;
            public string BotToken;
            public string IW4MAdminURI;
            public short IW4MAdminPort;
        }

        public static ConfigFile loadConfig(string fileName)
        {
            ConfigFile cfg;
            try
            {
                string configText = File.ReadAllText(fileName);
                cfg = (ConfigFile)JsonConvert.DeserializeObject(configText, typeof(ConfigFile));
            }

            catch (ArgumentNullException)
            {
                throw new Debugging.ConfigException("Invalid filename");
            }

            catch (FileNotFoundException)
            {
                throw new Debugging.ConfigException("Configuration file not found");
            }


            catch (InvalidCastException)
            {
                throw new Debugging.ConfigException("Configuration file not found");
            }

            return cfg;
        }

        public static void saveConfig(ConfigFile cfg)
        {
            try
            {
                string configText = JsonConvert.SerializeObject(cfg);
                File.WriteAllText("IW4MAdmin.Discord.cfg", configText);
            }

            catch (IOException)
            {
                throw new Debugging.ConfigException("Could not write configuration file");
            }
        }

        public static ConfigFile createConfig()
        {
            var cfg = new ConfigFile();
            while (cfg.BotToken == null || cfg.BotToken.Length == 0)
            {
                Console.Write("Enter your Discord bot's API Token: ");
                cfg.BotToken = Console.ReadLine();
            }

            while (cfg.IW4MAdminURI == null || cfg.IW4MAdminURI.Length == 0)
            {
                Console.Write("Enter your IW4MAdmin's web address (ie http://nbsclan.org): ");
                cfg.IW4MAdminURI = Console.ReadLine();
            }

            cfg.IW4MAdminPort = 0;

            while (cfg.IW4MAdminPort == 0)
            {
                Console.Write("Enter your IW4MAdmin's port (ie 8080): ");
                short.TryParse(Console.ReadLine(), out cfg.IW4MAdminPort);
            }

            cfg.CommandPrefix = '\0';
            
            while (cfg.CommandPrefix == '\0')
            {
                Console.Write("Enter your command prefix: ");
                string key = Console.ReadLine();
                if (key != null && key.Length == 1 && key[0] != '\r')
                    cfg.CommandPrefix = key[0];
            }

            return cfg;
        }
    }
}
