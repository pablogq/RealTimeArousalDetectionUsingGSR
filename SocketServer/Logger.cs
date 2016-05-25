using SocketServer.Properties;
using System;
using System.IO;

namespace SocketServer
{
    public class Logger
    {
        public static void Log(object logMessage)
        {
            String currentDate = DateTime.Now.ToString("yyyyMMdd");
            String logFileName = Settings.Default.LogFile.Replace(".txt", currentDate + ".txt");
            if (Directory.Exists(logFileName.Substring(0, logFileName.LastIndexOf('/'))))
            {
                using (StreamWriter w = File.AppendText(logFileName))
                {
                    logMessage = logMessage.ToString();
                    w.Write("\r\nLog Entry : ");
                    w.Write("{0} {1}", DateTime.Now.ToLongTimeString(),
                        DateTime.Now.ToLongDateString());
                    w.WriteLine("  :{0}", logMessage);
                    w.WriteLine("-------------------------------");
                }
            }
        }
    }
}
