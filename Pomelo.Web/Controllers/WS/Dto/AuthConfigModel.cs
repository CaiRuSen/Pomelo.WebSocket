using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Web.Controllers.WS.Dto
{
    public class AuthConfigModel
    {
        public bool IsCheck { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
}
