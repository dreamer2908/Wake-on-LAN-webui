using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class WolController : ApiController
    {
        public string[] Get(string mac, string ip, string subnet)
        {
            // http://localhost:54381/api/wol?mac=6C-F0-49-68-07-00&ip=172.21.160.244&subnet=255.255.255.0 
            //string mac = "6C-F0-49-68-07-00";
            //string ip = "172.21.160.244";
            //string subnet = "255.255.255.0";

            wol.wake(mac, ip, subnet);

            return new string[]
            {
                mac,
                ip,
                subnet
            };
        }
    }
}
