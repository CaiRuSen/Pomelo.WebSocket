using System;
using System.Collections.Generic;
using System.Text;

namespace Pomelo.Model
{
    public class WebSocketConfigModel
    {
        public string Host { get; set; }

        public string MsgChannel { get; set; }

        public string OnOffLineChannel { get; set; }
    }
}
