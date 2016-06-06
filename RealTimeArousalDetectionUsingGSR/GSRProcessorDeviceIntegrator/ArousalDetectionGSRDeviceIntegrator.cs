using Assets.Rage.GSRAsset.SignalDevice;
using Assets.Rage.GSRAsset.SignalProcessor;
using System;

namespace Assets.Rage.GSRAsset.Integrator
{
    public class ArousalDetectionGSRDeviceIntegrator : IArousalDetection, ISignalDeviceController
    {
        GSRHRDevice gsrDevice;
        GSRSignalProcessor gsrProcessor;

        public ArousalDetectionGSRDeviceIntegrator()
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
