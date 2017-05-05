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

using System;
using Assets.Rage.GSRAsset.SignalDevice;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Timers;
using AssetPackage;
using AssetManagerPackage;
using System.Globalization;

namespace Assets.Rage.GSRAsset.Utils
{
    public class GSRSignalProcessor
    {
        public SignalDataByTime[] signalValues;
        public String pathToBinaryFile;
        public const double BUTTERWORTH_TONIC_PHASIC_FREQUENCY = 0.05;
        private RealTimeArousalDetectionAssetSettings settings;
        private ILog logger;
        private int sampleRate = 0;
        ISignalDeviceController signalController;
        double timeWhenSignalValuesAreExtracted;

        private int arousalLevel;

        private double defaultTimeWindow;
        private double HIGHPASS_ADJUSTING_VARIABLE = 3000.00;

        private double minArousalArea;
        private double maxArousalArea;
        private double minTonicAmplitude;
        private double maxTonicAmplitude;
        private double minMovingAverage;
        private double maxMovingAverage;
        private Timer calibrationTimer;

        public GSRSignalProcessor()
        {
            signalController = GSRHRDevice.Instance;
            settings = RealTimeArousalDetectionAssetSettings.Instance;
            logger = (ILog)AssetManager.Instance.Bridge;

            sampleRate = !"TestWithoutDevice".Equals(settings.ApplicationMode) ? signalController.GetSignalSampleRate() : 0;
            signalValues = GetSignalValues();

            arousalLevel = settings.ArousalLevel;
            defaultTimeWindow = settings.DefaultTimeWindow;

            calibrationTimer = new Timer(settings.CalibrationTimerInterval);
            calibrationTimer.Elapsed += CalibrationTimer_Elapsed;

            InitializeMinMaxStatistics();
        }

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


        /// <summary>
        /// Get all available signal values in the cache.
        /// </summary>
        /// 
        /// <returns>
        /// All available signal values.
        /// </returns>
        public SignalDataByTime[] GetSignalValues()
        {
            signalValues = CacheSignalData.GetCacheData();
            timeWhenSignalValuesAreExtracted = (double)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            return signalValues;
        }

        /// <summary>
        /// Apply Median filter on a set of sigal data.
        /// </summary>
        /// 
        /// <param name="signalCoordinates">List of signal data.</param>
        /// 
        /// <returns>
        /// Medial filter values for the list of signal data.
        /// </returns>
        public SignalDataByTime[] GetMedianFilterPoints(SignalDataByTime[] signalCoordinates)
        {
            if (signalCoordinates == null) signalCoordinates = signalValues;
            if (signalCoordinates == null) return null;
            FilterMedian filterMedian = new FilterMedian(signalCoordinates);

            return filterMedian.GetMedianFilterPoints();
        }

        /// <summary>
        /// Calculate arousal statistics for the passed time window in seconds.
        /// </summary>
        /// 
        /// <param name="timeWindowType">The type of time window: milliseconds or seconds.</param>
        /// <param name="timeWindow">Time wondow.</param>
        /// 
        /// <returns>
        /// Arousal statistics for the passed time window in seconds.
        /// </returns>
        public ArousalStatistics GetArousalStatistics(List<SignalDataByTime> highPassCoordinates, double timeWindow, TimeWindowMeasure timeWindowType, int sampleRate)
        {
            if (timeWindow.CompareTo(0) <= 0 || sampleRate <= 0 || highPassCoordinates.Count <= 0) return null;

            int numberOfAffectedPoints = GetNumberOfAffectedPoints(highPassCoordinates, timeWindow, timeWindowType, sampleRate);

            //if (coordinates.Count < numberOfAffectedPoints) return GetArousalInfoForCoordinates(coordinates, coordinates.Count, defaultTimeWindow);
            double timeWindowInSeconds = timeWindowType.Equals(TimeWindowMeasure.Milliseconds) ? timeWindow / 1000 : timeWindow;
            return GetArousalInfoForCoordinates(highPassCoordinates, numberOfAffectedPoints, timeWindowInSeconds);
        }


