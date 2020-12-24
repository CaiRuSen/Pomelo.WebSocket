using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Web.Controllers.WS.Dto
{
    public class GetWsServerInput
    {
        /// <summary>
        /// 用于负载均衡时候的服务Id
        /// </summary>
        public string ClientId { get; set; }

        public string AppId  { get; set; }

        public string AppSecret { get; set; }
    }
}
