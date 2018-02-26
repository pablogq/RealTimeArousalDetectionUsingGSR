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
using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Assets.Rage.RealTimeArousalDetectionUsingGSRAsset.Utils
{
    public class RealTimeArousalDetectionAssetSettings : BaseSettings
    {
        #region Fields
        String source = "RealTimeArousalDetectionAssetSettings.xml";
        private static RealTimeArousalDetectionAssetSettings instance;
        #endregion Fields

        #region Constructors
        public static RealTimeArousalDetectionAssetSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RealTimeArousalDetectionAssetSettings();
                }
                return instance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Assets.Rage.GSRAsset.Integrator.RealTimeArousalDetectionAssetSettings
        /// class.
        /// </summary>
        private RealTimeArousalDetectionAssetSettings() : base()
        {
            LoadDefaultRealTimeArousalDetectionAssetSettings();
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the name of the log file.
        /// </summary>
        ///
        /// <value>
        /// The log file.
        /// </value>
        public String LogFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the test file.
        /// </summary>
        ///
        /// <value>
        /// The test file.
        /// </value>
        public String TestData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the minimum GSR Device signal value.
        /// </summary>
        ///
        /// <value>
        /// The minimum GSR Device signal value
        /// </value>
        public double MinGSRDeviceSignalValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum GSR Device signal value.
        /// </summary>
        ///
        /// <value>
        /// The maximum GSR Device signal value.
        /// </value>
        public double MaxGSRDeviceSignalValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the calibration timer interval.
        /// </summary>
        ///
        /// <value>
        /// The calibration timer interval.
        /// </value>
        public int CalibrationTimerInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the timewindow.
        /// </summary>
        ///
        /// <value>
        /// The timewindow.
        /// </value>
        public double DefaultTimeWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the samplerate.
        /// </summary>
        ///
        /// <value>
        /// The samplerate.
        /// </value>
        public int Samplerate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum arousal level.
        /// </summary>
        ///
        /// <value>
        /// The maximum arousal level.
        /// </value>
        public int ArousalLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of the socket port.
        /// </summary>
        ///
        /// <value>
        /// The number of the socket port.
        /// </value>
        public Int32 SocketPort
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the number of the socket IP address.
        /// </summary>
        ///
        /// <value>
        /// The number of the socket IP address.
        /// </value>
        public String SocketIPAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of the GSR device COM port.
        /// </summary>
        ///
        /// <value>
        /// The number of the GSR device COM port.
        /// </value>
        public String COMPort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the phasic frequency for the butterworth filter.
        /// </summary>
        ///
        /// <value>
        /// The phasic frequency for the butterworth filter.
        /// </value>
        public double ButterworthPhasicFrequency
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tonic frequency for the butterworth filter.
        /// </summary>
        ///
        /// <value>
        /// The tonic frequency for the butterworth filter.
        /// </value>
        public double ButterworthTonicFrequency
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the application mode.
        /// </summary>
        ///
        /// <value>
        /// The application mode.
        /// </value>
        public String ApplicationMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the form mode.
        /// </summary>
        ///
        /// <value>
        /// The form mode.
        /// </value>
        public String FormMode
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods
        public String SettingsToXmlString()
        {
            String xmlString = "";
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(RealTimeArousalDetectionAssetSettings));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlSerializer.Serialize(writer, this);
                    xmlString = stringWriter.ToString();

                    return (xmlString);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in XML Serialization", ex);
            }
        }

        void LoadDefaultRealTimeArousalDetectionAssetSettings()
        {
            IDataStorage storage = (IDataStorage)AssetManager.Instance.Bridge;
                
            if (storage != null && source != null && storage.Exists(source))
            {
                XElement xml = XElement.Parse(storage.Load(source));
                this.LogFile = xml.Element("LogFile").Value;
                this.TestData = xml.Element("TestData").Value;
                this.MinGSRDeviceSignalValue = Convert.ToDouble(xml.Element("MinGSRDeviceSignalValue").Value, CultureInfo.InvariantCulture);
                this.MaxGSRDeviceSignalValue = Convert.ToDouble(xml.Element("MaxGSRDeviceSignalValue").Value, CultureInfo.InvariantCulture);

                this.CalibrationTimerInterval = Convert.ToInt16(xml.Element("CalibrationTimerInterval").Value, CultureInfo.InvariantCulture);

                this.DefaultTimeWindow = Convert.ToDouble(xml.Element("DefaultTimeWindow").Value, CultureInfo.InvariantCulture);
                this.Samplerate        = Convert.ToInt16(xml.Element("Samplerate").Value, CultureInfo.InvariantCulture);
                this.ArousalLevel      = Convert.ToInt16(xml.Element("ArousalLevel").Value, CultureInfo.InvariantCulture);
                this.COMPort           = xml.Element("COMPort").Value;
                this.SocketPort        = Convert.ToInt32(xml.Element("SocketPort").Value, CultureInfo.InvariantCulture);
                this.SocketIPAddress   = xml.Element("SocketIPAddress").Value;

                this.ButterworthPhasicFrequency = Convert.ToDouble(xml.Element("ButterworthPhasicFrequency").Value, CultureInfo.InvariantCulture);
                this.ButterworthTonicFrequency = Convert.ToDouble(xml.Element("ButterworthTonicFrequency").Value, CultureInfo.InvariantCulture);

                this.ApplicationMode = xml.Element("ApplicationMode").Value;
                this.FormMode = xml.Element("FormMode").Value;
            }
        }

        /// <summary>
        /// Method for storing the settings to a xml file.
        /// </summary>
        /// 
        /// <param name="settings"> RealTimeArousalDetectionAssetSettings value. </param>
        /// <param name="configFile"> path to the target file. </param>
        internal void writeSettingsToFile(RealTimeArousalDetectionAssetSettings settings, String configFile)
        {
            IDataStorage storage = (IDataStorage)AssetManager.Instance.Bridge;
            if (storage != null)
            {
                storage.Save(configFile, settings.SettingsToXmlString());
            }
        }

        internal RealTimeArousalDetectionAssetSettings getRealTimeArousalDetectionAssetSettingsByString(String xmlContent)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RealTimeArousalDetectionAssetSettings));
            using (TextReader reader = new StringReader(xmlContent))
            {
                RealTimeArousalDetectionAssetSettings result = (RealTimeArousalDetectionAssetSettings)serializer.Deserialize(reader);
                return (result);
            }
        }
        #endregion Methods
    }
}
