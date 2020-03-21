using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class Sessions
    {
        public struct session
        {
            public string id { get; set; }
            public bool isLoggedIn { get; set; }
            public string username { get; set; }
        }

        private static List<session> sessions = new List<session>();

        public static bool readSession(string id, out session output)
        {
            foreach (var sec in sessions)
            {
                if (sec.id == id)
                {
                    output = sec;
                    return true;
                }
            }
            output = new session()
            {
                id = string.Empty,
                isLoggedIn = false,
                username = string.Empty
            };
            return false;
        }

        public static void writeSession(string id, bool isLoggIn, string username)
        {
            session ses = new session
            {
                id = id,
                isLoggedIn = isLoggIn,
                username = username
            };
            sessions.Add(ses);
        }

        public static void deleteSession(string id)
        {
            readSession(id, out session sec);
            deleteSession(sec);
        }

        public static void deleteSession(session sec)
        {
            sessions.Remove(sec);
        }
    }
}