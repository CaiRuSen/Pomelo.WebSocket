using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Pomelo.Enum
{
    public enum WebSocketMsgType
    {

        [Description("连接通知")]
        Link,

        [Description("消息内容")]
        Msg,


        [Description("上线通知")]
        OnLine,

        [Description("下线通知")]
        OffLine,
    }
}
