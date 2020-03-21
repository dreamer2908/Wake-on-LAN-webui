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
            var pingResult = singlePing(ip, 5000, 32, 128, out string replyAddr, out long replyTime, out int replyTtl);
            if (pingResult == pingStatus.online)
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



        public enum pingStatus
        {
            online = 0,
            timeout = 1,
            unreachable = 2,
            ttlExpired = 3,
            generalFailure = 4,
            invalid = -1,
            none = -2,
        }

        public static readonly Dictionary<pingStatus, string> pingStatusToText = new Dictionary<pingStatus, string>()
        {
            { pingStatus.online, "Online" },
            { pingStatus.timeout, "Timed out" },
            { pingStatus.unreachable, "Unreachable" },
            { pingStatus.ttlExpired, "TTL expired" },
            { pingStatus.generalFailure, "General failure" },
            { pingStatus.invalid, "Invalid" },
            { pingStatus.none, string.Empty },
        };

        public static readonly Dictionary<string, pingStatus> textToPingStatus = new Dictionary<string, pingStatus>()
        {
            { "Online", pingStatus.online },
            { "Timed out", pingStatus.timeout },
            { "Unreachable", pingStatus.unreachable },
            { "TTL expired", pingStatus.ttlExpired },
            { "General failure", pingStatus.generalFailure },
            { "Invalid", pingStatus.invalid },
            { string.Empty, pingStatus.none },
        };

        private static readonly Dictionary<IPStatus, pingStatus> ipStatusToPingStatus = new Dictionary<IPStatus, pingStatus>()
        {
            {IPStatus.BadDestination, pingStatus.generalFailure},
            {IPStatus.BadHeader, pingStatus.generalFailure},
            {IPStatus.BadOption, pingStatus.generalFailure},
            {IPStatus.BadRoute, pingStatus.generalFailure},
            {IPStatus.DestinationHostUnreachable, pingStatus.unreachable},
            {IPStatus.DestinationNetworkUnreachable, pingStatus.unreachable},
            {IPStatus.DestinationPortUnreachable, pingStatus.unreachable},
            //{IPStatus.DestinationProhibited, pingStatus.unreachable}, // DestinationProhibited & DestinationProtocolUnreachable have the same value
            {IPStatus.DestinationProtocolUnreachable, pingStatus.unreachable},
            {IPStatus.DestinationScopeMismatch, pingStatus.unreachable},
            {IPStatus.DestinationUnreachable, pingStatus.unreachable},
            {IPStatus.HardwareError, pingStatus.generalFailure},
            {IPStatus.IcmpError, pingStatus.generalFailure},
            {IPStatus.NoResources, pingStatus.generalFailure},
            {IPStatus.PacketTooBig, pingStatus.generalFailure},
            {IPStatus.ParameterProblem, pingStatus.generalFailure},
            {IPStatus.SourceQuench, pingStatus.generalFailure},
            {IPStatus.Success, pingStatus.online},
            {IPStatus.TimedOut, pingStatus.timeout},
            {IPStatus.TimeExceeded, pingStatus.ttlExpired},
            {IPStatus.TtlExpired, pingStatus.ttlExpired},
            {IPStatus.TtlReassemblyTimeExceeded, pingStatus.ttlExpired},
            {IPStatus.Unknown, pingStatus.generalFailure},
            {IPStatus.UnrecognizedNextHeader, pingStatus.generalFailure}
        };

        public static pingStatus singlePing(string hostname, int timeout, int bufferSize, int ttl, out string replyAddr, out long replyTime, out int replyTtl)
        {
            // Create a new instant
            Ping pingSender = new Ping();

            // Create a buffer of <bufferSize> bytes of data to be transmitted.
            byte[] buffer = new byte[bufferSize];
            //Random rnd = new Random();
            //rnd.NextBytes(buffer);

            // Set options for transmission:
            // The data can go through <ttl> gateways or routers before it is destroyed, and the data packet can be fragmented.
            PingOptions options = new PingOptions(ttl, false);

            // True work here
            PingReply reply = null;
            pingStatus pingResult;

            try
            {
                reply = pingSender.Send(hostname, timeout, buffer, options);
            }
            catch (System.Net.NetworkInformation.PingException)
            {
            }

            // Now compiling result
            pingResult = (reply != null) ? ipStatusToPingStatus[reply.Status] : pingStatus.generalFailure;
            replyAddr = (reply != null && reply.Address != null) ? reply.Address.ToString() : string.Empty; // address is null when timeout
            replyTime = (reply != null && reply.Status == IPStatus.Success) ? reply.RoundtripTime : 0;
            replyTtl = (reply != null && reply.Options != null) ? reply.Options.Ttl : 0; // options is null when unreachable

            return pingResult;
        }
    }
}
