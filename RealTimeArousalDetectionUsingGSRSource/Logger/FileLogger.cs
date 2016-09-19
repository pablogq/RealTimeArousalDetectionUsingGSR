using System;
using System.Configuration;
using System.IO;

    public class FileLogger : LogBase
    {
        public override void Log(string logMessage)
        {
            String currentDate = DateTime.Now.ToString("yyyyMMdd");
            String logFileName = ConfigurationManager.AppSettings.Get("LogFile").Replace(".txt", currentDate + ".txt");

            lock (lockObj)
            {
                if (Directory.Exists(logFileName.Substring(0, logFileName.LastIndexOf('/'))) || Directory.Exists(logFileName.Substring(0, logFileName.LastIndexOf('\\'))))
                {
                    using (StreamWriter streamWriter = File.AppendText(logFileName))
                    {
                        streamWriter.Write("\r\nLog Entry : ");
                        streamWriter.Write("{0} {1}", DateTime.Now.ToLongTimeString(),
                           DateTime.Now.ToLongDateString());
                        streamWriter.WriteLine("  :{0}", logMessage);
                        streamWriter.WriteLine("-------------------------------");
                        streamWriter.Close();
                    }
                }
            }
        }
    }
