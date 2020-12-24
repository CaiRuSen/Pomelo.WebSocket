using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Web.Controllers.WS.Dto
{
    public class OnlineUserOutput
    {
        public int Num { get; set; }
        public List<OnlineUser> OnlineUser { get; set; }
    }

    public class OnlineUser
    {
        public string ClientId { get; set; }
         
    }
}
