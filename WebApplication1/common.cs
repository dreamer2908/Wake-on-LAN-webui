using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;

namespace WebApplication1
{
    public class common
    {
        // query the database, return the number of rows affected, and output a datatable
        public static int queryDatabase(SqlCommand cmd, out DataTable dt)
        {
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            cmd.Connection = con;
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            con.Open();
            int i = cmd.ExecuteNonQuery();
            con.Close();

            return i;
        }

        public static bool getIpfromHost(string host, out string ip)
        {
            IPHostEntry hostEntry = null;

            try
            {
                hostEntry = Dns.GetHostEntry(host);
            }
            catch (System.Net.Sockets.SocketException)
            {
            }

            if (hostEntry != null)
            {
                // get the first IPv4 address
                foreach (var entry in hostEntry.AddressList)
                {
                    if (entry.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ip = entry.ToString();
                        return true;
                    }
                }
            }

            ip = null;
            return false;
        }

        public static bool getMacFromIp(string ip, out string mac)
        {
            // ping first to get it into arp table
            singlePing(ip, 5000, 32, 128, out string replyAddr, out long replyTime, out int replyTtl);
            // then parse arp to get its mac
            mac = getMacByIp(ip);

            return (mac != null);
        }

        public static string getMacByIp(string ip)
        {
            var macIpPairs = GetAllMacAddressesAndIppairs();
            int index = macIpPairs.FindIndex(x => x.IpAddress == ip);
            if (index >= 0)
            {
                return macIpPairs[index].MacAddress.ToUpper();
            }
            else
            {
                return null;
            }
        }

        public static List<MacIpPair> GetAllMacAddressesAndIppairs()
        {
            List<MacIpPair> mip = new List<MacIpPair>();
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            pProcess.StartInfo.FileName = "arp";
            pProcess.StartInfo.Arguments = "-a ";
            pProcess.StartInfo.UseShellExecute = false;
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.CreateNoWindow = true;
            pProcess.Start();
            string cmdOutput = pProcess.StandardOutput.ReadToEnd();
            string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})";

            foreach (Match m in Regex.Matches(cmdOutput, pattern, RegexOptions.IgnoreCase))
            {
                mip.Add(new MacIpPair()
                {
                    MacAddress = m.Groups["mac"].Value,
                    IpAddress = m.Groups["ip"].Value
                });
            }