        /// <summary>
        /// Calculate arousal statistics for the last time window set in the configuration file.
        /// </summary>
        /// 
        /// <param name="coordinates">List of all signal values stored in the cache.</param>
        /// 
        /// <returns>
        /// Arousal statistics for the last default time window.
        /// </returns>
        public ArousalStatistics GetArousalStatistics(List<SignalDataByTime> coordinates)
        {
            return GetArousalInfoForCoordinates(coordinates, coordinates.Count, defaultTimeWindow);
        }

        /// <summary>
        /// Calculate arousal statistics for the passed time window.
        /// </summary>
        /// 
        /// <param name="highPassCoordinates">List of all signal values stored in the cache.</param>
        /// <param name="numberOfAffectedPoints">Number of affected points.</param>
        /// <param name="timeWindow">Time wondow.</param>
        /// 
        /// <returns>
        /// Arousal statistics for the passed time window.
        /// </returns>
        private ArousalStatistics GetArousalInfoForCoordinates(List<SignalDataByTime> highPassCoordinates, int numberOfAffectedPoints, double timeWindow)
        {
            InflectionLine inflectionLinesHandler = new InflectionLine();
            List<SignalDataByTime> highPassCoordinatesByTimeWindow = AffectedCoordinatePoints(highPassCoordinates, numberOfAffectedPoints);
            List<InflectionPoint> inflectionPoints = inflectionLinesHandler.GetInflectionPoints(highPassCoordinatesByTimeWindow, "highPass");
            ArousalStatistics result = new ArousalStatistics();
            result = GetArousalInfoForInflectionPoints(inflectionPoints, timeWindow);
            result.SCRArousalArea = GetArousalArea(highPassCoordinatesByTimeWindow, timeWindow) ;
            result.MovingAverage = GetMovingAverage(signalValues, numberOfAffectedPoints) ;
            result.GeneralArousalLevel = GetGeneralArousalLevel(result.MovingAverage);
            result.SCRAchievedArousalLevel = GetPhasicLevel(result.SCRArousalArea);
            result.LastValue = highPassCoordinates.ElementAt(highPassCoordinates.Count - 1).HighPassValue;
            result.LastRawSignalValue = signalValues.ElementAt(signalValues.Length - 1).SignalValue;
            
            return result;
        }

        /// <summary>
        /// Define the SCL level depending on the average values of tonic amplitudes.
        /// </summary>
        /// 
        /// <param name="tonicAverageAmplitude">Average values of tonic amplitudes.</param>
        /// 
        /// <returns>
        /// Tonic level.
        /// </returns>
        private int GetTonicLevel(double tonicAverageAmplitude)
        {
            if (Double.IsNaN(minTonicAmplitude) && Double.IsNaN(maxTonicAmplitude))
            {
                minTonicAmplitude = tonicAverageAmplitude;
                maxTonicAmplitude = tonicAverageAmplitude;
                return arousalLevel / 2;
            }
            
            if (tonicAverageAmplitude.CompareTo(minTonicAmplitude) <= 0)
            {
                minTonicAmplitude = tonicAverageAmplitude;
                return 1;
            }

            if (tonicAverageAmplitude.CompareTo(maxTonicAmplitude) >= 0)
            {
                maxTonicAmplitude = tonicAverageAmplitude;
                return arousalLevel;
            }

            double step = (arousalLevel != 0) ? (maxTonicAmplitude - minTonicAmplitude) / arousalLevel : 0.0;
            return (step.CompareTo(0.0) != 0) ? (int)Math.Ceiling((tonicAverageAmplitude - minTonicAmplitude) / step) : 0;
        }

