/*
 * Copyright 2016 Sofia University
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * This project has received funding from the European Union's Horizon
 * 2020 research and innovation programme under grant agreement No 644187.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
