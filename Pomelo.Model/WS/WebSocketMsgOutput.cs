using Pomelo.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Model
{
    public class WebSocketMsgOutput
    { 
        public string Type { get; set; }

        public object Data { get; set; } = "";
    }
}
