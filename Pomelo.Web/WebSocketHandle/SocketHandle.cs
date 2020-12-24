using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pomelo.Common.Helper;
using Pomelo.Enum;
using Pomelo.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pomelo.Web.Middleware
{
    public class SocketHandle
    {
        public static ConcurrentDictionary<string, WebSocketListModel> webSocketList = new ConcurrentDictionary<string, WebSocketListModel>();
        private static WebSocketConfigModel config;

        public const int BufferSize = 1024 * 4;
        WebSocket socket;
        string clientId;

        bool isSubscribe = false;
        SocketHandle(WebSocket socket, string clientId)
        {
            this.socket = socket;
            this.clientId = clientId;

            if (!isSubscribe)
            {
                isSubscribe = true;

                config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetSection("WebSocket").Get<WebSocketConfigModel>();
                RedisHelper.Subscribe((config.MsgChannel, RedisSubScribleMessage));
            }
        }


        async Task EchoLoop()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);

            await SendMsg(this.socket, WebSocketMsgType.Link.ToString(), "连接成功");
            try
            {
                while (this.socket.State == WebSocketState.Open)
                {
                    WebSocketMsgOutput webSocketMsgModel = new WebSocketMsgOutput();

                    var incoming = await this.socket.ReceiveAsync(seg, CancellationToken.None);

                }

                this.socket.Abort();
            }
            catch (Exception e)
            {
                var outgoing = Encoding.UTF8.GetBytes(e.Message);
                await SendMsg(this.socket, WebSocketMsgType.Msg.ToString(), outgoing);
            }

            //移除在线ws记录
            RemoveWebsocket(this.clientId);

        }



        static async Task Acceptor(HttpContext context, Func<Task> n)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                string token = context.Request.Query["token"];
                if (string.IsNullOrEmpty(token)) return;

                //校验token有效性（token从业务端请求进行生成，服务端进行校验）
                var token_value_str = RedisHelper.Get($"IM_Token_{token}");
                if (string.IsNullOrEmpty(token_value_str)) return;

                var token_value = StringHelper.Json2Object<ImUserTokenModel>(token_value_str);

                var socket = await context.WebSockets.AcceptWebSocketAsync();

                var h = new SocketHandle(socket, token_value.ClientId);

                AddWebsocket(token_value.ClientId, socket);

                await h.EchoLoop();
            }
            else
            {
                context.Response.StatusCode = 400;
            }

        }


        /// <summary>
        /// 路由绑定处理
        /// </summary>
        /// <param name="app"></param>
        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(60),//向客户端发送的ping频率，保持连接处于打开状态
                ReceiveBufferSize = 4 * 1024//接收数据缓冲区大小
            });

            app.Use(SocketHandle.Acceptor);
        }




        /// <summary>
        /// 添加ws在线记录
        /// </summary>
        /// <param name="clientId"></param> 
        /// <param name="webSocket"></param>
        private static void AddWebsocket(string clientId, WebSocket webSocket)
        {
            var data = webSocketList.Count(p => p.Key == clientId);
            if (data == 0)
            {
                webSocketList.GetOrAdd(clientId, cli => new WebSocketListModel { WebSocket = webSocket });
            }
            else
            {
                //如果连接已存在，将已存在的移除，建立新的
                RemoveWebsocket(clientId);
                webSocketList.GetOrAdd(clientId, cli => new WebSocketListModel { WebSocket = webSocket });
            }

            //订阅上线通知
            var onlinedata = new
            {
                Type = WebSocketMsgType.OnLine.ToString(),
                Client = clientId
            };


            RedisHelper.Publish(config.OnOffLineChannel, StringHelper.ToJson(onlinedata));
        }


        /// <summary>
        /// 移除ws在线记录
        /// </summary>
        /// <param name="clientId"></param>
        private static void RemoveWebsocket(string clientId)
        {
            webSocketList.TryRemove(clientId, out var oldcli);

            //订阅下线通知
            var data = new
            {
                Type = WebSocketMsgType.OffLine.ToString(),
                Client = clientId //下线的设备id
            };
            RedisHelper.Publish(config.OnOffLineChannel, StringHelper.ToJson(data));
        }



        /// <summary>
        /// ws消息发送
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="type"></param>
        /// <param name="messageData"></param>
        /// <returns></returns>
        public static async Task<bool> SendMsg(WebSocket webSocket, string type, object messageData)
        { 
            WebSocketMsgOutput webSocketMsgModel = new WebSocketMsgOutput();

            webSocketMsgModel.Type = type;
            webSocketMsgModel.Data = messageData;

            var message = Encoding.UTF8.GetBytes(StringHelper.ToJson(webSocketMsgModel));
            await webSocket.SendAsync(message, WebSocketMessageType.Text, true, CancellationToken.None);

            return true;

        }





        /// <summary>
        /// Redis消息订阅
        /// </summary>
        /// <param name="e"></param>
        void RedisSubScribleMessage(CSRedis.CSRedisClient.SubscribeMessageEventArgs e)
        {
            try
            {
                var data = StringHelper.Json2Object<WebSocketMsgInput>(e.Body);
                var outgoing = new ArraySegment<byte>(Encoding.UTF8.GetBytes(data.Content));


                var toClientSocket = webSocketList.GetValueOrDefault(data.ToClientId);
                if (toClientSocket != null)
                {
                    var result = SendMsg(toClientSocket.WebSocket, data.Type, data.Content).Result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
