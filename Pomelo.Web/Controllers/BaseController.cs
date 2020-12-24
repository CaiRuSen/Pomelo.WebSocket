using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pomelo.Model;
using Pomelo.Web.Controllers.WS.Dto;

namespace Pomelo.Web.Controllers
{
    /// <summary>
    /// Base控制器
    /// </summary>
    public class BaseController : ControllerBase
    { 

        /// <summary>
        /// 日志
        /// </summary>
        protected readonly ILogger<BaseController> _Logger; 

        protected BaseController(ILogger<BaseController> logger)
        {
            _Logger = logger; 
        }
         
        /// <summary>
        /// 获取授权配置
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public AuthConfigModel GetAuthConfig()
        {
            return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetSection("Auth").Get<AuthConfigModel>();

        }

        [NonAction]
        public WebSocketConfigModel GetWsConfig()
        {
            return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build().GetSection("WebSocket").Get<WebSocketConfigModel>();

        }
    }
}