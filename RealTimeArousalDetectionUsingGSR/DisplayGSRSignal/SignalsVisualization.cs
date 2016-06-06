using Assets.Rage.GSRAsset.SignalProcessor;
using Assets.Rage.GSRAsset.SignalDevice;
using Assets.Rage.GSRAsset.SocketServer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Assets.Rage.GSRAsset.DisplayGSRSignal
{
    public partial class SignalsVisualization : Form
    {
        ISignalDeviceController signalController = new GSRHRDevice();
        GSRSignalProcessor gsrHandler = new GSRSignalProcessor();
        private SocketListener socketListener = new SocketListener();

        private System.Windows.Forms.Timer WorkingPeriodTimer;
        private System.Windows.Forms.Timer PausePeriodTimer;

        private string medianLineName = "De-noised filter";
        private string butterworthLowPassLine = "Tonic line";
        private string butterworthHighPassLine = "Phasic line";
        private double butterworthTonicPhasicFrequency = 0.05;

        int workPeriod;
        int pausePeriod;

        //bsAsset will provide common methods
        //BaseAsset bsAsset = new BaseAsset();

        public SignalsVisualization()
        {
            InitializeComponent();

            signalController.SelectCOMPort("COM3");
            signalController.OpenPort();
            //signalController.SetSignalSamplerate();
            //signalController.StartSignalsRecord();

            workPeriod = 4 * 1000;
            pausePeriod = 1 * 1000;

            this.WorkingPeriodTimer = new System.Windows.Forms.Timer(this.components);
            WorkingPeriodTimer.Tick += pause_Working_Event;
            WorkingPeriodTimer.Interval = workPeriod;

            this.PausePeriodTimer = new System.Windows.Forms.Timer(this.components);
            PausePeriodTimer.Tick += start_working_Event;
            PausePeriodTimer.Interval = pausePeriod;

            WorkingPeriodTimer.Start();

            if (DoesChartDisplay()) GSRChartDisplay();
            timer1.Tick += timer1_Tick;
        }


        private static bool DoesChartDisplay()
        {
            //return false;// Cache.GetAllForChannel(0) != null && Cache.GetAllForChannel(0).Count > 0;
            return Cache.GetAllForChannel(0) != null && Cache.GetAllForChannel(0).Count > 0;
        }

        //Handle click on the Refresh button
        private void refresh_Click(object sender, EventArgs e)
        {
            //RecordPauseDeviceSignal();
            
            timer1.Start();
            if (DoesChartDisplay())  GSRChartDisplay();
        }

        private void GSRChartDisplay()
        {
            int p = Cache.GetAllForChannel(0) != null ? Cache.GetAllForChannel(0).Count : 0;

            ChartClean();

            ChartArea chart = gsrChart.ChartAreas[0];
            ChartArea filterChart = butterworthChart.ChartAreas[0];
            int sampleRate = signalController.GetSignalSampleRate();

            //Logger.Log("start2: " + (DateTime.Now - DateTime.MinValue).TotalMilliseconds);
            Dictionary<int, List<double>> channelsValues = gsrHandler.ExtractChannelsValues();
            //Logger.Log("channelsValues: " + channelsValues.ElementAt(0).Value.Count);
            //Dictionary<int, List<double>> channelsValues = gsrHandler.ExtractChannelsValuesFromFile();
            gsrHandler.FillCoordinates(sampleRate);
            //Logger.Log("count: " + gsrHandler.coordinates.Count);
            Dictionary<int, Dictionary<double, double>> medianFilterCoordinates = gsrHandler.GetMedianFilterPoints(gsrHandler.coordinates);
            FilterButterworth highPassFilter = new FilterButterworth(butterworthTonicPhasicFrequency, sampleRate, ButterworthPassType.Highpass);
            FilterButterworth lowPassFilter = new FilterButterworth(butterworthTonicPhasicFrequency, sampleRate, ButterworthPassType.Lowpass);

            double xMaxValue = 0.0;
            double xMinValue = 0.0;
            double yMaxValue = 0.0;
            double yMinValue = 0.0;

            double yFilterMaxValue = 0.0;
            double yFilterMinValue = 0.0;

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
                    SetMinMax(ref xMaxValue, ref xMinValue, coordinate.Key, null, null);
                    SetMinMax(ref yMaxValue, ref yMinValue, coordinate.Value, MinYTxtBox.Text, MaxYTxtBox.Text);

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

                        SetMinMax(ref yFilterMaxValue, ref yFilterMinValue, highPassValue, null, null);
                        SetMinMax(ref yFilterMaxValue, ref yFilterMinValue, lowPassValue, MinYFltTxtBox.Text, MaxYFltTxtBox.Text);

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
            // }

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

            if(butterworthChart.Series[butterworthHighPassLine].Points.Count > maxNumberOfPoints)
            {
                int exceedPoints = butterworthChart.Series[butterworthHighPassLine].Points.Count - maxNumberOfPoints;
                for (int i = 0; i < exceedPoints; i++)
                {
                    butterworthChart.Series[butterworthHighPassLine].Points.RemoveAt(0);
                }
            }

            if(butterworthChart.Series[butterworthLowPassLine].Points.Count > maxNumberOfPoints)
            {
                int exceedPoints = butterworthChart.Series[butterworthLowPassLine].Points.Count - maxNumberOfPoints;
                for (int i = 0; i < exceedPoints; i++)
                {
                    butterworthChart.Series[butterworthLowPassLine].Points.RemoveAt(0);
                }
            }

            if(gsrChart.Series[medianLineName].Points.Count > maxNumberOfPoints)
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
                foreach(InflectionPoint currentPoint in inflectionPoints)
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
            foreach(DataPoint currentPoint in chartAreaPoints)
            {
                result.Add(new InflectionPoint(currentPoint.XValue, currentPoint.YValues[0], i));
                i++;
            }

            return result;
        }

        private static Dictionary<double, double> TransformToDictionary(DataPointCollection chartAreaPoints)
        {
            Dictionary<double, double> result = new Dictionary<double, double>();
            foreach(DataPoint currentPoint in chartAreaPoints)
            {
                result.Add(currentPoint.XValue, currentPoint.YValues[0]);
            }

            return result.OrderBy(key=>key.Key).ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);
        }

        private static void MarkInflectedPoint(DataPointCollection chartPoints, int i, Chart chart, String seriesName)
        {
            chartPoints[i].MarkerStyle = MarkerStyle.Circle;
            chartPoints[i].MarkerColor = Color.Blue;
            chartPoints[i].MarkerSize = 5;            
        }

        private static void SetMinMax(ref double xMaxValue, ref double xMinValue, double coordinateKeyValue, String requiredMin, String requiredMax)
        {
            if(!String.IsNullOrEmpty(requiredMin) && !String.IsNullOrEmpty(requiredMax))
            {
                xMinValue = Convert.ToDouble(requiredMin);
                xMaxValue = Convert.ToDouble(requiredMax);
            }
            else if (xMaxValue.Equals(0.0) && xMinValue.Equals(0.0))
            {
                xMaxValue = coordinateKeyValue;
                xMinValue = coordinateKeyValue;
            }
            else
            {
                if (xMaxValue.CompareTo(coordinateKeyValue) < 0)
                {
                    xMaxValue = coordinateKeyValue;
                }

                if (xMinValue.CompareTo(coordinateKeyValue) > 0)
                {
                    xMinValue = coordinateKeyValue;
                }
            }
        }

        private static void SetChartDetail(ChartArea chart, double maxValue, double minValue, double yMaxValue, double yMinValue)
        {
            chart.AxisX.Title = "Time";
            chart.AxisY.Title = "GSR";

            // Set maximum values.
            chart.AxisX.Maximum = maxValue;

            chart.AxisY.Maximum = yMaxValue;
            chart.AxisY.Minimum = yMinValue;

            // enable autoscroll
            chart.CursorX.AutoScroll = true;

            //zoom to [minValue, minvalue+numberOfVisiblePoints]
            //chart.AxisX.ScaleView.Zoomable = true;
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
            if(DoesChartDisplay()) GSRChartDisplay();
        }

        private void pause_Working_Event(object sender, EventArgs e)
        {
            PausePeriodTimer.Start();
            WorkingPeriodTimer.Stop();
            signalController.StopSignalsRecord();
        }

        private void start_working_Event(object sender, EventArgs e)
        {
            WorkingPeriodTimer.Start();
            PausePeriodTimer.Stop();
            signalController.SelectCOMPort("COM3");
            //signalController.OpenPort();
            signalController.StartSignalsRecord();
        }

        //Handle click on the Stop button
        private void stop_Click(object sender, EventArgs e)
        {
            //signalController.StopSignalsRecord();

            //GSRSignalProcessor gsrHandler = new GSRSignalProcessor();
            
            //ArousalStatistics gsrArousalStatistics = GetInflectionPoints(gsrChart, gsrChart.Series[medianLineName].Points, medianLineName, true);
            //ArousalInfo.Text = (gsrArousalStatistics != null) ? gsrArousalStatistics.ToString("De-noised filter") : "";
            /*
            ArousalStatistics butterworthStatistics = GetInflectionPoints(butterworthChart, butterworthChart.Series[butterworthHighPassLine].Points, butterworthHighPassLine, true);
            if (butterworthStatistics != null) butterworthStatistics.TonicStatistics = gsrHandler.GetTonicStatistics(TransformToDictionary(butterworthChart.Series[butterworthLowPassLine].Points));

            JavaScriptSerializer js = new JavaScriptSerializer();
            string json = js.Serialize(butterworthStatistics);
            */
            ArousalInfoButterworth.Text = gsrHandler.GetJSONArousalStatistics( gsrHandler.GetArousalStatisticsByMedianFilter(TransformToDictionary(gsrChart.Series[medianLineName].Points), -1.0, TimeWindowMeasure.Seconds) );

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
            //gsrChart.ChartAreas[0].AxisY.Minimum = newYMin;
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
