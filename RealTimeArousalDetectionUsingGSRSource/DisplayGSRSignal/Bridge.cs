/*
 * Copyright 2016-2018 Sofia University
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

using AssetPackage;
using System;
using System.IO;
using Assets.Rage.RealTimeArousalDetectionUsingGSRAsset.Utils;


namespace Assets.Rage.RealTimeArousalDetectionUsingGSRAsset.DisplayGSRSignal
{
    public class Bridge : IBridge, ILog, IDataStorage
    {
        public string IDataStoragePath;
        protected readonly object lockObj = new object();

        #region IDataStorage

        public Bridge()
        {
            IDataStoragePath = String.Format(@".{0}Resources{0}", Path.DirectorySeparatorChar);
        }

        public bool Delete(string fileId)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string fileId)
        {
            string filePath = IDataStoragePath + fileId;
            return (File.Exists(filePath));
        }

        public string[] Files()
        {
            throw new NotImplementedException();
        }

        public string GetPath(string fileId)
        {
            return IDataStoragePath + fileId;
        }

        public string Load(string fileId)
        {
            string filePath = IDataStoragePath + fileId;
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    return (line);
                }
            }
            catch (Exception e)
            {
                Log(Severity.Error, e.Message);
            }

            return (null);
        }

        public void Save(string fileId, string fileData)
        {
            string filePath = IDataStoragePath + fileId;
            using (StreamWriter file = new StreamWriter(filePath))
            {
                file.Write(fileData);
            }
        }

        #endregion IDataStorage
        #region ILog

        public void Log(Severity severity, string logMessage)
        {
            String currentDate = DateTime.Now.ToString("yyyyMMdd");
            String logFileName = RealTimeArousalDetectionAssetSettings.Instance.LogFile.Replace(".txt", currentDate + ".txt");

            lock (lockObj)
            {
                if ((logFileName.LastIndexOf('/') != -1 && Directory.Exists(logFileName.Substring(0, logFileName.LastIndexOf('/')))) || 
					(logFileName.LastIndexOf('\\') != -1 && Directory.Exists(logFileName.Substring(0, logFileName.LastIndexOf('\\')))))
                {
                    using (StreamWriter streamWriter = File.AppendText(logFileName))
                    {
                        streamWriter.Write("\r\nLog Entry (type {0}) : ", severity.ToString());
                        streamWriter.Write("{0} {1}", DateTime.Now.ToLongTimeString(),
                           DateTime.Now.ToLongDateString());
                        streamWriter.WriteLine("  :{0}", logMessage);
                        streamWriter.WriteLine("-------------------------------");
                        streamWriter.Close();
                    }
                }
            }
        }


        #endregion ILog    
    }

}
