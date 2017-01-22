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

using AssetPackage;
using Assets.Rage.GSRAsset.SignalDevice;
using Assets.Rage.GSRAsset.SignalProcessor;
using System;

namespace Assets.Rage.GSRAsset.Integrator
{
    public class ArousalDetectionGSRDeviceIntegrator : BaseAsset, IArousalDetection, ISignalDeviceController
    {
        GSRHRDevice gsrDevice;
        GSRSignalProcessor gsrProcessor;

        public ArousalDetectionGSRDeviceIntegrator() : base()
        {
            gsrDevice = new GSRHRDevice();
            gsrProcessor = new GSRSignalProcessor();
        }

        public GSRHRDevice GetGSRDevice()
        {
            return gsrDevice;
        }

        public GSRSignalProcessor GetGSRSignalProcessor()
        {
            return gsrProcessor;
        }

        public double GetGSRFeature(string featureName)
        {
            if ("SCRArousalArea".Equals(featureName))
            {
                return gsrProcessor.GetArousalStatistics().SCRArousalArea;
            }

            if ("SCRAchievedArousalLevel".Equals(featureName))
            {
                return gsrProcessor.GetArousalStatistics().SCRAchievedArousalLevel;
            }

            if ("SCLAchievedArousalLevel".Equals(featureName))
            {
                return gsrProcessor.GetArousalStatistics().SCLAchievedArousalLevel;
            }

            if ("MovingAverage".Equals(featureName))
            {
                return gsrProcessor.GetArousalStatistics().MovingAverage;
            }

            return -1;
        }

        public void GetSignalData(byte[] data)
        {
            gsrDevice.GetSignalData(data);
        }

        public int GetSignalSampleRate()
        {
            return gsrDevice.GetSignalSampleRate();
        }

        public int GetSignalSampleRateByConfig()
        {
            return gsrDevice.GetSignalSampleRate();
        }

        public void OpenPort()
        {
            gsrDevice.OpenPort();
        }

        public void SelectCOMPort(string portName)
        {
            gsrDevice.SelectCOMPort(portName);
        }

        public int SetMaxArousalLevel(int numberOfLevels)
        {
            if (numberOfLevels < 100 && numberOfLevels > 0)
            {
                gsrProcessor.ArousalLevel = numberOfLevels;
                return 0;
            }

            return -1;
        }

        public void SetSignalSamplerate()
        {
            gsrDevice.SetSignalSamplerate();
        }

        public int SetSignalSamplerate(string speed)
        {
            return gsrDevice.SetSignalSamplerate(speed);
        }

        public void SetSignalSettings()
        {
            gsrDevice.SetSignalSettings();
        }

        public int SetTimeWindow(double timeWindow)
        {
            if (timeWindow > 0)
            {
                gsrProcessor.DefaultTimeWindow = Convert.ToDouble(timeWindow);
                return 0;
            }

            return -1;
        }

        public int StartSignalsRecord()
        {
            return gsrDevice.StartSignalsRecord();
        }

        public int StopSignalsRecord()
        {
            return gsrDevice.StartSignalsRecord();
        }
    }
}
