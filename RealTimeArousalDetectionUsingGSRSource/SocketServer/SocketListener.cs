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

using Assets.Rage.GSRAsset.Utils;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net.NetworkInformation;
using AssetPackage;
using AssetManagerPackage;
using System.Globalization;

namespace Assets.Rage.GSRAsset.SocketServer
{
    public class SocketListener
    {
        TcpListener server = null;
        private RealTimeArousalDetectionAssetSettings settings;
        private ILog logger;
        private bool isTcpListenerActive = false;
        private TcpClient client;

        public void StartListening()
        {
            settings = RealTimeArousalDetectionAssetSettings.Instance;
            logger = (ILog)AssetManager.Instance.Bridge;
            try
            {
                // Set the TcpListener
                Int32 port = settings.SocketPort;
                IPAddress localAddr = IPAddress.Parse(settings.SocketIPAddress);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();
                isTcpListenerActive = true;

                GSRSignalProcessor gsrHandler = new GSRSignalProcessor();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true && isTcpListenerActive)
                {
                    // You could also user server.AcceptSocket() here.
                    client = server.AcceptTcpClient();

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        StringBuilder response = new StringBuilder();
                        response.Append("Received at ");
                        response.Append(DateTime.Now.ToString());
                        response.Append("\r\n");
                        response.Append(data);

                        //Start Of Calibration Period
                        if (data.Equals("SOCP"))
                        {
                            response.Append("\r\n");
                            string result = gsrHandler.StartOfCalibrationPeriod();
                            response.Append(result);
                        }

                        //End Of Calibration Period
                        if (data.Equals("EOCP"))
                        {
                            response.Append("\r\n");
                            string result = gsrHandler.EndOfCalibrationPeriod();
                            response.Append(result);
                        }

                        if (data.Equals("GET_EDA"))
                        {
                            response.Append("\r\n");
                            string result = gsrHandler.GetJSONArousalStatistics(gsrHandler.GetArousalStatistics());
                            response.Append(result);
                        }

                        //End Of Measurement
                        if (data.Equals("EOM"))
                        {
                            response.Append("\r\n");
                            string result = gsrHandler.EndOfMeasurement();
                            response.Append(result);
                        }

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(response.ToString());

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                isTcpListenerActive = false;
                logger.Log(Severity.Error, "SocketException: " + e);
            }
            finally
            {
                // Stop listening for new clients.
                isTcpListenerActive = false;
                if(client != null) client.Close();
                server.Stop();
            }
        }

        public void CloseSocket()
        {
            try
            {
                if(client != null) client.Close();
                server.Stop();
                isTcpListenerActive = false;
            }
            catch(Exception e)
            {
                isTcpListenerActive = false;
                logger.Log(Severity.Error, e.ToString());
            }
        }

        public void Start()
        {
            Thread t = new Thread(StartListening);
            t.Start();
        }

        public bool IsSocketConnected()
        {
            bool inUse = false;
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == settings.SocketPort)
                {
                    inUse = true;
                    break;
                }
            }

            return isTcpListenerActive || inUse;
        }
    }
}
