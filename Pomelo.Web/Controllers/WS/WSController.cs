using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pomelo.Common.Helper;
using Pomelo.Enum.Base;
using Pomelo.Model;
using Pomelo.Model.Base;
using Pomelo.Web.Controllers.WS.Dto;
using Pomelo.Web.Middleware;

namespace Pomelo.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WSController : BaseController
    {
        private static WebSocketConfigModel config;
        private static AuthConfigModel auth;



        public WSController(ILogger<BaseController> logger) : base(logger)
        {
            config = GetWsConfig();
            auth = GetAuthConfig();
        }

        /// <summary>
        /// 获取ws链接
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResultOutput GetWSLink(GetWsServerInput input)
        {
            BaseResultOutput baseResultOutput = new BaseResultOutput();
 

            if (auth.IsCheck && (auth.AppId != input.AppId || auth.AppSecret != input.AppSecret))
            {
                baseResultOutput.Code = ResultCode.Error;
                baseResultOutput.Message = "AppId 或 AppSecret 错误";
                return baseResultOutput;
            }

            var token = $"{Guid.NewGuid()}{Guid.NewGuid()}".Replace("-", "");

            if (string.IsNullOrEmpty(input.ClientId))
            {
                baseResultOutput.Code = Enum.Base.ResultCode.Error;
                baseResultOutput.Message = "参数异常";
                return baseResultOutput;
            }

            //token值
            var token_value = new ImUserTokenModel()
            {
                ClientId = input.ClientId
            };

            //生成token校验数据
            RedisHelper.Set($"IM_Token_{token}", $"{StringHelper.ToJson(token_value)}", 60);

            //返回ws链接
            string host = "";
            if (Request.IsHttps)
            {
                host = $"wss://{config.Host}/imserver/ws?token={token}";
            }
            else
            {
                host = $"ws://{config.Host}/imserver/ws?token={token}";
            }



            var getWsServerOutput = new GetWsServerOutput()
            {
                Url = host
            };

            baseResultOutput.Data = getWsServerOutput;

            return baseResultOutput;
        }



        /// <summary>
        /// 测试-发送消息给目标用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResultOutput Test_ClientSendMsg(SendMsgInput input)
        {
            BaseResultOutput baseResultOutput = new BaseResultOutput();

            if (config == null)
            {
                config = GetWsConfig();
            } 

            RedisHelper.Publish(config.MsgChannel, StringHelper.ToJson(input));
           
            return baseResultOutput;

        }




        /// <summary>
        /// 在线用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResultOutput AllOnlineUser(AllOnlineUserInput input)
        {

            BaseResultOutput baseResultOutput = new BaseResultOutput();

        
            if (auth.IsCheck && (auth.AppId != input.AppId || auth.AppSecret != input.AppSecret))
            {
                baseResultOutput.Code = ResultCode.Error;
                baseResultOutput.Message = "AppId 或 AppSecret 错误";
                return baseResultOutput;
            }

            OnlineUserOutput onlineUserOutput = new OnlineUserOutput();

            var data = new ConcurrentDictionary<string, WebSocketListModel>();
            int num = 0;

            data = SocketHandle.webSocketList;
            num = data.Count();
            onlineUserOutput.Num = num;

            onlineUserOutput.OnlineUser = new List<OnlineUser>();
            foreach (var item in data)
            {
                onlineUserOutput.OnlineUser.Add(new OnlineUser
                {
                    ClientId = item.Key
                });
            }

            baseResultOutput.Data = onlineUserOutput;
            return baseResultOutput;
        }



        /// <summary>
        /// 用户是否在线
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResultOutput UserIsOnline(GetWsServerInput input)
        {

            BaseResultOutput baseResultOutput = new BaseResultOutput();
      
            if (auth.IsCheck && (auth.AppId != input.AppId || auth.AppSecret != input.AppSecret))
            {
                baseResultOutput.Code = ResultCode.Error;
                baseResultOutput.Message = "AppId 或 AppSecret 错误";
                return baseResultOutput;
            }

            GetOnlineUserOutput getOnlineUserOutput = new GetOnlineUserOutput();

            var webSocketListModel = new WebSocketListModel();

            var isOnLine = SocketHandle.webSocketList.TryGetValue(input.ClientId, out webSocketListModel);

            getOnlineUserOutput.IsOnLine = isOnLine;
            getOnlineUserOutput.ClientId = input.ClientId;

            baseResultOutput.Data = getOnlineUserOutput;
            return baseResultOutput;
        }






    }
}
