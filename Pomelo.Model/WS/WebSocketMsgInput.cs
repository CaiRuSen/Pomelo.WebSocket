using Pomelo.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pomelo.Model
{
    public class WebSocketMsgInput
    {
        public string ToClientId { get; set; }
         
        public string Type { get; set; }


        public string Content { get; set; }
    }
}
