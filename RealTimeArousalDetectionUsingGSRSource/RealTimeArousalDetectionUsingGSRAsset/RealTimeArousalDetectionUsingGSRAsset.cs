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

using AssetManagerPackage;
using AssetPackage;
using Assets.Rage.GSRAsset.SignalDevice;
using Assets.Rage.GSRAsset.SocketServer;
using Assets.Rage.GSRAsset.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Assets.Rage.GSRAsset.Integrator
{
    public class RealTimeArousalDetectionUsingGSRAsset : BaseAsset, ISignalDeviceController
    {
        #region Fields
        private static RealTimeArousalDetectionUsingGSRAsset instance = null;
        private SocketListener socketListener;
        private RealTimeArousalDetectionAssetSettings settings;
        private GSRHRDevice gsrDevice;
        private GSRSignalProcessor gsrProcessor;
        private ILog logger;
        #endregion Fields

        #region Constructors
        public static RealTimeArousalDetectionUsingGSRAsset Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RealTimeArousalDetectionUsingGSRAsset();
                }
                return instance;
            }
        }

        public RealTimeArousalDetectionUsingGSRAsset() : base()
        {
            socketListener = new SocketListener();
            gsrDevice = GSRHRDevice.Instance;
            gsrProcessor = new GSRSignalProcessor();
            settings = RealTimeArousalDetectionAssetSettings.Instance;
            logger = (ILog)AssetManager.Instance.Bridge;

            //preventing multiple asset creation
            if (AssetManager.Instance.findAssetsByClass(this.Class).Count > 1)
            {
                logger.Log(Severity.Error, "Attempt for more than one instance of the class RealTimeArousalDetectionUsingGSRAsset (it is not allowed).");
            }
        }
        #endregion Constructors

        #region Properties
        public override ISettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                settings = (value as RealTimeArousalDetectionAssetSettings);
            }
        }

        #endregion Properties

        #region PublicMethods
        public GSRHRDevice GetGSRDevice()
        {
            return (GSRHRDevice)gsrDevice;
        }

        public int StopSignalsRecord()
        {
            return gsrDevice.StopSignalsRecord();
        }

        public ErrorStartSignalDevice StartGSRDevice(String lblError)
        {
            ErrorStartSignalDevice result = new ErrorStartSignalDevice();
            if (!"TestWithoutDevice".Equals(settings.ApplicationMode))
            {
                String errorComPort = "The COM Port " + settings.COMPort + " is not available;";
                String errorOpenPort = "The port can not be open;";
                String errorStartSignalDevice = "The GSR device can not be started. Please check log file;";

                try
                {
                    SelectCOMPort(settings.COMPort);
                    lblError = lblError.ToString().Replace(errorComPort, "");
                }
                catch (Exception e)
                {
                    if (!"BackgroundMode".Equals(settings.FormMode))
                    {
                        lblError = lblError + errorComPort;
                        return new ErrorStartSignalDevice(lblError, ErrorStartSignalDevice.ErrorType.ErrorComPort, e);
                    }
                }

                try
                {
                    OpenPort(settings.Samplerate);
                    lblError = lblError.ToString().Replace(errorOpenPort, "");
                }
                catch (Exception e)
                {
                    if (!"BackgroundMode".Equals(settings.FormMode))
                    {
                        lblError = lblError + errorOpenPort;
                        return new ErrorStartSignalDevice(lblError, ErrorStartSignalDevice.ErrorType.ErrorOpenPort, e);
                    }
                }

                try
                {
                    StartSignalsRecord();
                    lblError = lblError.ToString().Replace(errorStartSignalDevice, "");
                }
                catch (Exception e)
                {
                    if (!"BackgroundMode".Equals(settings.FormMode))
                    {
                        lblError = lblError + errorStartSignalDevice;
                        return new ErrorStartSignalDevice(lblError, ErrorStartSignalDevice.ErrorType.ErrorStartSignalDevice, e); 
                    }
                }
            }

            return new ErrorStartSignalDevice(lblError, ErrorStartSignalDevice.ErrorType.None, null);
        }

        public void StartSocket()
        {
            if (socketListener != null)
            {
                socketListener.Start();
            }
        }

        public void CloseSocket()
        {
            if (socketListener != null)
            {
                socketListener.CloseSocket();
            }
        }

        public bool IsSocketConnected()
        {
            return socketListener.IsSocketConnected();
        }

        public GSRSignalProcessor GetGSRSignalProcessor()
        {
            return gsrProcessor;
        }

        public double GetGSRFeature(string featureName)
        {
            ArousalStatistics arousalStat = gsrProcessor.GetArousalStatistics();
            if ("SCRArousalArea".Equals(featureName))
            {
                return arousalStat.SCRArousalArea;
            }

            if ("SCRAchievedArousalLevel".Equals(featureName))
            {
                return arousalStat.SCRAchievedArousalLevel;
            }

            if ("SCLAchievedArousalLevel".Equals(featureName))
            {
                return arousalStat.SCLAchievedArousalLevel;
            }

            if ("MovingAverage".Equals(featureName))
            {
                return arousalStat.MovingAverage;
            }

            if ("GeneralArousalLevel".Equals(featureName))
            {
                return arousalStat.GeneralArousalLevel;
            }

            return -1;
        }

        public List<SignalDataByTime> GetSignalData()
        {
            return gsrProcessor.GetSignalValues().ToList();
        }

        public SignalDataByTime[] GetMedianFilterValues(SignalDataByTime[] sourceList)
        {
            return gsrProcessor.GetMedianFilterPoints(sourceList);
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

        public void OpenPort(int sampleRate)
        {
            gsrDevice.OpenPort(sampleRate);
        }

        public void SelectCOMPort(string portName)
        {
            gsrDevice.SelectCOMPort(portName);
        }

        public int SetSignalSamplerate(int speed)
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
                gsrProcessor.DefaultTimeWindow = Convert.ToDouble(timeWindow, CultureInfo.InvariantCulture);
                return 0;
            }

            return -1;
        }

        public int StartSignalsRecord()
        {
            return gsrDevice.StartSignalsRecord();
        }

        public void SetSignalSamplerate()
        {
            gsrDevice.SetSignalSamplerate();
        }

        //Start Of Calibration Period
        public string StartOfCalibrationPeriod()
        {
            return gsrProcessor.StartOfCalibrationPeriod();
        }

        //End Of Calibration Period
        public string EndOfCalibrationPeriod()
        {
            return gsrProcessor.EndOfCalibrationPeriod();
        }

        //Get arousal statistics for the last time-window interval
        public string GetEDA()
        {
            return gsrProcessor.GetJSONArousalStatistics(gsrProcessor.GetArousalStatistics());
        }

        //End Of Measurement
        public string EndOfMeasurement()
        {
            return gsrProcessor.EndOfMeasurement();
        }

        #endregion PublicMethods
    }
}
