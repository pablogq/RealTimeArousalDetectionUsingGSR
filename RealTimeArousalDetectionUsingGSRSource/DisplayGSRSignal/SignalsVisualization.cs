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

using Assets.Rage.GSRAsset.SignalProcessor;
using Assets.Rage.GSRAsset.SignalDevice;
using Assets.Rage.GSRAsset.SocketServer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Configuration;

namespace Assets.Rage.GSRAsset.DisplayGSRSignal
{
    public partial class SignalsVisualization : Form
    {
        ISignalDeviceController signalController = new GSRHRDevice();
        GSRSignalProcessor gsrHandler = new GSRSignalProcessor();
        private SocketListener socketListener = new SocketListener();

        private string medianLineName = "De-noised filter";
        private string butterworthLowPassLine = "Tonic line";
        private string butterworthHighPassLine = "Phasic line";
        private double butterworthTonicPhasicFrequency = 0.05;
        private double xMaxValue = 0.0;
        private double yMaxValue = 0.0;
        private double yFilterMaxValue = 0.0;
        private double xMinValue = 0.0;
        private double yMinValue = 0.0;
        private double yFilterMinValue = 0.0;

        public SignalsVisualization()
        {
            InitializeComponent();

            if (DoesChartDisplay()) GSRChartDisplay();
            timer1.Tick += timer1_Tick;
        }


        private static bool DoesChartDisplay()
        {
            List<double> channelData = CacheSignalData.GetAllForChannel(0);
            //return false;// Cache.GetAllForChannel(0) != null && Cache.GetAllForChannel(0).Count > 0;
            return channelData != null && channelData.Count > 0;
        }

        //Handle click on the Refresh button
        private void refresh_Click(object sender, EventArgs e)
        {
            //RecordPauseDeviceSignal();

            if (!"TestWithoutDevice".Equals(ConfigurationManager.AppSettings.Get("ApplicationMode")))
            {
                signalController.SelectCOMPort(ConfigurationManager.AppSettings.Get("COMPort"));
                signalController.OpenPort();
                signalController.StartSignalsRecord();
            }

            Logger.Log("Sample rate is: " + signalController.GetSignalSampleRateByConfig());

            timer1.Start();
            if (DoesChartDisplay()) GSRChartDisplay();

            // timer1.Tick += timer1_Tick;
        }

