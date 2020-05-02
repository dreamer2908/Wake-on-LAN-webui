using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class Ping2Controller : ApiController
    {
        public int Get(string ip)
        {
            var pingResult = common.singlePing(ip, 5000, 32, 128, out string replyAddr, out long replyTime, out int replyTtl);
            return (pingResult == common.pingStatus.online) ? 0 : 1;
        }
    }
}
