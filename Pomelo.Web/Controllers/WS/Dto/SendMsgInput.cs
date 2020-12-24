using Pomelo.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Web.Controllers.WS.Dto
{
    public class SendMsgInput
    { 

        /// <summary>
        /// 接受者id
        /// </summary> 
        [Required]
        public string ToClientId { get; set; }

         

        /// <summary>
        /// 消息事件类型，比如链接成功为 Link，普通消息为 Msg，上下线消息 OnLine，OffLine 等
        /// </summary> 
        public string Type { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        public string Content { get; set; }
    }
}
