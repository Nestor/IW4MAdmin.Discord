using System;

namespace IW4MAdmin.Discord.Debugging
{
    struct ErrorMessage
    {
        public string errMessage;  // { get;  private set; }
        public ErrorCodes errCode; // { get; private set; }

        public ErrorMessage(string eM, ErrorCodes eC)
        {
            errMessage = eM;
            errCode = eC;
        }
    }
}
