using System;

namespace IW4MAdmin.Discord.Debugging
{
    class ConfigException : Exception
    {
        public ConfigException(string msg) : base(msg) { }
    }
}
