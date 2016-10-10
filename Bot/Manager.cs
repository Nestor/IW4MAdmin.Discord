using System;
using IW4MAdmin.Discord.Debugging;

namespace IW4MAdmin.Discord
{
    class Bot
    {
        private static Bot _bot = null;

        public static Bot getBot()
        {
            if (_bot == null)
                _bot = new Bot();
            return _bot;
        }

        public ErrorMessage Start()
        {
            return new ErrorMessage ("No Error", ErrorCodes.NO_ERROR );
        }
    }
}