        /// <summary>
        /// Define the phasic level depending on the phasic arousal area.
        /// </summary>
        /// 
        /// <param name="scrArousalArea">SCR arousal area.</param>
        /// 
        /// <returns>
        /// Phasic level (the level of arousal).
        /// </returns>
        private int GetPhasicLevel(double scrArousalArea)
        {
            if(Double.IsNaN(minArousalArea) && Double.IsNaN(maxArousalArea))
            {
                minArousalArea = scrArousalArea;
                maxArousalArea = scrArousalArea;
                return arousalLevel / 2;
            }
            if (scrArousalArea.CompareTo(minArousalArea) <= 0)
            {
                minArousalArea = scrArousalArea;
                return 1;
            }

            if(scrArousalArea.CompareTo(maxArousalArea) >= 0)
            {
                maxArousalArea = scrArousalArea;
                return arousalLevel;
            }

            double step = (arousalLevel != 0) ? (maxArousalArea - minArousalArea) / arousalLevel : 0.0;
            return (step.CompareTo(0.0) != 0) ? (int)Math.Ceiling((scrArousalArea - minArousalArea) / step) : 0;
        }

        /// <summary>
        /// Define the general arousal level depending on the moving average.
        /// </summary>
        /// 
        /// <param name="movingAverage">average of signal amplitudes (after median filter) for the last time window</param>
        /// 
        /// <returns>
        /// General level of arousal.
        /// </returns>
        private int GetGeneralArousalLevel(double movingAverage)
        {
            if(Double.IsNaN(minMovingAverage) && Double.IsNaN(maxMovingAverage))
            {
                minMovingAverage = 0.75*movingAverage;
                maxMovingAverage = 1.25*movingAverage;
            }

            if (movingAverage.CompareTo(minMovingAverage) <= 0)
            {
                minMovingAverage = movingAverage;
                return 1;
            }

            if(movingAverage.CompareTo(maxMovingAverage) >= 0)
            {
                maxMovingAverage = movingAverage;
                return arousalLevel;
            }

            double step = (arousalLevel != 0) ? (maxMovingAverage - minMovingAverage) / arousalLevel : 0.0;
            return (step.CompareTo(0.0) != 0) ? (int)Math.Ceiling((movingAverage - minMovingAverage) / step) : 0;
        }

        /// <summary>
        /// Return tonic statistic.
        /// </summary>
        /// 
        /// <param name="tonicCoordinates">List of SCL tonic signal values.</param>
        /// 
        /// <returns>
        /// Tonic statistic for a specified lst of SCL tonic signal values.
        /// </returns>
        public TonicStatistics GetTonicStatistics(List<SignalDataByTime> tonicCoordinates)
        {
            InflectionLine inflectionLinesHandler = new InflectionLine();
            List<InflectionPoint> inflectionPoints = inflectionLinesHandler.GetInflectionPoints(AffectedCoordinatePoints(tonicCoordinates, tonicCoordinates.Count), "lowPass");
            return GetTonicStatisticsForPoints(inflectionPoints);
        }

        /// <summary>
        /// Return tonic statistic.
        /// </summary>
        /// 
        /// <param name="tonicCoordinates">List of SCL tonic signal values.</param>
        /// <param name="numberOfAffectedPoints">Number of affected points.</param>
        /// 
        /// <returns>
        /// Tonic statistic for a specified lst of SCL tonic signal values.
        /// </returns>
        public TonicStatistics GetTonicStatistic(List<SignalDataByTime> tonicCoordinates, int numberOfAffectedPoints)
        {

            InflectionLine inflectionLinesHandler = new InflectionLine();
            List<InflectionPoint> inflectionPoints = inflectionLinesHandler.GetInflectionPoints(AffectedCoordinatePoints(tonicCoordinates, numberOfAffectedPoints), "lowPass");

            return GetTonicStatisticsForPoints(inflectionPoints);
        }