            return mip;
        }

        public struct MacIpPair
        {
            public string MacAddress;
            public string IpAddress;
        }

        /// <summary>
        /// Ping stuff
        /// </summary>
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

        #region Log
        public static int writeLog(DateTime d, string username, string action, string detail)
        {
            // SqlCommand cmd = new SqlCommand("DECLARE @Offset datetimeoffset = sysdatetimeoffset(); INSERT INTO Log ([timestamp], [ip], [username], [action], [detail]) VALUES (@Offset, @ip, @username, @action, @detail)");
            SqlCommand cmd = new SqlCommand("INSERT INTO Log ([timestamp], [ip], [username], [action], [detail]) VALUES (@timestamp, @ip, @username, @action, @detail)");
            cmd.Parameters.AddWithValue("@timestamp", formatDateTime(d));
            cmd.Parameters.AddWithValue("@ip", getRequestIPAddress());
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@action", action);
            cmd.Parameters.AddWithValue("@detail", detail);

            int rows = common.queryDatabase(cmd, out DataTable dt);
            return rows;
        }

        public static int writeLog(string username, string action, string detail)
        {
            return writeLog(DateTime.Now, username, action, detail);
        }

        public static string getRequestIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public static string getNowStringForFilename()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        }

        public static string datetimeCommonFormatString = "yyyy-MM-dd HH:mm:ss";
        public static string getNowString()
        {
            return formatDateTime(DateTime.Now);
        }

        public static string formatDateTime(DateTime d)
        {
            return d.ToString(datetimeCommonFormatString);
        }
        #endregion

        #region Settings from database
        public static void writeSettingDatabase(string key, string val)
        {
            SqlCommand cmd = new SqlCommand(@"
declare @kye varchar(50)
declare @val varchar(50)
set @kye = @k
set @val = @v

IF (NOT EXISTS(SELECT * FROM Settings WHERE [key] = @kye)) 
BEGIN
    INSERT INTO Settings ([key], [value]) VALUES (@kye, @val)
END 
ELSE 
BEGIN 
    UPDATE Settings 
    SET [value] = @val
    WHERE [key] = @kye
END ");
            cmd.Parameters.AddWithValue("@k", key);
            cmd.Parameters.AddWithValue("@v", val);

            int rows = common.queryDatabase(cmd, out DataTable dt);
        }

        public static void writeSettingDatabase(string key, int val)
        {
            writeSettingDatabase(key, val.ToString());
        }

        public static void writeSettingDatabase(string key, bool val)
        {
            writeSettingDatabase(key, val.ToString());
        }

        public static string readSettingDatabase(string key, string defVal)
        {
            string val = defVal;
            bool valid = false;

            // get the "value" by "key" in table Settings
            SqlCommand cmd = new SqlCommand("SELECT [key], [value] FROM Settings WHERE [key] = @kye;");
            cmd.Parameters.AddWithValue("@kye", key);
            int rows = common.queryDatabase(cmd, out DataTable dt);

            if (dt.Rows.Count > 0)
            {
                val = dt.Rows[0].ItemArray[1].ToString();
                valid = (val != null);
            }

            if (valid)
            {
                return val;
            }
            else
            {
                // write back the default value
                writeSettingDatabase(key, defVal.ToString());
                return defVal;
            }
        }

        public static int readSettingDatabase(string key, int defVal, int minVal, int maxVal)
        {
            string strVal = readSettingDatabase(key, defVal.ToString());
            int val = defVal;

            // check if the value is a valid int and within min and max
            bool isInt = int.TryParse(strVal, out val);
            if (isInt && val <= maxVal && val >= minVal)
            {
                return val;
            }
            else
            {
                // if not write back the default value
                writeSettingDatabase(key, defVal.ToString());
                return defVal;
            }
        }

        public static bool readSettingDatabase(string key, bool defVal)
        {
            string strVal = readSettingDatabase(key, defVal.ToString());

            if (strVal == true.ToString())
            {
                return true;
            }
            else if (strVal == false.ToString())
            {
                return false;
            }
            else
            {
                writeSettingDatabase(key, defVal.ToString());
                return defVal;
            }
        }
        #endregion

        #region Password
        public static string getSha1HashFromText(string text)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] temp = sha1.ComputeHash(Encoding.UTF8.GetBytes(text));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < temp.Length; i++)
            {
                sb.Append(temp[i].ToString("x2"));
            }

            return sb.ToString();
        }
        #endregion


        #region email
        static string email_host = "";
        static int email_port = 25;
        static bool email_ssl = false;
        static string email_from = "";
        static string email_user = "";
        static bool email_login = true;
        static string email_password = "";
        static List<string> email_to = new List<string>();

        public static void readEmailSenderParamenter()
        {
            email_from = common.readSettingDatabase("email_from", "");
            email_host = common.readSettingDatabase("email_host", "");
            email_port = common.readSettingDatabase("email_port", 25, 0, 65536);
            email_ssl = common.readSettingDatabase("email_ssl", false);
            email_login = common.readSettingDatabase("email_login", true);
            email_user = common.readSettingDatabase("email_user", "");
            email_password = common.readSettingDatabase("email_password", "");

            string email_tos = common.readSettingDatabase("email_to", "");
            splitMultivalueSettingStringToList(email_tos, separatorComma, email_to);
        }

        public static void setEmailSenderParamenter(string _email_host, int _email_port, bool _email_ssl, string _email_from, string _email_user, string _email_password)
        {
            email_host = _email_host;
            email_port = _email_port;
            email_ssl = _email_ssl;
            email_from = _email_from;
            email_user = _email_user;
            email_password = _email_password;
        }

        // send with above paramenters
        public static void sendEmail(string email_subject, string email_body, List<string> attachments = null)
        {
            sendEmail(email_host, email_port, email_ssl, email_from, email_user, email_password, email_to, email_subject, email_body, attachments);
        }

        // single receipent
        public static void sendEmail(string email_to, string email_subject, string email_body, List<string> attachments = null)
        {
            sendEmail(email_host, email_port, email_ssl, email_from, email_user, email_password, email_to, email_subject, email_body, attachments);
        }

        // multiple receipents
        public static void sendEmail(List<string> email_to, string email_subject, string email_body, List<string> attachments = null)
        {
            sendEmail(email_host, email_port, email_ssl, email_from, email_user, email_password, email_to, email_subject, email_body, attachments);
        }

        // single receipent
        public static void sendEmail(string email_host, int email_port, bool email_ssl, string email_from, string email_user, string email_password, string email_to, string email_subject, string email_body, List<string> attachments = null)
        {
            List<string> to = new List<string>();
            to.Add(email_to);
            sendEmail(email_host, email_port, email_ssl, email_from, email_user, email_password, to, email_subject, email_body, attachments);
        }

        // multiple receipents
        public static void sendEmail(string email_host, int email_port, bool email_ssl, string email_from, string email_user, string email_password, List<string> email_to, string email_subject, string email_body, List<string> attachments = null)
        {
            using (SmtpClient SmtpServer = new SmtpClient(email_host))
            {
                using (MailMessage mail = new MailMessage())
                {
                    try
                    {
                        mail.From = new MailAddress(email_from);
                        foreach (string em in email_to)
                        {
                            mail.To.Add(em);
                        }
                        mail.Subject = email_subject;
                        mail.IsBodyHtml = true;
                        mail.Body = convertTextToHtml(email_body);
                        mail.BodyEncoding = UTF8Encoding.UTF8;

                        // attach files
                        if (attachments != null)
                        {
                            foreach (var filename in attachments)
                            {
                                mail.Attachments.Add(new Attachment(filename));
                                // MessageBox.Show("Added attachment to email");
                            }
                        }

                        SmtpServer.Port = email_port;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(email_user, email_password);
                        SmtpServer.EnableSsl = email_ssl;

                        SmtpServer.Send(mail);

                        // MessageBox.Show("mail Send");
                        Console.WriteLine("mail Send");
                    }
                    catch (Exception ex)
                    {
                        // MessageBox.Show(ex.ToString());
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
        }

        private static string convertTextToHtml(string input)
        {
            string[] lines = splitLines(input);

            StringBuilder sb = new StringBuilder();

            foreach (string text in lines)
            {
                string encoded = System.Net.WebUtility.HtmlEncode(text);
                sb.AppendLine("<pre>" + encoded + "</pre>");
            }

            return sb.ToString();
        }

        public static string[] splitLines(string text)
        {
            string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            return lines;
        }

        public static string[] customSplitLines(string text)
        {
            List<string> result = new List<string>();

            string empty = " "; // workaround for Outlook ignoring totally empty line

            string thisLine = empty;
            int i = 0;
            while (i < text.Length)
            {
                if (text[i] == '\n')
                {
                    result.Add(thisLine);
                    thisLine = empty;
                    i++;
                }
                else if (text[i] == '\r')
                {
                    result.Add(thisLine);
                    thisLine = empty;

                    if (text[i + 1] == '\n')
                    {
                        i += 2;
                    }
                    else
                    {
                        i += 1;
                    }
                }
                else
                {
                    thisLine = thisLine + text[i].ToString();
                    i++;
                }
            }

            return result.ToArray();
        }

        public static bool splitKeyValue(string line, ref string key, ref string value)
        {
            string[] parts = line.Split(new string[] { "=" }, 2, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                key = parts[0];
                value = parts[1];
                return true;
            }
            return false;
        }

        private static string[] separatorComma = new string[] { "," };
        public static void splitMultivalueSettingStringToList(string source, string[] separator, List<string> target)
        {
            target.Clear();
            string[] array = source.Split(separator, StringSplitOptions.None);
            for (int i = 0; i < array.Length; i++)
            {
                string sub = array[i].Trim();
                if (sub.Length > 0)
                {
                    target.Add(sub);
                }
            }
        }
        #endregion
    }
}