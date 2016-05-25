using System;
using SignalDevice;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using RealTimeArousalDetection.Properties;

namespace Assets.Rage.GSRAsset
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
        private Settings settings = Settings.Default;

        private int arousalLevel;

        private double? minArousalArea;
        private double? maxArousalArea;
        private double? minTonicAmplitude;
        private double? maxTonicAmplitude;

        private double calibrationMinArousalArea;
        private double calibrationMaxArousalArea;
        private double calibrationMinTonicAmplitude;
        private double calibrationMaxTonicAmplitude;

        private double defaultTimeWindow;

        private Logger logger = new Logger();

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

            arousalLevel = 3;
            defaultTimeWindow = 10;

            calibrationMinArousalArea = settings.MinAverageArousalArea;
            calibrationMaxArousalArea = settings.MaxAverageArousalArea;
            calibrationMinTonicAmplitude = settings.MinAverageTonicAmplitude;
            calibrationMaxTonicAmplitude = settings.MaxAverageTonicAmplitude;

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

        public ArousalStatistics GetArousalStatistics(Dictionary<double, double> coordinates, double timeWindow, TimeWindowMeasure timewindowType, int sampleRate)
        {
            if (timeWindow.CompareTo(0) <= 0 || sampleRate <= 0 || coordinates.Count <= 0) return null;

            int numberOfAffectedPoints = GetNumberOfAffectedPoints(timeWindow, timewindowType, sampleRate);

            if (coordinates.Count < numberOfAffectedPoints) return GetArousalStatistics(coordinates);
            return GetArousalInfoForCoordinates(coordinates, numberOfAffectedPoints);
        }

        public ArousalStatistics GetArousalStatistics(Dictionary<double, double> coordinates)
        {
            return GetArousalInfoForCoordinates(coordinates, coordinates.Count);
        }

        private ArousalStatistics GetArousalInfoForCoordinates(Dictionary<double, double> coordinates, int numberOfAffectedPoints)
        {
            InflectionLine inflectionLinesHandler = new InflectionLine();
            List<InflectionPoint> inflectionPoints = inflectionLinesHandler.GetInflectionPoints(AffectedCoordinatePoints(coordinates, numberOfAffectedPoints));
            ArousalStatistics result = new ArousalStatistics();
            result = GetArousalInfoForInflectionPoints(inflectionPoints);
            result.SCRArousalArea = GetArousalArea(coordinates, numberOfAffectedPoints);
            result.SCRAchievedArousalLevel = GetPhasicLevel(result.SCRArousalArea);
            SetMinMaxArousalArea(result.SCRArousalArea);
            
            return result;
        }

        private void SetMinMaxArousalArea(double scrArousalArea)
        {
            if (minArousalArea == null) minArousalArea = scrArousalArea;
            if(scrArousalArea.CompareTo(minArousalArea) < 0)
            {
                minArousalArea = scrArousalArea;
            }

            if (maxArousalArea == null) maxArousalArea = scrArousalArea;
            if(scrArousalArea.CompareTo(maxArousalArea) > 0)
            {
                maxArousalArea = scrArousalArea;
            }
        }

        private int GetTonicLevel(double tonicAverageAmplitude)
        {
            if (tonicAverageAmplitude.CompareTo(calibrationMinTonicAmplitude) <= 0)
            {
                calibrationMinTonicAmplitude = tonicAverageAmplitude;
                return 1;
            }

            if (tonicAverageAmplitude.CompareTo(calibrationMaxTonicAmplitude) >= 0)
            {
                calibrationMaxTonicAmplitude = tonicAverageAmplitude;
                return arousalLevel;
            }

            double step = (arousalLevel != 0) ? (calibrationMaxTonicAmplitude - calibrationMinTonicAmplitude) / arousalLevel : 0.0;
            return (step.CompareTo(0.0) != 0) ? (int)Math.Ceiling((tonicAverageAmplitude - calibrationMinTonicAmplitude) / step) : 0;
        }

        private int GetPhasicLevel(double scrArousalArea)
        {
            if(scrArousalArea.CompareTo(calibrationMinArousalArea) <= 0)
            {
                calibrationMinArousalArea = scrArousalArea;
                return 1;
            }

            if(scrArousalArea.CompareTo(calibrationMaxArousalArea) >= 0)
            {
                calibrationMaxArousalArea = scrArousalArea;
                return arousalLevel;
            }

            double step = (arousalLevel != 0) ?(calibrationMaxArousalArea - calibrationMinArousalArea) / arousalLevel : 0.0;
            return (step.CompareTo(0.0) != 0) ? (int)Math.Ceiling((scrArousalArea - calibrationMinArousalArea) / step) : 0;
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
            result.SCLAchievedArousalLevel = GetTonicLevel(result.MeanAmp);

            SetMinMaxTonicAmplitude(result.MinAmp, result.MaxAmp);

            return result;
        }

        private void SetMinMaxTonicAmplitude(double minAmp, double maxAmp)
        {
            if (minTonicAmplitude == null) minTonicAmplitude = minAmp;
            if (minAmp.CompareTo(minTonicAmplitude) < 0)
            {
                minTonicAmplitude = minAmp;
            }

            if (maxTonicAmplitude == null) maxTonicAmplitude = maxAmp;
            if (maxAmp.CompareTo(maxTonicAmplitude) > 0)
            {
                maxTonicAmplitude = maxAmp;
            }
        }

        private double GetArousalArea(Dictionary<double, double> coordinates, int numberOfAffectedPoints)
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
            
            return area;
        }

        public ArousalStatistics GetArousalInfoForInflectionPoints(List<InflectionPoint> inflectionPoints)
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

            //Logger.Log("Start...");
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
                //Logger.Log("infl. point: x1=" + x1 + ", y1=" + y1 + ", x2=" + x2 + ", y2=" + y2);
                //Logger.Log("Line length: " + inflectionLinesHandler.GetLineLength(x1, y1, x2, y2));

                if (inflectionLinesHandler.GetDirection(x1, y1, x2, y2).Equals(InflectionLineDirection.Positive))
                {
                    //Logger.Log("Positive SummaryArousal: " + arousalProgress.SummaryArousal);
                    arousalStat.NumberPositiveInflectionPoints++;
                    arousalStat.SummaryArousal += inflectionLinesHandler.GetLineLength(x1, y1, x2, y2);
                    //Logger.Log("Positive SummaryArousal2: " + arousalProgress.SummaryArousal);
                }
                else if (inflectionLinesHandler.GetDirection(x1, y1, x2, y2).Equals(InflectionLineDirection.Negative))
                {
                    //Logger.Log("Negative SummaryArousal: " + arousalProgress.SummaryArousal);
                    arousalStat.NumberNegativeInflectionPoints++;
                    arousalStat.SummaryArousal -= inflectionLinesHandler.GetLineLength(x1, y1, x2, y2);
                    //Logger.Log("Negative SummaryArousal2: " + arousalProgress.SummaryArousal);
                }
                else if (inflectionLinesHandler.GetDirection(x1, y1, x2, y2).Equals(InflectionLineDirection.Neutral))
                {
                    //Logger.Log("Negative SummaryArousal: " + arousalProgress.SummaryArousal);
                    arousalStat.NumberOfNeutralInflectionPoints++;
                    arousalStat.SummaryArousal -= inflectionLinesHandler.GetLineLength(x1, y1, x2, y2);
                    //Logger.Log("Negative SummaryArousal2: " + arousalProgress.SummaryArousal);
                }
            }

            arousalStat.NumberInflectionPoints = arousalStat.NumberPositiveInflectionPoints + 
                                                     arousalStat.NumberNegativeInflectionPoints + 
                                                     arousalStat.NumberOfNeutralInflectionPoints;
            scrAmplitude.Mean = (allMaximums != null && allMaximums.Count > 0) ? Convert.ToDecimal(sumMaximums / allMaximums.Count) : 0;
            scrAmplitude.Count = allMaximums.Count;
            scrAmplitude.StdDeviation = GetStandardDeviation(allMaximums, scrAmplitude.Mean);
            arousalStat.SCRAmplitude = scrAmplitude;

            scrRise.Mean = (allRises != null && allRises.Count > 0) ? Convert.ToDecimal(sumRises / allRises.Count) : 0;
            scrRise.Count = allRises.Count;
            scrRise.StdDeviation = GetStandardDeviation(allRises, scrRise.Mean);
            arousalStat.SCRRise = scrRise;

            scrRecovery.Mean = (allRecoveryTimes != null && allRecoveryTimes.Count > 0) ? Convert.ToDecimal(sumRecoveryTime / allRecoveryTimes.Count) : 0;
            scrRecovery.Count = allRecoveryTimes.Count;
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

        public static void Log(object logMessage)
        {
            String currentDate = DateTime.Now.ToString("yyyyMMdd");
            String logFileName = ConfigurationManager.AppSettings.Get("LogFile").Replace(".txt", currentDate + ".txt");
            using (StreamWriter w = File.AppendText(logFileName))
            {
                logMessage = logMessage.ToString();
                w.Write("\r\nLog Entry : ");
                w.Write("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                w.WriteLine("  :{0}", logMessage);
                w.WriteLine("-------------------------------");
            }
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

            Log("jsonObject: " + json);

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

            butterworthFilterCoordinates = GetButterworthHighLowPassCoordinates(sampleRate, medianFilterCoordinates);
            lowPassFilterCoordinates = butterworthFilterCoordinates.LowPassCoordinates;
            highPassFilterCoordinates = butterworthFilterCoordinates.HighPassCoordinates;

            ArousalStatistics butterworthStatistics = GetInflectionPoints(highPassFilterCoordinates, sampleRate, timeWindow, timeMeasure);
            if (butterworthStatistics != null) butterworthStatistics.TonicStatistics = GetTonicStatistics(lowPassFilterCoordinates);


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
            Log("Calibration data: " + GetJSONArousalStatistics(statistics));
            double calibrationRatioSCR = statistics.SCRArousalArea / settings.MinAverageArousalArea;    
            Log("calibrationRatioSCR: " + calibrationRatioSCR);
            double calibrationRatioSCL = statistics.TonicStatistics.MinAmp / settings.MinAverageTonicAmplitude;
            Log("calibrationRatioSCL: " + calibrationRatioSCL);
            calibrationMinArousalArea = settings.MinAverageArousalArea * calibrationRatioSCR;
            calibrationMinTonicAmplitude = settings.MinAverageTonicAmplitude * calibrationRatioSCL;
            calibrationMaxArousalArea = settings.MaxAverageArousalArea * calibrationRatioSCR;
            calibrationMaxTonicAmplitude = settings.MinAverageTonicAmplitude * calibrationRatioSCL;
            Log("calibrationMinArousalArea: " + calibrationMinArousalArea);
            Log("calibrationMinTonicAmplitude: " + calibrationMinTonicAmplitude);
            Log("calibrationMaxArousalArea: " + calibrationMaxArousalArea);
            Log("calibrationMaxTonicAmplitude: " + calibrationMaxTonicAmplitude);

            return "The calibration process was executed.";
        }

        public string EndOfMeasurement()
        {
            Log("Start...");
            Log("Old settings.NumberParticipants: " + settings.NumberParticipants);
            Log("Old settings.MinAverageArousalArea: " + settings.MinAverageArousalArea);
            Log("Old settings.MaxAverageArousalArea: " + settings.MaxAverageArousalArea);
            Log("Old settings.MinAverageTonicAmplitude: " + settings.NumberParticipants);
            Log("Old settings.MaxAverageTonicAmplitude: " + settings.NumberParticipants);

            int currentNumberParticipants = settings.NumberParticipants + 1;
            int oldNumberParticipants = settings.NumberParticipants + 1;
            if (minArousalArea != null && settings.MinAbsoluteArousalArea.CompareTo(minArousalArea) > 0) settings.MinAbsoluteArousalArea = minArousalArea.GetValueOrDefault();
            if (maxArousalArea != null && settings.MaxAbsoluteArousalArea.CompareTo(maxArousalArea) < 0) settings.MaxAbsoluteArousalArea = maxArousalArea.GetValueOrDefault();
            settings.MinAverageArousalArea = (settings.MinAverageArousalArea * oldNumberParticipants + minArousalArea.GetValueOrDefault()) / currentNumberParticipants;
            settings.MaxAverageArousalArea = (settings.MaxAverageArousalArea * oldNumberParticipants + maxArousalArea.GetValueOrDefault()) / settings.MaxAverageArousalArea;

            if (minTonicAmplitude != null && minTonicAmplitude.GetValueOrDefault().CompareTo(settings.MinAbsoluteTonicAmplitude) < 0) settings.MinAbsoluteTonicAmplitude = minTonicAmplitude.GetValueOrDefault();
            if (maxTonicAmplitude != null && maxTonicAmplitude.GetValueOrDefault().CompareTo(settings.MaxAbsoluteTonicAmplitude) > 0) settings.MaxAbsoluteTonicAmplitude = maxTonicAmplitude.GetValueOrDefault();
            settings.MinAverageTonicAmplitude = (settings.MinAverageTonicAmplitude * oldNumberParticipants + minArousalArea.GetValueOrDefault()) / currentNumberParticipants;
            settings.MaxAverageTonicAmplitude = (settings.MaxAverageTonicAmplitude * oldNumberParticipants + maxTonicAmplitude.GetValueOrDefault()) / currentNumberParticipants;

            settings.NumberParticipants = currentNumberParticipants;

            settings.Save();

            Log("New settings.NumberParticipants: " + settings.NumberParticipants);
            Log("New settings.MinAverageArousalArea: " + settings.MinAverageArousalArea);
            Log("New settings.MaxAverageArousalArea: " + settings.MaxAverageArousalArea);
            Log("New settings.MinAverageTonicAmplitude: " + settings.NumberParticipants);
            Log("New settings.MaxAverageTonicAmplitude: " + settings.NumberParticipants);
            Log("End...");
            return "The measurement process was ended.";
        }
    }
}
