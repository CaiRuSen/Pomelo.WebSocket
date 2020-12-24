using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Pomelo.Model
{
    public class WebSocketListModel
    {

        public string ClientId { get; set; }

        public  WebSocket WebSocket { get; set; }
    }
}
