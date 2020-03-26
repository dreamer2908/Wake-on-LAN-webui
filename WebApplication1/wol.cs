using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Web;

namespace WebApplication1
{
    public static class wol
    {
        public static IPAddress wake(string mac, string ip, string subnet, int sendTo)
        {
            // clean up input to make sure it won't crash
            var macAddress = mac;
            macAddress = Regex.Replace(macAddress, "[-|:]", "");       // Remove any semicolons or minus characters present in our MAC address

            ip = sanitizeIpAddress(ip);
            subnet = sanitizeIpAddress(subnet);

            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                EnableBroadcast = true
            };

            int payloadIndex = 0;

            /* The magic packet is a broadcast frame containing anywhere within its payload 6 bytes of all 255 (FF FF FF FF FF FF in hexadecimal), followed by sixteen repetitions of the target computer's 48-bit MAC address, for a total of 102 bytes. */
            byte[] payload = new byte[1024];    // Our packet that we will be broadcasting

            // Add 6 bytes with value 255 (FF) in our payload
            for (int i = 0; i < 6; i++)
            {
                payload[payloadIndex] = 255;
                payloadIndex++;
            }

            // Repeat the device MAC address sixteen times
            for (int j = 0; j < 16; j++)
            {
                for (int k = 0; k < macAddress.Length; k += 2)
                {
                    var s = macAddress.Substring(k, 2);
                    payload[payloadIndex] = byte.Parse(s, NumberStyles.HexNumber);
                    payloadIndex++;
                }
            }

            // select the target address
            // sendTo 1: Broadcast address 255.255.255.255
            // sendTo 2: Broadcast address from IP and subnet mask
            // sendTo 3: IP address

            IPAddress target;
            switch (sendTo)
            {
                case 1:
                    target = IPAddress.Parse("255.255.255.255"); break;
                case 2:
                    target = getBroadcastAddress(IPAddress.Parse(ip), IPAddress.Parse(subnet)); break;
                case 3:
                    target = IPAddress.Parse(ip); break;
                default:
                    target = IPAddress.Parse("255.255.255.255"); break;
            }

            sock.SendTo(payload, new IPEndPoint(target, 7));  // Broadcast our packet
            sock.Close(10000);

            return target;
        }

        public static IPAddress getBroadcastAddress(this IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public static string sanitizeIpAddress(string ip)
        {
            string defaultIp = "255.255.255.255";
            try
            {
                IPAddress.Parse(ip);
            }
            catch (FormatException)
            {
                return defaultIp;
            }
            return ip;
        }
    }
}