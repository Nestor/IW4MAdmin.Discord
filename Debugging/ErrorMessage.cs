using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