        /// <summary>
        /// Calculate number of affected signal values depending on time window.
        /// </summary>
        /// 
        /// <param name="coordinates">List of all signal values stored in the cache.</param>
        /// <param name="timeWindow">Time wondow.</param>
        /// <param name="sampleRate">Sample rate.</param>
        /// 
        /// <returns>
        /// Number of affected signal values.
        /// </returns>
        private int GetNumberOfAffectedPoints(List<SignalDataByTime> coordinates, double timeWindow, TimeWindowMeasure timewindowType, int sampleRate)
        {
            timeWindow = timewindowType.Equals(TimeWindowMeasure.Milliseconds) ? (timeWindow/1000) : timeWindow;
            int numberOfAffectedPoints = Convert.ToInt32((1000.0 / sampleRate) * timeWindow, CultureInfo.InvariantCulture);
            double minAcceptableTime = timeWhenSignalValuesAreExtracted - timeWindow * 1000;

            if (coordinates.Count < numberOfAffectedPoints) return coordinates.Count;

            for (int i = (coordinates.Count - numberOfAffectedPoints - 1); i < coordinates.Count; i++)
            {
                if (coordinates[i].Time < minAcceptableTime)
                {
                    numberOfAffectedPoints--;
                }
                else
                {
                    break;
                }

            }

            if ((coordinates.Count - numberOfAffectedPoints) > 1)
            {
                for (int i = (coordinates.Count - numberOfAffectedPoints - 2); i > -1; i--)
                {
                    if(coordinates[i].Time >= minAcceptableTime)
                    {
                        numberOfAffectedPoints++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return numberOfAffectedPoints;
        }

        /// <summary>
        /// Calculate SCL features: min/mean/max of the tonic amplitude and tonic slope.
        /// </summary>
        /// 
        /// <param name="inflectionPoints">List of SCL tonic signal values.</param>
        /// 
        /// <returns>
        /// Tonic statistic for a specified lst of SCL tonic signal values.
        /// </returns>
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
            double maxTonic = inflectionPoints.ElementAt(inflectionPoints.Count - 1).CoordinateY;

            double sumMaximums = 0;

            for (int i = 0; i < inflectionPoints.Count; i++)
            {
                if (inflectionPoints.ElementAt(i).ExtremaType.Equals(ExtremaType.Maximum))
                {
                    double currentY = inflectionPoints.ElementAt(i).CoordinateY;
                    minTonic = (minTonic.CompareTo(currentY) > 0 && !currentY.Equals(0.0)) ? currentY : minTonic;
                    maxTonic = (maxTonic.CompareTo(currentY) < 0) ? currentY : maxTonic;

                    double currentAmplitude = currentY;
                    allMaximums.Add(currentAmplitude);
                    sumMaximums += currentAmplitude;
                }
            }

            result.MinAmp = minTonic;
            result.MaxAmp = maxTonic;
            decimal mean = GetTonicMean(allMaximums, sumMaximums, result.MeanAmp);

            result.StdDeviation = GetStandardDeviation(allMaximums, mean);

            return result;
        }

        private static decimal GetTonicMean(List<double> allMaximums, double sumMaximums, double tonicMeanAmp)
        {
            if (allMaximums.Count == 1) return Convert.ToDecimal(tonicMeanAmp, CultureInfo.InvariantCulture);
            return (allMaximums != null && allMaximums.Count > 0) ? Convert.ToDecimal(sumMaximums / allMaximums.Count, CultureInfo.InvariantCulture) : 0;
        }

        /// <summary>
        /// Calculate average of amplitudes of the GSR signal values that is an indicator for the general arousal. 
        /// </summary>
        ///
        /// <param name="coordinates"> List of GSR signal values. </param>
        /// <param name="numberOfAffectedPoints"> Number of signal values (depending on time window) that should participate in the calculation. </param>
        ///
        /// <returns>
        /// Average of GSR signal amplitudes.
        /// </returns>
        private double GetMovingAverage(SignalDataByTime[] coordinates, int numberOfAffectedPoints)
        {
            double movingAverage = 0;
            for (int i = (coordinates.Length - numberOfAffectedPoints); i < coordinates.Length; i++)
            {
                movingAverage += Math.Abs(coordinates.ElementAt(i).SignalValue);
            }

            return (movingAverage / numberOfAffectedPoints);
        }

        /// <summary>
        /// Calculate area of the signal. 
        /// </summary>
        ///
        /// <param name="coordinates"> List of inflection points. </param>
        /// <param name="numberOfAffectedPoints"> Number of signal values (depending on time window) that should participate in the calculation. </param>
        /// <param name="timeWindow"> Time window. </param>
        ///
        /// <returns>
        /// Area of the signal.
        /// </returns>
        private double GetArousalArea(List<SignalDataByTime> coordinates, double timeWindow)
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

            for(int i = 0; i < (coordinates.Count - 1); i++)
            {
                
                if (i < (coordinates.Count - 2))
                {
                    double x1 = coordinates.ElementAt(i).Time;
                    double y1 = coordinates.ElementAt(i).SignalValue;
                    //double y1 = coordinates.ElementAt(i).HighPassValue;
                    double x2 = coordinates.ElementAt(i + 1).Time;
                    double y2 = coordinates.ElementAt(i + 1).SignalValue;
                    //double y2 = coordinates.ElementAt(i + 1).HighPassValue;

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

        /// <summary>
        /// Calculate following arousal features: 
        /// - SCR Amplitude;
        /// - SCR Rise
        /// - SCR Recovery
        /// </summary>
        ///
        /// <param name="inflectionPoints"> List of inflection points. </param>
        /// <param name="timeWindow"> Time window. </param>
        ///
        /// <returns>
        /// Arousal statistic with information for SCR Amplitude, SCR Rise, and SCR Recovery.
        /// </returns>
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
            int indexPreviousMinimum = -1;
            int indexNextMinimum = -1;
            bool passInitialization = false;
            bool passInitializationRice = false;
            bool isRecoveryExist = false;

            for (int i = 0; i < (inflectionPoints.Count); i++)
            {
                if(inflectionPoints.ElementAt(i).ExtremaType.Equals(ExtremaType.Minimum))
                {
                    indexPreviousMinimum = i;
                }

                if (inflectionPoints.ElementAt(i).ExtremaType.Equals(ExtremaType.Maximum))
                {
                    //for rise statistics we have to find previous min. current maximum and next minimum
                    double x0 = (indexPreviousMinimum > -1) ? inflectionPoints.ElementAt(indexPreviousMinimum).CoordinateX : inflectionPoints.ElementAt(0).CoordinateX;
                    double x1 = inflectionPoints.ElementAt(i).CoordinateX;
                    double y1 = inflectionPoints.ElementAt(i).CoordinateY;

                    //searching for the next minimum
                    for(int j = (i + 1); j < inflectionPoints.Count; j++)
                    {
                        if(inflectionPoints.ElementAt(j).ExtremaType.Equals(ExtremaType.Minimum))
                        {
                            indexNextMinimum = j;
                            indexPreviousMinimum = indexNextMinimum;
                            isRecoveryExist = true;
                            break;
                        }
                    }

                    double x2 = (isRecoveryExist) ? inflectionPoints.ElementAt(indexNextMinimum).CoordinateX : 0.0;
                    isRecoveryExist = isRecoveryExist && (x2 - x1).CompareTo(0.1) > 0;

                    double currentAmplitude = y1;

                    double currentRise = x1 - x0;
                    double currentRecouvery = (isRecoveryExist) ? (x2 - x1) / 2 : 0.0;

                    if (!passInitialization)
                    {
                        scrAmplitude.Maximum = currentAmplitude;
                        scrAmplitude.Minimum = currentAmplitude;

                        if (isRecoveryExist && currentRecouvery.CompareTo(0.0) > 0)
                        {
                            scrRecovery.Maximum = currentRecouvery;
                            scrRecovery.Minimum = currentRecouvery;
                        }

                        passInitialization = true;
                    }
                    else 
                    {
                        scrAmplitude.Maximum = (scrAmplitude.Maximum.CompareTo(currentAmplitude) < 0) ? currentAmplitude : scrAmplitude.Maximum;
                        scrAmplitude.Minimum = (scrAmplitude.Minimum.CompareTo(currentAmplitude) > 0 && currentAmplitude.CompareTo(0.0) != 0) ? currentAmplitude : scrAmplitude.Minimum;

                        if (isRecoveryExist && currentRecouvery.CompareTo(0.0) > 0)
                        {
                            scrRecovery.Maximum = (scrRecovery.Maximum.CompareTo(currentRecouvery) < 0) ? currentRecouvery : scrRecovery.Maximum;
                            scrRecovery.Minimum = (scrRecovery.Minimum.CompareTo(currentRecouvery) > 0) ? currentRecouvery : scrRecovery.Minimum;
                        }
                    }

                    if (i > 0 && !passInitializationRice && (currentRise.CompareTo(0.1) > 0))
                    {
                        scrRise.Maximum = currentRise;
                        scrRise.Minimum = currentRise;

                        passInitializationRice = true;
                    }
                    else if (i > 0 && passInitializationRice && (currentRise.CompareTo(0.1) > 0))
                    {
                        scrRise.Maximum = (scrRise.Maximum.CompareTo(currentRise) < 0) ? currentRise : scrRise.Maximum;
                        scrRise.Minimum = (scrRise.Minimum.CompareTo(currentRise) > 0) ? currentRise : scrRise.Minimum;
                    }

                    if (isRecoveryExist && currentRecouvery.CompareTo(0.0) > 0)
                    {
                        allRecoveryTimes.Add(currentRecouvery);
                        sumRecoveryTime += currentRecouvery;
                    }

                    allMaximums.Add(currentAmplitude);
                    sumMaximums += currentAmplitude;

                    if ((i > 0) && (currentRise.CompareTo(0.1) > 0))
                    {

                        allRises.Add(currentRise);
                        sumRises += currentRise;
                    }

                    if(isRecoveryExist) i = indexNextMinimum;
                }

                isRecoveryExist = false;
            }

            scrAmplitude.Mean = (allMaximums != null && allMaximums.Count > 0) ? Convert.ToDecimal(sumMaximums / allMaximums.Count, CultureInfo.InvariantCulture) : 0;
            scrAmplitude.Count = allMaximums.Count / timeWindow ;
            scrAmplitude.StdDeviation = GetStandardDeviation(allMaximums, scrAmplitude.Mean);
            arousalStat.SCRAmplitude = scrAmplitude;

            scrRise.Mean = (allRises != null && allRises.Count > 0) ? Convert.ToDecimal(sumRises / allRises.Count, CultureInfo.InvariantCulture) : 0;
            scrRise.Count = allRises.Count / timeWindow ;
            scrRise.StdDeviation = GetStandardDeviation(allRises, scrRise.Mean);
            arousalStat.SCRRise = scrRise;

            scrRecovery.Mean = (allRecoveryTimes != null && allRecoveryTimes.Count > 0) ? Convert.ToDecimal(sumRecoveryTime / allRecoveryTimes.Count, CultureInfo.InvariantCulture) : 0;
            scrRecovery.Count = allRecoveryTimes.Count / timeWindow ;
            scrRecovery.StdDeviation = GetStandardDeviation(allRecoveryTimes, scrRecovery.Mean);
            arousalStat.SCRRecoveryTime = scrRecovery;

            return arousalStat;
        }

        /// <summary>
        /// Calculate standard deviation of a list of numbers 
        /// </summary>
        ///
        /// <param name="listOfNumbers"> List of numbers. </param>
        /// <param name="mean"> Mean of the list listOfNumbers. </param>
        ///
        /// <returns>
        /// Standard deviation.
        /// </returns>
        private decimal GetStandardDeviation(List<double> listOfNumbers, decimal mean)
        {
            double stdDeviation = 0;
            foreach(double currentNumber in listOfNumbers)
            {
                stdDeviation += Math.Pow((currentNumber - (double)mean), 2);
            }

            return (listOfNumbers.Count > 0) ? Convert.ToDecimal(Math.Sqrt(stdDeviation / listOfNumbers.Count), CultureInfo.InvariantCulture) : 0;
        }

        /// <summary>
        /// Define signal values that will be included in calculations of EDA features.
        /// </summary>
        ///
        /// <param name="signalValueInCache"> List with all signal value store in the cache. </param>
        /// <param name="numberOfAffectedPoints"> Number of affected signal values. </param>
        ///
        /// <returns>
        /// List of signal values that will be included in calculations of EDA features.
        /// </returns>
        public List<SignalDataByTime> AffectedCoordinatePoints(List<SignalDataByTime> signalValueInCache, int numberOfAffectedPoints)
        {
            List<SignalDataByTime> result = new List<SignalDataByTime>();

            for (int i = (signalValueInCache.Count - numberOfAffectedPoints); i < signalValueInCache.Count - 1; i++)
            {
                result.Add(signalValueInCache.ElementAt(i));
            }

            return result;
        }

        /// <summary>
        /// Convert an ArousalStatistics object in JSON string.
        /// </summary>
        ///
        /// <param name="statisticObject">The target arousal statistics</param>
        ///
        /// <returns>
        /// The arousal statistic in JSON format.
        /// </returns>
        public string GetJSONArousalStatistics(ArousalStatistics statisticObject)
        {
            
            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = js.Serialize(statisticObject);

            return json;
        }

        /// <summary>
        /// Calculate SCR features.
        /// </summary>
        /// 
        /// <returns>
        /// The arousal statistic for default time window.
        /// </returns>
        public ArousalStatistics GetArousalStatistics()
        {
            return GetArousalStatistics(defaultTimeWindow, TimeWindowMeasure.Seconds);
        }

        /// <summary>
        /// Calculate SCR features.
        /// </summary>
        /// 
        /// <param name="timeWindow">The time window.</param>
        /// <param name="timeScale">The time scale - it can be in milliseconds or in seconds.</param>
        /// 
        /// <returns>
        /// The arousal statistic according to the specified time window.
        /// </returns>
        public ArousalStatistics GetArousalStatistics(double timeWindow, TimeWindowMeasure timeScale)
        {
            signalValues = GetSignalValues();
            //The signal values receved by device are already filtered (we give the average of eight sequence values).
            //Therefore it is not needed to apply other filter.
            SignalDataByTime[] medianFilterCoordinates = GetMedianFilterPoints(signalValues);
            if (medianFilterCoordinates != null) return GetArousalStatisticsByMedianFilter(medianFilterCoordinates, timeWindow, timeScale);
            return GetArousalStatisticsByMedianFilter(signalValues, timeWindow, timeScale); ;
        }

        /// <summary>
        /// Calculate SCR features after aplying median filter on the signal values.
        /// </summary>
        /// 
        /// <param name="medianFilterCoordinates">List of new signal values after aplying median filter on raw signal values.</param>
        /// <param name="timeWindow">The time window.</param>
        /// <param name="timeMeasure">The time scale - it can be in milliseconds or in seconds.</param>
        /// 
        /// <returns>
        /// The arousal statistic according to the specified time window.
        /// </returns>
        public ArousalStatistics GetArousalStatisticsByMedianFilter(SignalDataByTime[] medianFilterCoordinates, double timeWindow, TimeWindowMeasure timeMeasure)
        {
            if(timeWindow.CompareTo(0.0) <= 0)
            {
                timeWindow = defaultTimeWindow;
                timeMeasure = TimeWindowMeasure.Seconds;
            }

            ArousalStatistics butterworthStatistics = GetArousalStatistics(medianFilterCoordinates.ToList(), timeWindow, timeMeasure, sampleRate);
            if (butterworthStatistics != null)
            {
                butterworthStatistics.TonicStatistics = GetTonicStatistics(medianFilterCoordinates.ToList());
                butterworthStatistics.SCLAchievedArousalLevel = GetTonicLevel(butterworthStatistics.TonicStatistics.MeanAmp);
                butterworthStatistics.LowPassSignalValue = medianFilterCoordinates[medianFilterCoordinates.Count() - 1].LowPassValue;
                butterworthStatistics.HighPassSignalValue = medianFilterCoordinates[medianFilterCoordinates.Count() - 1].HighPassValue;
                butterworthStatistics.LastMedianFilterValue = medianFilterCoordinates[medianFilterCoordinates.Count() - 1].SignalValue;
            }
            
            return butterworthStatistics;
        }

        /// <summary>
        /// Start of the calibration period.
        /// </summary>
        /// 
        /// <returns>
        /// Message to the socket client for starting of the calibration period.
        /// </returns>
        public string StartOfCalibrationPeriod()
        {
            calibrationTimer.Start();
            calibrationTimer.Enabled = true;

            return "The calibration process was started.";
        }

        /// <summary>
        /// Call the method GetArousalStatistics().
        /// </summary>
        private void CalibrationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            GetArousalStatistics();
        }

        /// <summary>
        /// End of the calibration period.
        /// </summary>
        /// 
        /// <returns>
        /// Message to the socket client for ending of the calibration period.
        /// </returns>
        public string EndOfCalibrationPeriod()
        {
            calibrationTimer.Stop();
            calibrationTimer.Enabled = false;

            SetCalibrationMinMaxStatistics();
            
            logger.Log(Severity.Information, "The calibration process was executed. The result is: \nminArousalArea: " + minArousalArea +
                                                                            "\nmaxArousalArea: " + maxArousalArea +
                                                                            "\nminTonicAmplitude: " + minTonicAmplitude +
                                                                            "\nmaxTonicAmplitude: " + maxTonicAmplitude +
                                                                            "\nminMovingAverage: " + minMovingAverage +
                                                                            "\nmaxMovingAverage: " + maxMovingAverage);

            return "The calibration process was executed.";
        }

        /// <summary>
        /// Set min/max {arousal area, tonic amplitude and general arousal} after the calibration period.
        /// </summary>
        /// 
        private void SetCalibrationMinMaxStatistics()
        {
            double minGSRDeviceSignalValue = settings.MinGSRDeviceSignalValue;
            double maxGSRDeviceSignalValue = settings.MaxGSRDeviceSignalValue;

            minArousalArea = 0.75 * minArousalArea;
            minTonicAmplitude = 0.75 * minTonicAmplitude;
            minMovingAverage = 0.75 * minMovingAverage;

            minMovingAverage = (minMovingAverage.CompareTo(minGSRDeviceSignalValue) > 0) ? minMovingAverage : minGSRDeviceSignalValue;

            maxArousalArea = 1.25 * maxArousalArea;
            maxTonicAmplitude = 1.25 * maxTonicAmplitude;
            maxMovingAverage = 1.25 * maxMovingAverage;

            maxMovingAverage = (maxMovingAverage.CompareTo(maxGSRDeviceSignalValue) < 0) ? maxMovingAverage : maxGSRDeviceSignalValue;
        }

        /// <summary>
        /// End of the measurement.
        /// </summary>
        /// 
        /// <returns>
        /// Message to the socket client for ending of the measurement.
        /// </returns>
        public string EndOfMeasurement()
        {
            logger.Log(Severity.Information, "The measurement process was ended. The result is: \nminArousalArea: " + minArousalArea +
                                                                         "\nmaxArousalArea: " + maxArousalArea +
                                                                         "\nminTonicAmplitude: " + minTonicAmplitude +
                                                                         "\nmaxTonicAmplitude: " + maxTonicAmplitude +
                                                                         "\nminMovingAverage: " + minMovingAverage +
                                                                         "\nmaxMovingAverage: " + maxMovingAverage);

            InitializeMinMaxStatistics();

            return "The measurement process was ended.";
        }

        /// <summary>
        /// Convert a string to double with precision to the 4-th digit.
        /// </summary>
        /// 
        /// <param name="value">String value.</param>
        /// <param name="medianCoordinatesValues">list of signal data.</param>
        /// 
        /// <returns>
        /// Double value of the string.
        /// </returns>
        private string RoundString(string value)
        {
            return Math.Round(Convert.ToDouble(value, CultureInfo.InvariantCulture), 4).ToString();
        }

        /// <summary>
        /// Set initial value of min/max {arousal area, tonic amplitude and general arousal}.
        /// </summary>
        /// 
        public void InitializeMinMaxStatistics()
        {
            minArousalArea = Double.NaN;
            maxArousalArea = Double.NaN;
            minTonicAmplitude = Double.NaN;
            maxTonicAmplitude = Double.NaN;
            minMovingAverage = Double.NaN;
            maxMovingAverage = Double.NaN;
        }
    }
}
