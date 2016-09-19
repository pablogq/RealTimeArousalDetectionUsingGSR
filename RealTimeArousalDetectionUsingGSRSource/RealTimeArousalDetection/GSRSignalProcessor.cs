using System;
using Assets.Rage.GSRAsset.SignalDevice;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Assets.Rage.GSRAsset.SignalProcessor
{
    public class GSRSignalProcessor
    {
        public Dictionary<int, List<double>> channelsValues;
        public String pathToBinaryFile;
        public double gsrValuesReadTime;
        public Dictionary<int, Dictionary<double, double>> coordinates;
        public const int GSR_CHANNEL = 0;
        public const int HR_CHANNEL = 1;
        public const double BUTTERWORTH_TONIC_PHASIC_FREQUENCY = 0.05;
        private Configuration settings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        private int arousalLevel;

        private double defaultTimeWindow;

        public int ArousalLevel
        {
            get
            {
                return arousalLevel;
            }

            set
            {
                arousalLevel = value;
            }
        }

        public double DefaultTimeWindow
        {
            get
            {
                return defaultTimeWindow;
            }

            set
            {
                defaultTimeWindow = value;
            }
        }

        public GSRSignalProcessor()
        {
            channelsValues = new Dictionary<int, List<double>>();
            pathToBinaryFile = ConfigurationManager.AppSettings.Get("BinaryFile");
            gsrValuesReadTime = (double)(DateTime.Now - DateTime.MinValue).TotalMilliseconds;

            arousalLevel = Convert .ToInt32(settings.AppSettings.Settings["ArousalLevel"].Value);
            defaultTimeWindow = GetAppValue("DefaultTimeWindow");

            //re-initialize calibration data
            settings.AppSettings.Settings["CalibrationMinArousalArea"].Value = 
            
                settings.AppSettings.Settings["MinAverageArousalArea"].Value;
            settings.AppSettings.Settings["CalibrationMaxArousalArea"].Value =
                settings.AppSettings.Settings["MaxAverageArousalArea"].Value;
            settings.AppSettings.Settings["CalibrationMinTonicAmplitude"].Value = 
                settings.AppSettings.Settings["MinAverageTonicAmplitude"].Value;
            settings.AppSettings.Settings["CalibrationMaxTonicAmplitude"].Value =
                settings.AppSettings.Settings["MaxAverageTonicAmplitude"].Value;

            //settings.Save(ConfigurationSaveMode.Modified);
            //ConfigurationManager.RefreshSection("appSettings");

            /*
            minArousalArea = -1.0;
            maxArousalArea = -1.0;
            minTonicAmplitude = 0.0;
            maxTonicAmplitude = 0.0;
            */
        }

        public void SetPathToBinaryFile(String path)
        {
            this.pathToBinaryFile = path;
        }

        
        public Dictionary<int, List<double>> ExtractChannelsValues()
        {
            channelsValues = Cache.GetChannelsCache();
            return channelsValues;
        }

        //read binary file and collect values for each one channel
        public Dictionary<int, List<double>> ExtractChannelsValuesFromFile()
        {
            int numberOfValues = 0;
            using (BinaryReader binaryData = new BinaryReader(File.Open(pathToBinaryFile, FileMode.Open)))
            {
                // Position and length variables.
                int pos = 0;
                // Use BaseStream.
                int length = (int)binaryData.BaseStream.Length;
                while (pos < length)
                {
                    // Read integer.
                    byte[] unitValue = binaryData.ReadBytes(3);

                    TetradArray currentBCDChannelValue = new TetradArray();
                    TetradUtil tetradUtil = new TetradUtil();
                    int channelNumber = -1;

                    foreach (byte subUnit in unitValue)
                    {
                        tetradUtil.SetTetradChannelValuesByByte(subUnit);
                        string bcdNumber = tetradUtil.GetBCDNumber();
                        channelNumber = tetradUtil.GetChannelNumber();
                        int tetradNumber = tetradUtil.GetTetradNumber();
                        string tetradValue = tetradUtil.GetTetradValue();
                        currentBCDChannelValue = tetradUtil.SetBCDChannelValue(currentBCDChannelValue);
                    }

                    //add currentValue
                    int currentINTChannelValue = currentBCDChannelValue.GetTetradValue();

                    if (numberOfValues > 5000)
                    {
                        break;
                    }
                    else
                    {
                        FillChannelsValues(channelNumber, currentINTChannelValue);
                    }

                    numberOfValues++;
                    // Advance our position variable.
                    pos += 3;
                }
            }

            //gsrValuesReadTime = DateTime.Now.Millisecond;

            return channelsValues;
        }

        private void FillChannelsValues(int channelNumber, int currentINTChannelValue)
        {
            if (channelsValues.ContainsKey(channelNumber) && currentINTChannelValue > -1)
            {
                List<double> channelTetradArray = null;
                channelsValues.TryGetValue(channelNumber, out channelTetradArray);
                if (channelTetradArray != null)
                {
                    channelTetradArray.Add(currentINTChannelValue);
                    channelsValues.Remove(channelNumber);
                    channelsValues.Add(channelNumber, channelTetradArray);
                }
            }
            else if (channelNumber < 4 && channelNumber > -1 && currentINTChannelValue > -1)
            {
                channelsValues.Add(channelNumber, new List<double> { currentINTChannelValue });
            }
        }

        public Dictionary<int, Dictionary<double, double>> GetMedianFilterPoints(Dictionary<int, Dictionary<double, double>> signalCoordinates)
        {
            if (signalCoordinates == null) signalCoordinates = coordinates;
            if (signalCoordinates == null) return null;
            FilterMedian filterMedian = new FilterMedian(signalCoordinates);

            return filterMedian.GetMedianFilterPoints();
        }

        public ArousalStatistics GetArousalStatistics(Dictionary<double, double> coordinates, double timeWindow, TimeWindowMeasure timeWindowType, int sampleRate)
        {
            if (timeWindow.CompareTo(0) <= 0 || sampleRate <= 0 || coordinates.Count <= 0) return null;

            int numberOfAffectedPoints = GetNumberOfAffectedPoints(timeWindow, timeWindowType, sampleRate);

            if (coordinates.Count < numberOfAffectedPoints) return GetArousalStatistics(coordinates);
            double timeWindowInSeconds = timeWindowType.Equals(TimeWindowMeasure.Milliseconds) ? timeWindow / 1000 : timeWindow;
            return GetArousalInfoForCoordinates(coordinates, numberOfAffectedPoints, timeWindow);
        }

        public ArousalStatistics GetArousalStatistics(Dictionary<double, double> coordinates)
        {
            return GetArousalInfoForCoordinates(coordinates, coordinates.Count, defaultTimeWindow);
        }

        private ArousalStatistics GetArousalInfoForCoordinates(Dictionary<double, double> coordinates, int numberOfAffectedPoints, double timeWindow)
        {
            InflectionLine inflectionLinesHandler = new InflectionLine();
            List<InflectionPoint> inflectionPoints = inflectionLinesHandler.GetInflectionPoints(AffectedCoordinatePoints(coordinates, numberOfAffectedPoints));
            ArousalStatistics result = new ArousalStatistics();
            result = GetArousalInfoForInflectionPoints(inflectionPoints, timeWindow);
            result.SCRArousalArea = GetArousalArea(coordinates, numberOfAffectedPoints, timeWindow) ;
            result.MovingAverage = GetMovingAverage(coordinates, numberOfAffectedPoints) ;
            result.SCRAchievedArousalLevel = GetPhasicLevel(result.SCRArousalArea);
            SetMinMaxArousalArea(result.SCRArousalArea);
            
            return result;
        }

        private void SetMinMaxArousalArea(double scrArousalArea)
        {
            if (Convert.ToDouble( settings.AppSettings.Settings["MinArousalArea"].Value ).CompareTo(-1) == 0)
            {
                settings.AppSettings.Settings["MinArousalArea"].Value = scrArousalArea.ToString();
            }
            else if (scrArousalArea.CompareTo(
                     Convert.ToDouble(settings.AppSettings.Settings["MinArousalArea"].Value)) < 0)
            {
                settings.AppSettings.Settings["MinArousalArea"].Value = scrArousalArea.ToString();
            }

            if (Convert.ToDouble(settings.AppSettings.Settings["MaxArousalArea"].Value).CompareTo(-1) == 0)
            {
                settings.AppSettings.Settings["MaxArousalArea"].Value = scrArousalArea.ToString();
            }
            else if (scrArousalArea.CompareTo(Convert.ToDouble(settings.AppSettings.Settings["MaxArousalArea"].Value)) > 0)
            {
                settings.AppSettings.Settings["MaxArousalArea"].Value = scrArousalArea.ToString();
            }

            settings.Save(ConfigurationSaveMode.Minimal);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private int GetTonicLevel(double tonicAverageAmplitude)
        {
            if (tonicAverageAmplitude.CompareTo(
                GetAppValue("MinAverageTonicAmplitude")) <= 0)
            {
                settings.AppSettings.Settings["MinAverageTonicAmplitude"].Value = tonicAverageAmplitude.ToString();
                return 1;
            }

            if (tonicAverageAmplitude.CompareTo(
                GetAppValue("MaxAverageTonicAmplitude")) >= 0)
            {
                settings.AppSettings.Settings["MaxAverageTonicAmplitude"].Value = tonicAverageAmplitude.ToString();
                return arousalLevel;
            }

            settings.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            double step = (arousalLevel != 0) ? (GetAppValue("MaxAverageTonicAmplitude") - GetAppValue("MinAverageTonicAmplitude")) / arousalLevel : 0.0;
            return (step.CompareTo(0.0) != 0) ? (int)Math.Ceiling((tonicAverageAmplitude -
                GetAppValue("MinAverageTonicAmplitude")) / step) : 0;
        }

        private int GetPhasicLevel(double scrArousalArea)
        {
            if(scrArousalArea.CompareTo(Convert.ToDouble(settings.AppSettings.Settings["MinAverageArousalArea"].Value)) <= 0)
            {
                settings.AppSettings.Settings["MinAverageArousalArea"].Value = scrArousalArea.ToString();
                return 1;
            }

            if(scrArousalArea.CompareTo(Convert.ToDouble(settings.AppSettings.Settings["MaxAverageArousalArea"].Value)) >= 0)
            {
                settings.AppSettings.Settings["MaxAverageArousalArea"].Value = scrArousalArea.ToString();
                return arousalLevel;
            }

            settings.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            double step = (arousalLevel != 0) ?(GetAppValue("MaxAverageArousalArea") - GetAppValue("MinAverageArousalArea")) / arousalLevel : 0.0;
            return (step.CompareTo(0.0) != 0) ? (int)Math.Ceiling((scrArousalArea - GetAppValue("MinAverageArousalArea")) / step) : 0;
        }

        public TonicStatistics GetTonicStatistics(Dictionary<double, double> tonicCoordinates)
        {

            InflectionLine inflectionLinesHandler = new InflectionLine();
            List<InflectionPoint> inflectionPoints = inflectionLinesHandler.GetInflectionPoints(AffectedCoordinatePoints(tonicCoordinates, tonicCoordinates.Count));

            return GetTonicStatisticsForPoints(inflectionPoints);
        }

        public TonicStatistics GetTonicStatistic(Dictionary<double, double> tonicCoordinates, int numberOfAffectedPoints)
        {

            InflectionLine inflectionLinesHandler = new InflectionLine();
            List<InflectionPoint> inflectionPoints = inflectionLinesHandler.GetInflectionPoints(AffectedCoordinatePoints(tonicCoordinates, numberOfAffectedPoints));

            return GetTonicStatisticsForPoints(inflectionPoints);
        }


        public TonicStatistics GetTonicStatistic(Dictionary<double, double> coordinates, double timeWindow, TimeWindowMeasure timewindowType, int sampleRate)
        {
            if (timeWindow.CompareTo(0) <= 0 || sampleRate <= 0 || coordinates.Count <= 0) return null;
            int numberOfAffectedPoints = GetNumberOfAffectedPoints(timeWindow, timewindowType, sampleRate);

            if (coordinates.Count < numberOfAffectedPoints) return GetTonicStatistics(coordinates);

            return GetTonicStatistic(coordinates, numberOfAffectedPoints);
        }

        private int GetNumberOfAffectedPoints(double timeWindow, TimeWindowMeasure timewindowType, int sampleRate)
        {
            timeWindow = timewindowType.Equals(TimeWindowMeasure.Milliseconds) ? timeWindow : timeWindow * 1000;
            int numberOfAffectedPoints = Convert.ToInt32((sampleRate / 1000.0) * timeWindow);

            return numberOfAffectedPoints;
        }

        private TonicStatistics GetTonicStatisticsForPoints(List<InflectionPoint> inflectionPoints)
        {
            TonicStatistics result = new TonicStatistics();

            double tonicCoordinateXFirst = inflectionPoints.ElementAt(0).CoordinateX;
            double tonicCoordinateXLast = inflectionPoints.ElementAt(inflectionPoints.Count - 1).CoordinateX;
            double tonicCoordinateYFirst = inflectionPoints.ElementAt(0).CoordinateY;
            double tonicCoordinateYLast = inflectionPoints.ElementAt(inflectionPoints.Count - 1).CoordinateY;
            result.MeanAmp = (tonicCoordinateYFirst + tonicCoordinateYLast) / 2;
            result.Slope = (tonicCoordinateYLast - tonicCoordinateYFirst) / (tonicCoordinateXLast - tonicCoordinateXFirst);

            List<double> allMaximums = new List<double>();

            double minTonic = inflectionPoints.ElementAt(0).CoordinateY;
            double maxTonic = inflectionPoints.ElementAt(0).CoordinateY;
            double sumMaximums = 0;

            for (int i = 1; i < (inflectionPoints.Count - 1); i++)
            {

                double currentY = inflectionPoints.ElementAt(i).CoordinateY;
                minTonic = (minTonic > currentY) ? currentY : minTonic;
                maxTonic = (maxTonic < currentY) ? currentY : maxTonic;

                double previousY = inflectionPoints.ElementAt(i - 1).CoordinateY;
                double currentAmplitude = currentY - previousY;

                if (inflectionPoints.ElementAt(i).ExtremaType.Equals(ExtremaType.Maximum))
                {
                    allMaximums.Add(currentAmplitude);
                    sumMaximums += currentAmplitude;
                }

            }

            result.MinAmp = minTonic;
            result.MaxAmp = maxTonic;

            decimal mean = (allMaximums != null && allMaximums.Count > 0) ? Convert.ToDecimal(sumMaximums / allMaximums.Count) : 0;

            result.StdDeviation = GetStandardDeviation(allMaximums, mean);
            //result.SCLAchievedArousalLevel = GetTonicLevel(result.MeanAmp);

            SetMinMaxTonicAmplitude(result.MinAmp, result.MaxAmp);

            return result;
        }

        private void SetMinMaxTonicAmplitude(double minAmp, double maxAmp)
        {
            if (Convert.ToDouble(GetAppValue("MinTonicAmplitude")).CompareTo(100) == 0)
            {
                settings.AppSettings.Settings["MinTonicAmplitude"].Value = minAmp.ToString();
            }
            else if (minAmp.CompareTo(Convert.ToDouble(GetAppValue("MinTonicAmplitude"))) < 0)
            {
                settings.AppSettings.Settings["MinTonicAmplitude"].Value = minAmp.ToString();
            }

            if (Convert.ToDouble(GetAppValue("MaxTonicAmplitude")).CompareTo(-100) == 0)
            {
                settings.AppSettings.Settings["MaxTonicAmplitude"].Value = maxAmp.ToString();
            }
            else if (maxAmp.CompareTo(Convert.ToDouble(GetAppValue("MaxTonicAmplitude"))) > 0)
            {
                settings.AppSettings.Settings["MaxTonicAmplitude"].Value = maxAmp.ToString();
            }

            settings.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings"); ;
        }

        private double GetMovingAverage(Dictionary<double, double> coordinates, int numberOfAffectedPoints)
        {
            double movingAverage = 0;
            for (int i = (coordinates.Count - numberOfAffectedPoints); i < (coordinates.Count - 1); i++)
            {
                movingAverage += Math.Abs(coordinates.ElementAt(i).Value);
            }

            return (movingAverage / numberOfAffectedPoints);
        }

        private double GetArousalArea(Dictionary<double, double> coordinates, int numberOfAffectedPoints, double timeWindow)
        {
            /*
            * For each one point P1 we take the next one P2 and
            * calculate the area of trapezoid or rectangular or two triangulars.
            * if P1.Y and P2.Y are in the same quadrant: S = (|P1.Y| + |P2.Y|)*(P2.X - P1.X)/2
            * else S = S1 + S2, where
            * S1 is the area of the triangular with peaks P1.Y, P1.X, Pm.X
            * S2 is the area of the triangular with peaks P2.Y, P2.X, Pm.X
            * Pm is the intercept point between the line (P1, P2) and y=0
            */
            double area = 0;
            for(int i = (coordinates.Count - numberOfAffectedPoints); i < (coordinates.Count - 1); i++)
            {
                
                if (i < (coordinates.Count - 2))
                {
                    double x1 = coordinates.ElementAt(i).Key;
                    double y1 = coordinates.ElementAt(i).Value;
                    double x2 = coordinates.ElementAt(i + 1).Key;
                    double y2 = coordinates.ElementAt(i + 1).Value;

                    if ( y1*y2 >= 0 )
                    {
                        area += (Math.Abs(y1) + Math.Abs(y2)) * (x2 - x1) / 2;
                    }
                    else if ( (x2 - x1) != 0)
                    {
                        // find x where y = 0
                        // y = a.x+b => X = -b/a
                        double a = (y2 - y1) / (x2 - x1);
                        double b = y1 - (a * x1);
                        double xIntercept = (-1)*b / a;
                        area += ((Math.Abs(y1)) * (xIntercept - x1) + (Math.Abs(y2)) * (x2 - xIntercept)) / 2;
                    }
                }
            }
            
            return (area / timeWindow);
        }

        public ArousalStatistics GetArousalInfoForInflectionPoints(List<InflectionPoint> inflectionPoints, double timeWindow)
        {
            InflectionLine inflectionLinesHandler = new InflectionLine();

            ArousalStatistics arousalStat = new ArousalStatistics();
            ArousalFeature scrAmplitude = new ArousalFeature("Amplitude");
            ArousalFeature scrRise = new ArousalFeature("Rise time");
            ArousalFeature scrRecovery = new ArousalFeature("Recovery time");
            List<double> allMaximums = new List<double>();
            List<double> allRises = new List<double>();
            List<double> allRecoveryTimes = new List<double>();
            double sumMaximums = 0;
            double sumRises = 0;
            double sumRecoveryTime = 0;

            for (int i = 0; i < (inflectionPoints.Count - 1); i++)
            {
                double x0 = (i > 0) ? inflectionPoints.ElementAt(i - 1).CoordinateX : inflectionPoints.ElementAt(0).CoordinateX;
                double y0 = (i > 0) ? inflectionPoints.ElementAt(i - 1).CoordinateY : 0;
                double x1 = inflectionPoints.ElementAt(i).CoordinateX;
                double y1 = inflectionPoints.ElementAt(i).CoordinateY;
                double x2 = inflectionPoints.ElementAt(i + 1).CoordinateX;
                double y2 = inflectionPoints.ElementAt(i + 1).CoordinateY;
                double currentAmplitude = y1 - y0;

                if (inflectionPoints.ElementAt(i).ExtremaType.Equals(ExtremaType.Maximum)) {
                    double currentRise = x1 - x0;
                    double currentRecouvery = (x1 + x2) / 2 - x1;
                    if (i == 0 || i == 1)
                    {
                        scrAmplitude.Maximum = currentAmplitude;
                        scrAmplitude.Minimum = scrAmplitude.Maximum;
                        if (i > 0)
                        {
                            scrRise.Maximum = currentRise;
                            scrRise.Minimum = currentRise;
                            allRises.Add(currentRise);

                            scrRecovery.Maximum = currentRecouvery;
                            scrRecovery.Minimum = currentRecouvery;
                            allRecoveryTimes.Add(currentRecouvery);
                        }
                    }
                    else 
                    {
                        scrAmplitude.Maximum = (scrAmplitude.Maximum.CompareTo(currentAmplitude) < 0) ? currentAmplitude : scrAmplitude.Maximum;
                        scrAmplitude.Minimum = (scrAmplitude.Minimum.CompareTo(currentAmplitude) > 0 && currentAmplitude.CompareTo(0.0) != 0) ? currentAmplitude : scrAmplitude.Minimum;

                        scrRise.Maximum = (scrRise.Maximum.CompareTo(currentRise) < 0) ? currentRise : scrRise.Maximum;
                        scrRise.Minimum = (scrRise.Minimum.CompareTo(currentRise) > 0) ? currentRise : scrRise.Minimum;

                        scrRecovery.Maximum = (scrRecovery.Maximum.CompareTo(currentRecouvery) < 0) ? currentRecouvery : scrRecovery.Maximum;
                        scrRecovery.Minimum = (scrRecovery.Minimum.CompareTo(currentRecouvery) > 0) ? currentRecouvery : scrRecovery.Minimum;
                    }

                    allMaximums.Add(currentAmplitude);
                    sumMaximums += currentAmplitude;

                    if (i > 0)
                    {
                        allRises.Add(currentRise);
                        sumRises += currentRise;

                        allRecoveryTimes.Add(currentRecouvery);
                        sumRecoveryTime += currentRecouvery;
                    }
                }
            }

            scrAmplitude.Mean = (allMaximums != null && allMaximums.Count > 0) ? Convert.ToDecimal(sumMaximums / allMaximums.Count) : 0;
            scrAmplitude.Count = allMaximums.Count / timeWindow ;
            scrAmplitude.StdDeviation = GetStandardDeviation(allMaximums, scrAmplitude.Mean);
            arousalStat.SCRAmplitude = scrAmplitude;

            scrRise.Mean = (allRises != null && allRises.Count > 0) ? Convert.ToDecimal(sumRises / allRises.Count) : 0;
            scrRise.Count = allRises.Count / timeWindow ;
            scrRise.StdDeviation = GetStandardDeviation(allRises, scrRise.Mean);
            arousalStat.SCRRise = scrRise;

            scrRecovery.Mean = (allRecoveryTimes != null && allRecoveryTimes.Count > 0) ? Convert.ToDecimal(sumRecoveryTime / allRecoveryTimes.Count) : 0;
            scrRecovery.Count = allRecoveryTimes.Count / timeWindow ;
            scrRise.StdDeviation = GetStandardDeviation(allRecoveryTimes, scrRecovery.Mean);
            arousalStat.SCRRecoveryTime = scrRecovery;
            

            return arousalStat;
        }

        private double GetLineBetween2Points(InflectionPoint point1, InflectionPoint point2, double x)
        {
            double dx = point2.CoordinateX - point1.CoordinateX;  //This part has problem in your code
            if (dx == 0)
                return float.NaN;
            var a = (point2.CoordinateY - point1.CoordinateY) / dx;
            var b = point1.CoordinateY - (a * point1.CoordinateX);

            return a * x + b;
        }

        private decimal GetStandardDeviation(List<double> allMaximums, decimal mean)
        {
            double stdDeviation = 0;
            foreach(double localMaximum in allMaximums)
            {
                stdDeviation += Math.Pow((localMaximum - (double)mean), 2);
            }

            return (allMaximums.Count > 0) ? Convert.ToDecimal(Math.Sqrt(stdDeviation / allMaximums.Count)) : 0;
        }

        public List<InflectionPoint> AffectedCoordinatePoints(Dictionary<double, double> coordinates, int numberOfAffectedPoints)
        {
            List<InflectionPoint> result = new List<InflectionPoint>();

            for (int i = (coordinates.Count - numberOfAffectedPoints); i < coordinates.Count - 1; i++)
            {
                result.Add(new InflectionPoint(coordinates.ElementAt(i).Key, coordinates.ElementAt(i).Value, i));
            }

            return result;
        }

        //print all values from the binary file - used for test
        public void PrintChannelsValues()
        {
            foreach (KeyValuePair<int, List<double>> member in channelsValues)
            {
                Logger.Log("Channel with number: " + member.Key);
                List<double> valuesPerChannel = member.Value;
                foreach (int value in valuesPerChannel)
                {
                    Logger.Log("value: " + value);
                }
            }
        }

        public void FillCoordinates(int samplerate)
        {
            //coordinates save the received GSR value for a millisecond
            coordinates = new Dictionary<int, Dictionary<double, double>>();

            foreach (KeyValuePair<int, List<double>> entry in channelsValues)
            {
                int currentChannel = entry.Key;
                List<double> channelValues = entry.Value;

                
                Dictionary<double, double> currentChannelCoordinates = new Dictionary<double, double>();
                int channelValuesCount = channelValues.Count;
                for (int i = channelValuesCount - 1; i > -1; i--)
                {
                    //TODO: to be calculate according to the sample rate value
                    double time = gsrValuesReadTime - (channelValuesCount - i - 1) * (1000/samplerate);
                    double gsrValue = (1 / channelValues[i])*4000;
                    if (!currentChannelCoordinates.ContainsKey(time))
                    {
                        currentChannelCoordinates.Add(time, gsrValue);
                    }
                }

                if (!coordinates.ContainsKey(currentChannel))
                {
                    coordinates.Add(currentChannel, currentChannelCoordinates);
                }
            }
        }

        public Dictionary<int, Dictionary<double, double>> GetCoordinates()
        {
            return coordinates;
        }

        public string GetJSONArousalStatistics(ArousalStatistics statisticObject)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = js.Serialize(statisticObject);

            Logger.Log("jsonObject: " + json);

            return json;
        }

        public ArousalStatistics GetArousalStatistics()
        {
            return GetArousalStatistics(defaultTimeWindow, TimeWindowMeasure.Seconds);
        }

        public ArousalStatistics GetArousalStatistics(double timeWindow, TimeWindowMeasure timeScale)
        {
            ISignalDeviceController signalController = new GSRHRDevice();

            Dictionary<int, List<double>> channelsValues = ExtractChannelsValues();
            //Dictionary<int, List<double>> channelsValues = gsrHandler.ExtractChannelsValuesFromFile();

            int sampleRate = signalController.GetSignalSampleRate();
            FillCoordinates(sampleRate);

            Dictionary<int, Dictionary<double, double>> medianFilterCoordinates = GetMedianFilterPoints(coordinates);
            if (medianFilterCoordinates != null) return GetArousalStatisticsByMedianFilter(medianFilterCoordinates.Values.ElementAt(GSR_CHANNEL), timeWindow, timeScale);

            return null;
        }

        public ArousalStatistics GetArousalStatisticsByMedianFilter(Dictionary<double, double> medianFilterCoordinates, double timeWindow, TimeWindowMeasure timeMeasure)
        {
            ArousalStatistics result = new ArousalStatistics();
            ISignalDeviceController signalController = new GSRHRDevice();
            int sampleRate = signalController.GetSignalSampleRate();
            ButterworthFilterCoordinates butterworthFilterCoordinates = new ButterworthFilterCoordinates();
            Dictionary<double, double> lowPassFilterCoordinates = new Dictionary<double, double>();
            Dictionary<double, double> highPassFilterCoordinates = new Dictionary<double, double>();

            if(timeWindow.CompareTo(0.0) <= 0)
            {
                timeWindow = defaultTimeWindow;
                timeMeasure = TimeWindowMeasure.Seconds;
            }

            medianFilterCoordinates = medianFilterCoordinates.OrderBy(key => key.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
            butterworthFilterCoordinates = GetButterworthHighLowPassCoordinates(sampleRate, medianFilterCoordinates);
            lowPassFilterCoordinates = butterworthFilterCoordinates.LowPassCoordinates;
            highPassFilterCoordinates = butterworthFilterCoordinates.HighPassCoordinates;

            ArousalStatistics butterworthStatistics = GetInflectionPoints(highPassFilterCoordinates, sampleRate, timeWindow, timeMeasure);
            if (butterworthStatistics != null)
            {
                butterworthStatistics.TonicStatistics = GetTonicStatistics(lowPassFilterCoordinates);
                butterworthStatistics.SCLAchievedArousalLevel = GetTonicLevel(butterworthStatistics.TonicStatistics.MeanAmp);
            }

            return butterworthStatistics;
        }

        private ArousalStatistics GetInflectionPoints(Dictionary<double, double> coordinates, int sampleRate, double timeWindow, TimeWindowMeasure timeScale)
        {
            List<InflectionPoint> chartPointsCoordinates = TransformToCoordinatePoints(coordinates);
            InflectionLine inflLineHandler = new InflectionLine();
            List<InflectionPoint> inflectionPoints = inflLineHandler.GetInflectionPoints(chartPointsCoordinates);

            return GetArousalStatistics(coordinates, timeWindow, timeScale, sampleRate);

        }

        private ArousalStatistics GetInflectionPoints(Dictionary<double, double> coordinates)
        {
            ISignalDeviceController signalController = new GSRHRDevice();
            int sampleRate = signalController.GetSignalSampleRate();
            return GetInflectionPoints(coordinates, sampleRate, defaultTimeWindow, TimeWindowMeasure.Seconds);

        }

        private static List<InflectionPoint> TransformToCoordinatePoints(Dictionary<double, double> coordinates)
        {
            List<InflectionPoint> result = new List<InflectionPoint>();
            int i = 0;
            foreach(KeyValuePair<double, double> coordinate in coordinates)
            {
                result.Add(new InflectionPoint(coordinate.Key, coordinate.Value, i));
                i++;
            }

            return result;
        }

        public ButterworthFilterCoordinates GetButterworthHighLowPassCoordinates(int sampleRate, Dictionary<double, double> medianCoordinatesValues)
        {
            FilterButterworth highPassFilter = new FilterButterworth(BUTTERWORTH_TONIC_PHASIC_FREQUENCY, sampleRate, ButterworthPassType.Highpass);
            FilterButterworth lowPassFilter = new FilterButterworth(BUTTERWORTH_TONIC_PHASIC_FREQUENCY, sampleRate, ButterworthPassType.Lowpass);

            Dictionary<double, double> lowPassFilterCoordinates = new Dictionary<double, double>();
            Dictionary<double, double> highPassFilterCoordinates = new Dictionary<double, double>();

            foreach (KeyValuePair<double, double> medianCoordinate in medianCoordinatesValues)
            {
                double medianValue = medianCoordinate.Value;
                double highPassValue = highPassFilter.GetFilterValue(medianValue);
                double lowPassValue = lowPassFilter.GetFilterValue(medianValue);

                lowPassFilterCoordinates.Add(medianCoordinate.Key, lowPassValue);
                highPassFilterCoordinates.Add(medianCoordinate.Key, highPassValue);
            }

            ButterworthFilterCoordinates result = new ButterworthFilterCoordinates();
            result.LowPassCoordinates = lowPassFilterCoordinates;
            result.HighPassCoordinates = highPassFilterCoordinates;

            return result;
        }

        public string EndOfCalibrationPeriod()
        {
            ArousalStatistics statistics = GetArousalStatistics();
            Logger.Log("\n\n\n\n\n\n\n");
            Logger.Log("Received message for end of calibration");
            Logger.Log("MinAverageArousalArea: " + settings.AppSettings.Settings["MinAverageArousalArea"].Value);
            Logger.Log("SCRArousalArea: " + statistics.SCRArousalArea);
            double calibrationRatioSCR = Math.Round(Math.Abs(statistics.SCRArousalArea / Convert.ToDouble(
                settings.AppSettings.Settings["MinAverageArousalArea"].Value)), 4) ;
            double deltaSCL = Math.Round(statistics.TonicStatistics.MeanAmp - Convert.ToDouble(
                settings.AppSettings.Settings["MinAverageTonicAmplitude"].Value), 4);
            settings.AppSettings.Settings["CalibrationMinArousalArea"].Value = RoundString((Convert.ToDouble(
                settings.AppSettings.Settings["MinAverageArousalArea"].Value) * calibrationRatioSCR).ToString());
            settings.AppSettings.Settings["CalibrationMinTonicAmplitude"].Value = RoundString(statistics.TonicStatistics.MeanAmp.ToString());
            settings.AppSettings.Settings["CalibrationMaxArousalArea"].Value = RoundString((Convert.ToDouble(
                settings.AppSettings.Settings["MaxAverageArousalArea"].Value) * calibrationRatioSCR).ToString());
            settings.AppSettings.Settings["CalibrationMaxTonicAmplitude"].Value = RoundString((Convert.ToDouble(
                settings.AppSettings.Settings["MaxAverageTonicAmplitude"].Value) + deltaSCL).ToString());

            settings.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            Logger.Log("Calibration data: " + GetJSONArousalStatistics(statistics));
            Logger.Log("calibrationRatioSCR: " + calibrationRatioSCR);
            Logger.Log("calibrationRatioSCL: " + deltaSCL);
            Logger.Log("calibrationMinArousalArea: " + settings.AppSettings.Settings["CalibrationMinArousalArea"].Value);
            Logger.Log("calibrationMinTonicAmplitude: " + settings.AppSettings.Settings["CalibrationMinTonicAmplitude"].Value);
            Logger.Log("calibrationMaxArousalArea: " + settings.AppSettings.Settings["CalibrationMaxArousalArea"].Value);
            Logger.Log("calibrationMaxTonicAmplitude: " + settings.AppSettings.Settings["CalibrationMaxTonicAmplitude"].Value);
            Logger.Log("The calibration process was executed.");

            return "The calibration process was executed.";
        }

        public string EndOfMeasurement()
        {
            Logger.Log("Received message for end of measurement");
            Logger.Log("Old settings.NumberParticipants: " + GetAppValue("NumberParticipants"));
            Logger.Log("Old settings.MinAverageArousalArea: " + GetAppValue("MinAverageArousalArea"));
            Logger.Log("Old settings.MaxAverageArousalArea: " + GetAppValue("MaxAverageArousalArea"));
            Logger.Log("Old settings.MinAverageTonicAmplitude: " + GetAppValue("MinAverageTonicAmplitude"));
            Logger.Log("Old settings.MaxAverageTonicAmplitude: " + GetAppValue("MaxAverageTonicAmplitude"));

            Logger.Log("NumberParticipants: " + GetAppValue("NumberParticipants"));
            Logger.Log("minArousalArea: " + GetAppValue("MinArousalArea"));
            Logger.Log("maxArousalArea: " + GetAppValue("MaxArousalArea"));
            Logger.Log("minTonicAmplitude: " + GetAppValue("MinTonicAmplitude"));
            Logger.Log("maxTonicAmplitude: " + GetAppValue("MaxTonicAmplitude"));

            int oldNumberParticipants = Convert.ToInt32(GetAppValue("NumberParticipants"));
            int currentNumberParticipants = oldNumberParticipants + 1;
            if (GetAppValue("MinAbsoluteArousalArea").CompareTo(GetAppValue("MinArousalArea")) > 0)
            {
                settings.AppSettings.Settings["MinAbsoluteArousalArea"].Value = RoundString(settings.AppSettings.Settings["MinArousalArea"].Value);
            }
            if (GetAppValue("MaxAbsoluteArousalArea").CompareTo(GetAppValue("MaxArousalArea")) < 0)
            {
                settings.AppSettings.Settings["MaxAbsoluteArousalArea"].Value = Math.Round(GetAppValue("MaxArousalArea"), 4).ToString();
            }
            settings.AppSettings.Settings["MinAverageArousalArea"].Value = RoundString(( (
                GetAppValue("MinAverageArousalArea") * oldNumberParticipants + 
                GetAppValue("MinArousalArea") ) / currentNumberParticipants ).ToString());
            settings.AppSettings.Settings["MaxAverageArousalArea"].Value = RoundString(
               ( ( GetAppValue("MaxAverageArousalArea") * oldNumberParticipants + 
                GetAppValue("MaxArousalArea") ) / currentNumberParticipants).ToString());

            if (GetAppValue("MinTonicAmplitude").CompareTo(GetAppValue("MinAbsoluteTonicAmplitude")) < 0)
            {
                settings.AppSettings.Settings["MinAbsoluteTonicAmplitude"].Value = Math.Round(GetAppValue("MinTonicAmplitude"), 4).ToString();
            }
            if (GetAppValue("MaxTonicAmplitude").CompareTo(GetAppValue("MaxAbsoluteTonicAmplitude")) > 0)
            {
                settings.AppSettings.Settings["MaxAbsoluteTonicAmplitude"].Value = Math.Round(GetAppValue("MaxTonicAmplitude"), 4).ToString();
            }
            settings.AppSettings.Settings["MinAverageTonicAmplitude"].Value = 
                ( (GetAppValue("MinAverageTonicAmplitude") * oldNumberParticipants + 
                GetAppValue("MinTonicAmplitude")) / currentNumberParticipants).ToString();
            settings.AppSettings.Settings["MaxAverageTonicAmplitude"].Value = RoundString( ( (
                GetAppValue("MaxAverageTonicAmplitude") * oldNumberParticipants +
                GetAppValue("MaxTonicAmplitude")) / currentNumberParticipants).ToString());

            settings.AppSettings.Settings["NumberParticipants"].Value = currentNumberParticipants.ToString();

            settings.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            //re-initialize min/max for arousal area and tonic amplitude
            settings.AppSettings.Settings["MaxArousalArea"].Value = "-1";
            settings.AppSettings.Settings["MinArousalArea"].Value = "-1";
            settings.AppSettings.Settings["MinTonicAmplitude"].Value = "100";
            settings.AppSettings.Settings["MaxTonicAmplitude"].Value = "-100";

            settings.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            Logger.Log("New settings.NumberParticipants: " + GetAppValue("NumberParticipants"));
            Logger.Log("New settings.MinAverageArousalArea: " + GetAppValue("MinAverageArousalArea"));
            Logger.Log("New settings.MaxAverageArousalArea: " + GetAppValue("MaxAverageArousalArea"));
            Logger.Log("New settings.MinAverageTonicAmplitude: " + GetAppValue("MinAverageTonicAmplitude"));
            Logger.Log("New settings.MaxAverageTonicAmplitude: " + GetAppValue("MaxAverageTonicAmplitude"));
            Logger.Log("End...");

            return "The measurement process was ended.";
        }

        private string RoundString(string value)
        {
            return Math.Round(Convert.ToDouble(value), 4).ToString();
        }

        private double GetAppValue(string value)
        {
            return Convert.ToDouble(settings.AppSettings.Settings[value].Value);
        }

        public void InitializeMinMaxValues()
        {

            settings.AppSettings.Settings["MinAbsoluteArousalArea"].Value = "800";
            settings.AppSettings.Settings["MaxAbsoluteArousalArea"].Value = "4000";
            settings.AppSettings.Settings["MinAverageArousalArea"].Value = "700";
            settings.AppSettings.Settings["MaxAverageArousalArea"].Value = "4000";
            settings.AppSettings.Settings["MinAbsoluteTonicAmplitude"].Value = "-0.05";
            settings.AppSettings.Settings["MaxAbsoluteTonicAmplitude"].Value = "3.12";
            settings.AppSettings.Settings["MinAverageTonicAmplitude"].Value = "-0.05";
            settings.AppSettings.Settings["MaxAverageTonicAmplitude"].Value = "3.12";
            settings.AppSettings.Settings["NumberParticipants"].Value = "1";

            settings.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
