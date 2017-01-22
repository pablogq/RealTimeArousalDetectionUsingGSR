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

using SignalDevice.Properties;
using System;
using System.Configuration;
using System.IO.Ports;

namespace Assets.Rage.GSRAsset.SignalDevice
{
    public class GSRHRDevice : ISignalDeviceController
    {
        // The main control for communicating through the RS-232 port
        //private SerialPort comport = SignalDeviceSerialPort.Instance;
        private Settings settings = Settings.Default;
        //private SignalDeviceUtils signalDeviceUtils;
        private Configuration appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private int SampleRate = 0;
        private HMDevice.HMDevice device =  new HMDevice.HMDevice();

        public GSRHRDevice()
        {
            //super();
        }

        public void GetSignalData(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void OpenPort()
        {
            device.OpenPort();
        }

        public void SelectCOMPort(string portName)
        {
            if (!device.IsPortOpen())
            {
                //comport.PortName = portName;
                device.SelectCOMPort(portName);
            }
        }

        public void SetSignalSettings()
        {
            ConfigurationManager.RefreshSection("appSettings");

            if (Convert.ToInt32(ConfigurationManager.AppSettings["Samplerate"]) < 1 && device.IsPortOpen())
            {
                //default samplerate(speed/per second)
                device.SetSignalSamplerate(20);
            }
            else
            {
                int sampleRateSetting = Int16.Parse(ConfigurationManager.AppSettings["Samplerate"]);
                device.SetSignalSamplerate(sampleRateSetting);
                SampleRate = Convert.ToInt32(sampleRateSetting);
            }
        }

        public int SetSignalSamplerate(string samplerate)
        {
            device.SetSignalSamplerate(Int16.Parse(samplerate));
            return -1;
        }

        public void SetSignalSamplerate()
        {
            int sampleRateSettings = Int16.Parse(ConfigurationManager.AppSettings["Samplerate"]);
            ConfigurationManager.RefreshSection("appSettings");
            SampleRate = sampleRateSettings;
            device.SetSignalSamplerate(sampleRateSettings);
        }

        public int GetSignalSampleRate()
        {
            if(SampleRate > 0)
            {
                if (!(device.GetSignalSampleRate() > 0 && SampleRate == device.GetSignalSampleRate()))
                {
                    device.SetSignalSamplerate(SampleRate);
                }

                return SampleRate;
            }
            else
            {
                ConfigurationManager.RefreshSection("appSettings");
                int sampleRateSetting = Convert.ToInt32(ConfigurationManager.AppSettings["Samplerate"]);
                SampleRate = sampleRateSetting;
                //if (!(device.GetSignalSampleRate() > 0))device.SetSignalSamplerate(sampleRateSetting);
                return sampleRateSetting;
            }
        }

        public int GetSignalSampleRateByConfig()
        {
            ConfigurationManager.RefreshSection("appSettings");            
            return Convert.ToInt32(ConfigurationManager.AppSettings["Samplerate"]);
        }

        public int StartSignalsRecord()
        {
            device.StartSignalsTransfer();
            return 0;
        }

        public int StopSignalsRecord()
        {
            if(!"TestWithoutDevice".Equals(ConfigurationManager.AppSettings.Get("ApplicationMode"))) device.StopSignalsTransfer();
            return 0;
        }
    }
}
