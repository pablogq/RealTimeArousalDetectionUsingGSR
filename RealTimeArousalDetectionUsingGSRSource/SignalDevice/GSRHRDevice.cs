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

using HMDevice;
using Assets.Rage.RealTimeArousalDetectionUsingGSRAsset.Utils;

namespace Assets.Rage.RealTimeArousalDetectionUsingGSRAsset.SignalDevice
{
    public class GSRHRDevice : ISignalDeviceController
    {
        private int SampleRate = 0;
        private HMDevice.HMDevice device;
        private static GSRHRDevice instance;
        private RealTimeArousalDetectionAssetSettings settings;

        private GSRHRDevice()
        {
            device = new HMDevice.HMDevice();
            settings = RealTimeArousalDetectionAssetSettings.Instance;
        }

        public static GSRHRDevice Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GSRHRDevice();
                }
                return instance;
            }
        }

        public void OpenPort()
        {
            device.OpenPort();
        }

        public void OpenPort(int sampleRate)
        {
            device.OpenPort(sampleRate);
        }

        public void SelectCOMPort(string portName)
        {
            if (!device.IsPortOpen())
            {
                device.SelectCOMPort(portName);
            }
        }

        public void SetSignalSettings()
        {
            if (settings.Samplerate < 1 && device.IsPortOpen())
            {
                //default samplerate(speed/per second)
                device.SetSignalSamplerate(100);
            }
            else
            {
                int sampleRateSetting = settings.Samplerate;
                device.SetSignalSamplerate(sampleRateSetting);
                SampleRate = sampleRateSetting;
            }
        }

        public int SetSignalSamplerate(int samplerate)
        {
            device.SetSignalSamplerate(samplerate);
            return -1;
        }

        public void SetSignalSamplerate()
        {
            int sampleRateSettings = settings.Samplerate;
            device.SetSignalSamplerate(sampleRateSettings);
        }

        public int GetSignalSampleRate()
        {
            if (SampleRate > 0)
            {
                if (!(device.GetSignalSampleRate() > 0 && SampleRate == device.GetSignalSampleRate()))
                {
                    device.SetSignalSamplerate(SampleRate);
                }

                return SampleRate;
            }
            else
            {
                int sampleRateSetting = settings.Samplerate;
                return sampleRateSetting;
            }
        }

        public int GetSignalSampleRateByConfig()
        {
            return settings.Samplerate;
        }

        public int StartSignalsRecord()
        {
            device.StartSignalsTransfer();
            return 0;
        }

        public int StopSignalsRecord()
        {
            if (!"TestWithoutDevice".Equals(settings.ApplicationMode))
            {
                device.StopSignalsTransfer();
            }

            return 0;
        }
    }
}
