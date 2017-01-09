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
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Assets.Rage.GSRAsset.Integrator
{
    class RealTimeArousalDetectionAssetSettings : BaseSettings
    {
        #region Fields
        String source = "gsrSettings.xml";
        String serverSource = "";
        #endregion Fields

        /// <summary>
        /// Initializes a new instance of the Assets.Rage.GSRAsset.Integrator.RealTimeArousalDetectionAssetSettings
        /// class.
        /// </summary>
        public RealTimeArousalDetectionAssetSettings() : base()
        {
            //
        }

        /// <summary>
        /// Gets or sets the timewindow.
        /// </summary>
        ///
        /// <value>
        /// The timewindow.
        /// </value>
        public String DefaultTimeWindow
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
        public String Samplerate
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
        public String ArousalLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of the GSR device COM port.
        /// </summary>
        ///
        /// <value>
        /// The the number of the GSR device COM port.
        /// </value>
        public String COMPort
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of the socket port.
        /// </summary>
        ///
        /// <value>
        /// The the number of the socket port.
        /// </value>
        public String SocketPort
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the number of the socket IP address.
        /// </summary>
        ///
        /// <value>
        /// The the number of the socket IP address.
        /// </value>
        public String SocketIPAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Defines to load the initial settings from the web service.
        /// </summary>
        [XmlElement()]
        public String ServerSource
        {
            get { return (serverSource); }
            set { serverSource = value;}
        }

        /// <summary>
        /// Defines to load the initial settings from the local xml file.
        /// </summary>
        [XmlElement()]
        public String LocalSource
        {
            get { return source; }
            set { source = value; }
        }

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
    }
}