        private void GSRChartDisplay()
        {
            int p = CacheSignalData.GetAllForChannel(0) != null ? CacheSignalData.GetAllForChannel(0).Count : 0;
            Logger.Log(DateTime.Now.Millisecond + ", " + p);

            ChartClean();

            ChartArea chart = gsrChart.ChartAreas[0];
            ChartArea filterChart = butterworthChart.ChartAreas[0];
            int sampleRate = signalController.GetSignalSampleRate();

            Dictionary<int, List<double>> channelsValues = gsrHandler.ExtractChannelsValues();
            gsrHandler.FillCoordinates(sampleRate);
            Dictionary<int, Dictionary<double, double>> medianFilterCoordinates = gsrHandler.GetMedianFilterPoints(gsrHandler.coordinates);
            FilterButterworth highPassFilter = new FilterButterworth(butterworthTonicPhasicFrequency, sampleRate, ButterworthPassType.Highpass);
            FilterButterworth lowPassFilter = new FilterButterworth(butterworthTonicPhasicFrequency, sampleRate, ButterworthPassType.Lowpass);

            xMaxValue = 0.0;
            xMinValue = 0.0;
            yMaxValue = 0.0;
            yMinValue = 0.0;

            yFilterMaxValue = 0.0;
            yFilterMinValue = 0.0;

            // Adjust Y & X axis scale
            gsrChart.ResetAutoValues();

            int numPoints = 0;
            foreach (KeyValuePair<int, Dictionary<double, double>> channelCoordinates in gsrHandler.coordinates)
            {
                string currentChannel = "GSR " + (channelCoordinates.Key + 1).ToString();

                if (channelCoordinates.Value == null && channelCoordinates.Value.Count == 0)
                {
                    gsrChart.Series[currentChannel].IsVisibleInLegend = false;
                }

                numPoints = channelsValues.Values.Count;
                Dictionary<double, double> coordinatesValues = SortDictionaryByKey(channelCoordinates.Value);
                foreach (KeyValuePair<double, double> coordinate in coordinatesValues)
                {
                    SetMinMax(coordinate.Key, null, null, "x");
                    SetMinMax(coordinate.Value, MinYTxtBox.Text, MaxYTxtBox.Text, "y");

                    gsrChart.Series[currentChannel].Points.AddXY(coordinate.Key, coordinate.Value);
                }

                if (medianFilterCoordinates != null)
                {
                    Dictionary<double, double> medianCoordinatesValues = medianFilterCoordinates.Values.ElementAt(channelCoordinates.Key);
                    foreach (KeyValuePair<double, double> medianCoordinate in medianCoordinatesValues)
                    {

                        double medianValue = medianCoordinate.Value;

                        double highPassValue = highPassFilter.GetFilterValue(medianValue);
                        double lowPassValue = lowPassFilter.GetFilterValue(medianValue);

                        SetMinMax(highPassValue, null, null, "xFilter");
                        SetMinMax(lowPassValue, MinYFltTxtBox.Text, MaxYFltTxtBox.Text, "yFilter");

                        gsrChart.Series[medianLineName].Points.AddXY(medianCoordinate.Key, medianValue);
                        butterworthChart.Series[butterworthLowPassLine].Points.AddXY(medianCoordinate.Key, lowPassValue);
                        butterworthChart.Series[butterworthHighPassLine].Points.AddXY(medianCoordinate.Key, highPassValue);
                    }
                }
            }

            /*
            ArousalStatistics gsrArousalStatistics = GetInflectionPoints(gsrChart, gsrChart.Series[medianLineName].Points, medianLineName, true);
            ArousalInfo.Text = (gsrArousalStatistics != null) ? gsrArousalStatistics.ToString("De-noised filter") : "";
            ArousalStatistics butterworthStatistics = GetInflectionPoints(butterworthChart, butterworthChart.Series[butterworthHighPassLine].Points, butterworthHighPassLine, true);
            if(butterworthStatistics != null) butterworthStatistics.TonicStatistics = gsrHandler.GetTonicStatistics(TransformToDictionary(butterworthChart.Series[butterworthLowPassLine].Points));

            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = js.Serialize(butterworthStatistics);

            Logger.Log("jsonObject: " + json);

            ArousalInfoButterworth.Text = (butterworthStatistics != null) ? butterworthStatistics.ToString("Phasic line") : "";
            */

            //remove old points
            //RemoveOldPoints(2500);

            if (gsrHandler.coordinates.Count > 5) gsrChart.DataManipulator.FinancialFormula(FinancialFormula.ExponentialMovingAverage, "5", "GSR 1:Y", "Moving Average:Y");

            //if(number == 1)
            //{
            SetChartDetail(chart, xMaxValue, xMinValue, yMaxValue, yMinValue);
            SetChartDetail(filterChart, xMaxValue, xMinValue, yFilterMaxValue, yFilterMinValue);
            //}

            //gsrChart.Invalidate();
            //butterworthChart.Invalidate();
        }

