

namespace AssetPackage
{
    using AssetManagerPackage;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;

    /// <summary>
    /// The logger class
    /// </summary>
    public class BaseLog : ILog, IAsset
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseLog()
        {
            this.Id = AssetManager.Instance.registerAssetInstance(this, this.Class);
        }

        public BaseLog(IBridge bridge) : this()
        {
            this.Bridge = bridge;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the bridge.
        /// </summary>
        ///
        /// <value>
        /// The bridge.
        /// </value>
        public IBridge Bridge
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the class.
        /// </summary>
        ///
        /// <value>
        /// The class.
        /// </value>
        public String Class
        {
            get
            {
                return this.GetType().Name;
            }
        }

        /// <summary>
        /// Gets the dependencies.
        /// </summary>
        ///
        /// <value>
        /// The dependencies.
        /// </value>
        public Dictionary<String, String> Dependencies
        {
            get
            {
                Dictionary<String, String> result = new Dictionary<String, String>();

                foreach (Depends dep in VersionInfo.Dependencies)
                {
                    String minv = dep.minVersion != null ? dep.minVersion : "0.0";
                    String maxv = dep.maxVersion != null ? dep.maxVersion : "*";

                    result.Add(dep.name, String.Format("{0}-{1}", minv, maxv));
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object has settings.
        /// </summary>
        ///
        /// <value>
        /// true if this object has settings, false if not.
        /// </value>
        public Boolean hasSettings
        {
            get
            {
                return Settings != null;
            }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        ///
        /// <value>
        /// The identifier.
        /// </value>
        public String Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the maturity.
        /// </summary>
        ///
        /// <value>
        /// The maturity.
        /// </value>
        public String Maturity
        {
            get
            {
                return VersionInfo.Maturity;
            }
        }

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        ///
        /// <value>
        /// The settings.
        /// </value>
        public virtual ISettings Settings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        ///
        /// <value>
        /// The version.
        /// </value>
        public String Version
        {
            get
            {
                return String.Format("{0}.{1}.{2}.{3}",
                        VersionInfo.Major,
                        VersionInfo.Minor,
                        VersionInfo.Build,
                        VersionInfo.Revision == 0 ? "" : VersionInfo.Revision.ToString()
                    ).TrimEnd('.');
            }
        }

        /// <summary>
        /// Gets information describing the version.
        /// </summary>
        ///
        /// <value>
        /// Information describing the version.
        /// </value>
        public RageVersionInfo VersionInfo
        {
            get;
            private set;
        }
        #endregion Properties

        /// <summary>
        /// Executes the log operation.
        /// </summary>
        ///
        /// <param name="msg"> The message. </param>
        public void Log(string msg)
        {
            String currentDate = DateTime.Now.ToString("yyyyMMdd");
            String logFileName = ConfigurationManager.AppSettings.Get("LogFile").Replace(".txt", currentDate + ".txt");
            using (StreamWriter w = File.AppendText(logFileName))
            {
                w.Write("\r\nLog Entry : ");
                w.Write("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                w.WriteLine("  :{0}", msg);
                w.WriteLine("-------------------------------");
            }
        }
   }
}
