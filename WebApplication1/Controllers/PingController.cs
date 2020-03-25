using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class PingController : ApiController
    {
        public HttpResponseMessage Get(string ip)
        {
            string icon = "/Images/warning-32.png";
            var pingResult = common.singlePing(ip, 5000, 32, 128, out string replyAddr, out long replyTime, out int replyTtl);
            if (pingResult == common.pingStatus.online)
            {
                icon = "/Images/ok-32.png";
            }

            //var response = new HttpResponseMessage();
            //response.Content = new StringContent(string.Format("<!DOCTYPE html><html><body><div><img src='{0}' /></div></body></html>", icon));
            //response.Content = new StringContent(string.Format("<img width='32' height='32' src='{0}' />", icon));
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            // redirect to the correct icon
            var response = Request.CreateResponse(HttpStatusCode.Moved);
            string fullyQualifiedUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            response.Headers.Location = new Uri(new Uri(fullyQualifiedUrl), icon);
            return response;
        }
    }
}
