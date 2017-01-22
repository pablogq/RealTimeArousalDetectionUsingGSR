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
using System.Collections.Generic;
using System.IO.Ports;
using SignalDevice.Properties;
using System.Text;
using System.Linq;

namespace Assets.Rage.GSRAsset.SignalDevice
{
    public class SignalDeviceUtils
    {
        private SerialPort comport = SignalDeviceSerialPort.Instance;
        private Settings settings = Settings.Default;

        int value00 = 0, value01 = 0;
        int counter00 = 0, counter01 = 0;
        double HRvalue = 0;

        public SignalDeviceUtils()
        {
            //super();
        }

        public void PortDataReceiver(object sender, SerialDataReceivedEventArgs e)
        {
            // If the com port has been closed, do nothing
            if (!comport.IsOpen) return;

            if (settings.DataMode == DataMode.Text)
            {
                // This method will be called when there is data waiting in the port's buffer
                // Read all the data waiting in the buffer
                int data;
                // Display the text to the user in the terminal
                if (Int32.TryParse((comport.ReadExisting()).TrimStart('0'), out data))
                {
                    Cache.AddChannelCacheValue(0, data);
                }

                //Log(LogMsgType.Incoming, data);
            }
            else
            {
                // Obtain the number of bytes waiting in the port's buffer
                int bytes = comport.BytesToRead;

                // Create a byte array buffer to hold the incoming data
                byte[] buffer = new byte[bytes];

                // Read the data from the port and store it in our buffer
                comport.Read(buffer, 0, bytes);

                // Show the user the incoming data in hex format
                //Log(LogMsgType.Incoming, ByteArrayToHexString(buffer));
                LogGSRData(buffer);
                LogHRData(buffer);
            }


        }

        private void LogHRData(byte[] data)
        {
            byte HiBit = 0xF0;
            byte LoBit = 0x0F;
            byte cod;
            StringBuilder sbHR = new StringBuilder(data.Length * 32);
            foreach (byte b in data)
            {
                cod = (byte)(b & HiBit);
                switch (cod)
                {
                    case 0x40:
                        value01 = value01 + (b & LoBit);
                        counter01 = counter01 + 1;
                        if (counter01 == 3)
                        {
                            HRvalue = (432000.0 / value01);
                            sbHR.Append(" ");
                            sbHR.Append((HRvalue).ToString("000.00"));
                            sbHR.Append(":");
                            sbHR.Append((value01 * 1.0).ToString("00000.000"));
                        }
                        break;
                    case 0x50:
                        value01 = value01 + 16 * (b & LoBit);
                        counter01 = counter01 + 1;
                        break;
                    case 0x60:
                        counter01 = counter01 + 1;
                        value01 = value01 + 256 * (b & LoBit);
                        break;
                    case 0x70:
                        counter01 = 0;
                        value01 = 256 * 16 * (b & LoBit);
                        break;

                    default:

                        break;
                }
            }

            AddDataToCache(1, sbHR.ToString().ToUpper());
        }

        private void LogGSRData(byte[] data)
        {
            byte HiBits = 0xF0;
            byte LoBits = 0x0F;
            byte coding;
            StringBuilder sb = new StringBuilder(data.Length * 24);
            foreach (byte b in data)
            {
                coding = (byte)(b & HiBits);
                switch (coding)
                {
                    case 0x00:
                        value00 = value00 + (b & LoBits);
                        counter00 = counter00 + 1;
                        //sb.Append(Convert.ToString(counter00, 10).PadLeft(2, '.').PadRight(3, ' '));
                        //sb.Append(Convert.ToString(value00, 10).PadLeft(4, ' ').PadRight(5, ' '));
                        if (counter00 == 2)
                        {
                            sb.Append(" ");
                            sb.Append(Convert.ToString(value00, 10).PadLeft(4, ' ').PadRight(5, ' '));
                        }
                        break;
                    case 0x10:
                        value00 = value00 + 16 * (b & LoBits);
                        counter00 = counter00 + 1;
                        //sb.Append(Convert.ToString(counter00, 10).PadLeft(2, '.').PadRight(3, ' '));
                        //sb.Append(Convert.ToString(value00, 10).PadLeft(4, ' ').PadRight(5, ' '));
                        break;
                    case 0x20:
                        counter00 = 0;
                        value00 = 256 * (b & LoBits);
                        //sb.Append(Convert.ToString(counter00, 10).PadLeft(2, '.').PadRight(3, ' '));
                        //sb.Append(Convert.ToString(value00, 10).PadLeft(4, ' ').PadRight(5, ' '));
                        break;

                    default:

                        break;
                }
            }

            AddDataToCache(0, sb.ToString().ToUpper());
        }