        private static Dictionary<double, double> SortDictionaryByKey(Dictionary<double, double> dictionary)
        {
            return dictionary.OrderBy(key => key.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
        }

        private void RemoveOldPoints(int maxNumberOfPoints)
        {
            if (gsrChart.Series[0].Points.Count > maxNumberOfPoints)
            {
                int exceedPoints = gsrChart.Series[0].Points.Count - maxNumberOfPoints;
                for (int i = 0; i < exceedPoints; i++)
                {
                    gsrChart.Series[0].Points.RemoveAt(0);
                }
            }

            if (butterworthChart.Series[butterworthHighPassLine].Points.Count > maxNumberOfPoints)
            {
                int exceedPoints = butterworthChart.Series[butterworthHighPassLine].Points.Count - maxNumberOfPoints;
                for (int i = 0; i < exceedPoints; i++)
                {
                    butterworthChart.Series[butterworthHighPassLine].Points.RemoveAt(0);
                }
            }

            if (butterworthChart.Series[butterworthLowPassLine].Points.Count > maxNumberOfPoints)
            {
                int exceedPoints = butterworthChart.Series[butterworthLowPassLine].Points.Count - maxNumberOfPoints;
                for (int i = 0; i < exceedPoints; i++)
                {
                    butterworthChart.Series[butterworthLowPassLine].Points.RemoveAt(0);
                }
            }

            if (gsrChart.Series[medianLineName].Points.Count > maxNumberOfPoints)
            {
                int exceedPoints = gsrChart.Series[medianLineName].Points.Count - maxNumberOfPoints;
                for (int i = 0; i < exceedPoints; i++)
                {
                    gsrChart.Series[medianLineName].Points.RemoveAt(0);
                }
            }
        }

        private ArousalStatistics GetInflectionPoints(Chart chart, DataPointCollection chartAreaPoints, String seriesName, bool flagMarked)
        {
            GSRSignalProcessor gsrHandler = new GSRSignalProcessor();
            List<InflectionPoint> chartPointsCoordinates = TransformToCoordinatePoints(chartAreaPoints);
            InflectionLine inflLineHandler = new InflectionLine();
            List<InflectionPoint> inflectionPoints = inflLineHandler.GetInflectionPoints(chartPointsCoordinates);

            if (flagMarked)
            {
                foreach (InflectionPoint currentPoint in inflectionPoints)
                {
                    MarkInflectedPoint(chartAreaPoints, currentPoint.IndexOrigin, chart, seriesName);
                }
            }

            return gsrHandler.GetArousalStatistics(TransformToDictionary(chartAreaPoints));
        }

        private static List<InflectionPoint> TransformToCoordinatePoints(DataPointCollection chartAreaPoints)
        {
            List<InflectionPoint> result = new List<InflectionPoint>();
            int i = 0;
            foreach (DataPoint currentPoint in chartAreaPoints)
            {
                result.Add(new InflectionPoint(currentPoint.XValue, currentPoint.YValues[0], i));
                i++;
            }

            return result;
        }

        private static Dictionary<double, double> TransformToDictionary(DataPointCollection chartAreaPoints)
        {
            Dictionary<double, double> result = new Dictionary<double, double>();
            foreach (DataPoint currentPoint in chartAreaPoints)
            {
                result.Add(currentPoint.XValue, currentPoint.YValues[0]);
            }

            return result.OrderBy(key => key.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
        }

        private static void MarkInflectedPoint(DataPointCollection chartPoints, int i, Chart chart, String seriesName)
        {
            chartPoints[i].MarkerStyle = MarkerStyle.Circle;
            chartPoints[i].MarkerColor = Color.Blue;
            chartPoints[i].MarkerSize = 5;
        }

        private void SetMinMax(double coordinateKeyValue, String requiredMin, String requiredMax, String target)
        {
            double xxMaxValue = 0.0;
            double xxMinValue = 0.0;

            if ("x".Equals(target))
            {
                xxMaxValue = xMaxValue;
                xxMinValue = xMinValue;
            }
            else if ("y".Equals(target))
            {
                xxMaxValue = yMaxValue;
                xxMinValue = yMinValue;
            }
            else if ("xFilter".Equals(target))
            {
                xxMaxValue = yFilterMaxValue;
                xxMinValue = yFilterMinValue;
            }
            else if ("yFilter".Equals(target))
            {
                xxMaxValue = yFilterMaxValue;
                xxMinValue = yFilterMinValue;
            }

            if (!String.IsNullOrEmpty(requiredMin) && !String.IsNullOrEmpty(requiredMax))
            {
                xxMinValue = Convert.ToDouble(requiredMin);
                xxMaxValue = Convert.ToDouble(requiredMax);
            }
            else if (xxMaxValue.Equals(0.0) && xxMinValue.Equals(0.0))
            {
                xxMaxValue = coordinateKeyValue;
                xxMinValue = coordinateKeyValue;
            }
            else
            {
                if (xxMaxValue.CompareTo(coordinateKeyValue) < 0)
                {
                    xxMaxValue = coordinateKeyValue;
                }


                if (xxMinValue.CompareTo(coordinateKeyValue) > 0)
                {
                    xxMinValue = coordinateKeyValue;
                }
            }

            if ("x".Equals(target))
            {
                xMaxValue = xxMaxValue;
                xMinValue = xxMinValue;
            }
            else if ("y".Equals(target))
            {
                yMaxValue = xxMaxValue;
                yMinValue = xxMinValue;
            }
            else if ("xFilter".Equals(target))
            {
                yFilterMaxValue = xxMaxValue;
                yFilterMinValue = xxMinValue;
            }
            else if ("yFilter".Equals(target))
            {
                yFilterMaxValue = xxMaxValue;
                yFilterMinValue = xxMinValue;
            }
        }

        private static void SetChartDetail(ChartArea chart, double maxValue, double minValue, double yMaxValue, double yMinValue)
        {
            chart.AxisX.Title = "Time";
            chart.AxisY.Title = "GSR";

            // Set maximum values.
            chart.AxisX.Maximum = maxValue;

            if (yMaxValue - yMinValue < 0.00005)
            {
                yMinValue -= 0.00005;
            }

            chart.AxisY.Maximum = yMaxValue;
            chart.AxisY.Minimum = yMinValue;

            // enable autoscroll
            chart.CursorX.AutoScroll = true;

            //zoom to [minValue, minvalue+numberOfVisiblePoints]
            chart.AxisX.ScaleView.Zoomable = true;
            chart.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;

            if (maxValue.CompareTo((double)500.0) < 0)
            {
                chart.AxisX.ScaleView.Zoom(minValue, maxValue);
            }
            else
            {
                chart.AxisX.ScaleView.Zoom(maxValue - 500, maxValue);
            }

            chart.AxisX.ScrollBar.IsPositionedInside = true;

            // disable zoom-reset button (only scrollbar's arrows are available)
            chart.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;

            // set scrollbar small change 
            if (maxValue.CompareTo((double)500.0) < 0)
            {
                chart.AxisX.ScaleView.SmallScrollSize = 10;

            }
            else
            {
                chart.AxisX.ScaleView.SmallScrollSize = 500;
            }
        }

        private void ChartClean()
        {
            foreach (var series in gsrChart.Series)
            {
                series.Points.Clear();
            }

            foreach (var series in butterworthChart.Series)
            {
                series.Points.Clear();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (DoesChartDisplay()) GSRChartDisplay();
        }

        //Handle click on the Stop button
        private void stop_Click(object sender, EventArgs e)
        {
            signalController.StopSignalsRecord();

            //GSRSignalProcessor gsrHandler = new GSRSignalProcessor();

            //ArousalInfoButterworth.Text = gsrHandler.GetJSONArousalStatistics( gsrHandler.GetArousalStatisticsByMedianFilter(TransformToDictionary(gsrChart.Series[medianLineName].Points), -1.0, TimeWindowMeasure.Seconds) );

            //ArousalInfoButterworth.Text = (butterworthStatistics != null) ? butterworthStatistics.ToString("Phasic line") : "";

            //gsrHandler.EndMeasurment();
            //timer1.Stop();
        }


        private void MinYBtn_Click(object sender, EventArgs e)
        {
            double newYMin;
            if (Double.TryParse(MinYTxtBox.Text, out newYMin) && newYMin.CompareTo(gsrChart.ChartAreas[0].AxisY.Maximum) < 0)
            {
                gsrChart.ChartAreas[0].AxisY.Minimum = newYMin;
            }
            gsrChart.ChartAreas[0].AxisY.Minimum = newYMin;
        }

        private void MaxYBtn_Click(object sender, EventArgs e)
        {
            double newYMax;
            if (Double.TryParse(MaxYTxtBox.Text, out newYMax) && newYMax.CompareTo(gsrChart.ChartAreas[0].AxisY.Minimum) > 0)
            {
                gsrChart.ChartAreas[0].AxisY.Maximum = newYMax;
            }
            //gsrChart.ChartAreas[0].AxisY.Maximum = newYMax;
        }

        Point? prevPosition = null;
        Point? clickPosition = null;
        ToolTip tooltip = new ToolTip();

        void gsrChart_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.Location;
            if (prevPosition.HasValue && pos == prevPosition.Value)
                return;
            tooltip.RemoveAll();
            prevPosition = pos;
            var results = gsrChart.HitTest(pos.X, pos.Y, true,
                                         ChartElementType.DataPoint);
            foreach (var result in results)
            {
                if (result.Series != null)
                {
                    var xVal = result.ChartArea.AxisX.PixelPositionToValue(pos.X);
                    var yVal = result.ChartArea.AxisY.PixelPositionToValue(pos.Y);

                    tooltip.Show("X=" + xVal + ", Y=" + yVal,
                                 this.gsrChart, e.Location.X, e.Location.Y);
                }
            }
        }

        private void butterworthChart_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.Location;
            if (prevPosition.HasValue && pos == prevPosition.Value)
                return;
            tooltip.RemoveAll();
            prevPosition = pos;
            var results = butterworthChart.HitTest(pos.X, pos.Y, true,
                                         ChartElementType.DataPoint);
            foreach (var result in results)
            {
                if (result.Series != null)
                {
                    var xVal = result.ChartArea.AxisX.PixelPositionToValue(pos.X);
                    var yVal = result.ChartArea.AxisY.PixelPositionToValue(pos.Y);

                    tooltip.Show("X=" + xVal + ", Y=" + yVal,
                                 this.butterworthChart, e.Location.X, e.Location.Y);
                }
            }
        }

        void gsrChart_MouseClick(object sender, MouseEventArgs e)
        {
            var pos = e.Location;
            clickPosition = pos;
            var results = gsrChart.HitTest(pos.X, pos.Y, true,
                                         ChartElementType.PlottingArea);
            foreach (var result in results)
            {
                if (result.ChartElementType == ChartElementType.PlottingArea)
                {
                    var xVal = result.ChartArea.AxisX.PixelPositionToValue(pos.X);
                    var yVal = result.ChartArea.AxisY.PixelPositionToValue(pos.Y);

                    tooltip.Show("X=" + xVal + ", Y=" + yVal,
                                 this.gsrChart, e.Location.X, e.Location.Y - 15);
                }
            }
        }

        private void CheckDenoisedFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (checkDenoisedFilter.Checked) gsrChart.Series[medianLineName].Enabled = true;
            else gsrChart.Series[medianLineName].Enabled = false;
        }

        private void MinYFltBtn_Click(object sender, EventArgs e)
        {
            double newYFltMin;
            if (Double.TryParse(MinYFltTxtBox.Text, out newYFltMin) && newYFltMin.CompareTo(butterworthChart.ChartAreas[0].AxisY.Maximum) < 0)
            {
                butterworthChart.ChartAreas[0].AxisY.Minimum = newYFltMin;
            }
        }

        private void MaxYFltBtn_Click(object sender, EventArgs e)
        {
            double newYFltMax;
            if (Double.TryParse(MaxYFltTxtBox.Text, out newYFltMax) && newYFltMax.CompareTo(butterworthChart.ChartAreas[0].AxisY.Minimum) > 0)
            {
                butterworthChart.ChartAreas[0].AxisY.Maximum = newYFltMax;
            }
        }

        private void SignalVisualization_Closed(object sender, EventArgs e)
        {
            if (signalController != null)
            {
                signalController.StopSignalsRecord();
            }

            if (socketListener != null)
            {
                socketListener.CloseSocket();
            }
        }

        private void btnStartSocket_Click(object sender, EventArgs e)
        {
            socketListener.Start();
            btnStartSocket.Visible = false;
        }
    }
}
