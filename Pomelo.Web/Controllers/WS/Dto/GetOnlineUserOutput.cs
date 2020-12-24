using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Web.Controllers.WS.Dto
{
    public class GetOnlineUserOutput
    {
        public bool IsOnLine { get; set; }

        public string ClientId { get; set; }
    }
}
