﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class WolController : ApiController
    {
        public string[] Get(string mac = "", string ip = "", string subnet = "", string username = "", string pcname = "", string sendto = "")
        {
            // http://localhost:54381/api/wol?mac=6C-F0-49-68-07-00&ip=172.21.160.244&subnet=255.255.255.0 
            //string mac = "6C-F0-49-68-07-00";
            //string ip = "172.21.160.244";
            //string subnet = "255.255.255.0";

            var timestamp = DateTime.Now;

            // check if sendto paramenter is valid
            // get it from the database if not
            bool valid = false;
            if (int.TryParse(sendto, out int t))
            {
                if (t > 0 && t <= 3)
                {
                    valid = true;
                }
            }
            string sendToMode = valid ? sendto : common.readSettingDatabase_sendTo();

            IPAddress usedAddress = wol.wake(mac, ip, subnet, sendToMode);
            common.writeLog(timestamp, username, "Wake-on-LAN", string.Format("pcname = {0}, ip = {1}, subnet = {2}, mac = {3}", pcname, ip, subnet, mac));

            return new string[]
            {
                mac,
                usedAddress.ToString()
            };
        }
    }
}