        private void AddDataToCache(int channel, string data)
        {
            double mos;
            List<double> dataList = data.Split(' ')
                    .Select(m => { double.TryParse(m, out mos); return mos; })
                    .Where(m => m != 0)
                    .ToList();
            //(data != null && data.Length > 0) ? data.Split(' ').Select(int.Parse).ToList() : null;
            if (dataList != null && dataList.Count > 0)
            {
                Cache.AddChannelCacheValue(channel, dataList);
                //Logger.Log((DateTime.Now - DateTime.MinValue).TotalMilliseconds + ", data: " + data);
            }
        }

        /// <summary> Convert a string of hex digits (ex: E4 CA B2) to a byte array. </summary>
        /// <param name="s"> The string containing the hex digits (with or without spaces). </param>
        /// <returns> Returns an array of bytes. </returns>
        private byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        /// <summary> Converts an array of bytes into a formatted string of hex digits (ex: E4 CA B2)</summary>
        /// <param name="data"> The array of bytes to be translated into a string of hex digits. </param>
        /// <returns> Returns a well formatted string of hex digits with spacing. </returns>
        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            return sb.ToString().ToUpper();
        }

        public string[] OrderedPortNames()
        {
            // Just a placeholder for a successful parsing of a string to an integer
            int num;

            // Order the serial port names in numberic order (if possible)
            return SerialPort.GetPortNames().OrderBy(a => a.Length > 3 && int.TryParse(a.Substring(3), out num) ? num : 0).ToArray();
        }

        public string RefreshComPortList(IEnumerable<string> PreviousPortNames, string CurrentSelection, bool PortOpen)
        {
            // Create a new return report to populate
            string selected = null;

            // Retrieve the list of ports currently mounted by the operating system (sorted by name)
            string[] ports = SerialPort.GetPortNames();

            // First determain if there was a change (any additions or removals)
            bool updated = PreviousPortNames.Except(ports).Count() > 0 || ports.Except(PreviousPortNames).Count() > 0;

            // If there was a change, then select an appropriate default port
            if (updated)
            {
                // Use the correctly ordered set of port names
                ports = OrderedPortNames();

                // Find newest port if one or more were added
                string newest = SerialPort.GetPortNames().Except(PreviousPortNames).OrderBy(a => a).LastOrDefault();

                // If the port was already open... (see logic notes and reasoning in Notes.txt)
                if (PortOpen)
                {
                    if (ports.Contains(CurrentSelection)) selected = CurrentSelection;
                    else if (!String.IsNullOrEmpty(newest)) selected = newest;
                    else selected = ports.LastOrDefault();
                }
                else
                {
                    if (!String.IsNullOrEmpty(newest)) selected = newest;
                    else if (ports.Contains(CurrentSelection)) selected = CurrentSelection;
                    else selected = ports.LastOrDefault();
                }
            }

            // If there was a change to the port list, return the recommended default selection
            return selected;
        }

        public void SendData()
        {
            /*
            if (CurrentDataMode == DataMode.Text)
            {
                // Send the user's text straight out the port
                comport.Write(txtSendData.Text);

                // Show in the terminal window the user's text
                Log(LogMsgType.Outgoing, txtSendData.Text + "\n");
            }
            else
            {
                try
                {
                    // Convert the user's string of hex digits (ex: B4 CA E2) to a byte array
                    byte[] data = HexStringToByteArray(txtSendData.Text);

                    // Send the binary data out the port
                    comport.Write(data, 0, data.Length);

                    // Show the hex digits on in the terminal window
                    Log(LogMsgType.Outgoing, ByteArrayToHexString(data) + "\n");
                }
                catch (FormatException)
                {
                    // Inform the user if the hex string was not properly formatted
                    Log(LogMsgType.Error, "Not properly formatted hex string: " + txtSendData.Text + "\n");
                }
            }
            txtSendData.SelectAll();
            */
        }
    }
}
