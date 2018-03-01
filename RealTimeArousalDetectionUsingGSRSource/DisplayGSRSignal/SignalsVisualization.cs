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

using Assets.Rage.RealTimeArousalDetectionUsingGSRAsset.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;
using AssetManagerPackage;
using AssetPackage;

namespace Assets.Rage.RealTimeArousalDetectionUsingGSRAsset.DisplayGSRSignal
{
    public partial class SignalsVisualization : Form
    {
        private const int maximumNumberOfChartPoints = 200;
        private string rawSignalLineName = "Raw signal";
        private string medianLineName = "Denoised signal";
        private string butterworthLowPassLine = "Tonic line";
        private string butterworthHighPassLine = "Phasic line";

        private List<double> positionsX = new List<double>();
        private List<double> positionsY = new List<double>();
        private List<double> butterworthPositionsX = new List<double>();
        private List<double> butterworthPositionsY = new List<double>();
        private RealTimeArousalDetectionUsingGSRAsset gsrAsset;
        private RealTimeArousalDetectionAssetSettings settings;
        private ILog logger;

        public SignalsVisualization()
        {
            AssetManager.Instance.Bridge = new Bridge();
            gsrAsset = RealTimeArousalDetectionUsingGSRAsset.Instance;
            settings = (RealTimeArousalDetectionAssetSettings)gsrAsset.Settings;
            logger = (ILog)AssetManager.Instance.Bridge;

            if ("BackgroundMode".Equals(settings.FormMode))
            {
                this.WindowState = FormWindowState.Minimized;
                Load += new EventHandler(Form_Load);
            }
            else
            {
                InitializeComponent();
                lblErrors.Text = "";

                StartGSRDevice();
                if (DoesChartDisplay())
                {
                    GSRChartDisplay();
                    timer1.Tick += timer1_Tick;
                }

                StartSocket();
                lblSampleRateValue.Text = !"TestWithoutDevice".Equals(settings.ApplicationMode) ? gsrAsset.GetSignalSampleRate().ToString() : "0";
                lblSocketAddressValue.Text = settings.SocketIPAddress + ":" + settings.SocketPort;
                lblTimeWindowValue.Text = settings.DefaultTimeWindow + " s";
                btnSocketLight.BackColor = Color.GreenYellow;

                this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.gsrChart_MouseWheel);
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            StartGSRDevice();
            StartSocket();

            this.Hide();
        }

        private void gsrChart_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0)
                {
                    positionsX.RemoveAt(positionsX.Count - 1);
                    positionsX.RemoveAt(positionsX.Count - 1);

                    if (positionsX.Count < 2)
                    {
                        gsrChart.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                        butterworthChart.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    }
                    else
                    {
                        gsrChart.ChartAreas[0].AxisX.ScaleView.Zoom(positionsX.ElementAt(positionsX.Count - 2), positionsX.ElementAt(positionsX.Count - 1));
                        butterworthChart.ChartAreas[0].AxisX.ScaleView.Zoom(positionsX.ElementAt(positionsX.Count - 2), positionsX.ElementAt(positionsX.Count - 1));
                    }

                    positionsY.RemoveAt(positionsY.Count - 1);
                    positionsY.RemoveAt(positionsY.Count - 1);

                    if (positionsY.Count < 2) gsrChart.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    else gsrChart.ChartAreas[0].AxisY.ScaleView.Zoom(positionsY.ElementAt(positionsY.Count - 2), positionsY.ElementAt(positionsY.Count - 1));
                }

                if (e.Delta > 0)
                {
                    double xMin = gsrChart.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = gsrChart.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                    double yMin = gsrChart.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                    double yMax = gsrChart.ChartAreas[0].AxisY.ScaleView.ViewMaximum;

                    double posXStart = gsrChart.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    double posXFinish = gsrChart.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;

                    positionsX.Add(posXStart);
                    positionsX.Add(posXFinish);

                    double posYStart = gsrChart.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    double posYFinish = gsrChart.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;

                    positionsY.Add(posYStart);
                    positionsY.Add(posYFinish);

                    gsrChart.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    butterworthChart.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    gsrChart.ChartAreas[0].AxisY.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch (Exception exc)
            {
                logger.Log(Severity.Error, exc.ToString());
            }
        }

        void StartGSRDevice()
        {
            string lblErrorString = (!"BackgroundMode".Equals(settings.FormMode)) ? lblErrors.Text.ToString() : "";
            ErrorStartSignalDevice startingResult = gsrAsset.StartGSRDevice(lblErrorString);
            if(startingResult.Exception == null)
            {
                if (!"BackgroundMode".Equals(settings.FormMode)) lblErrors.Text = startingResult.ErrorContent;
            }
            else if (startingResult.Exception != null)
            {
                if (!"BackgroundMode".Equals(settings.FormMode)) lblErrors.Text = startingResult.ErrorContent;
                logger.Log(Severity.Error, PrintException(startingResult.Exception));
            }
        }

        private static string PrintException(Exception e)
        {
            return e != null ? e.ToString() : "";
        }

        private static bool DoesChartDisplay()
        {
            List<SignalDataByTime> channelData = CacheSignalData.GetCacheData().ToList();

            return channelData != null && channelData.Count > 0;
        }

        //Handle click on the Refresh button
        private void refresh_Click(object sender, EventArgs e)
        {
            if (!"TestWithoutDevice".Equals(settings.ApplicationMode))
            {
                StartGSRDevice();
            }

            if (DoesChartDisplay())
            {
                timer1.Start();
                GSRChartDisplay();
            }

        }

        private void GSRChartDisplay()
        {
            ChartClean();

            gsrChart.Series[0].XValueType = ChartValueType.DateTime;
            butterworthChart.Series[0].XValueType = ChartValueType.DateTime;

            List<SignalDataByTime> signalValues = gsrAsset.GetSignalData();
            SignalDataByTime[] signalValuesCopy = new SignalDataByTime[signalValues.Count];
            signalValues.CopyTo(signalValuesCopy);
            SignalDataByTime[] medianFilterCoordinates = gsrAsset.GetMedianFilterValues(signalValuesCopy);

            // Adjust Y & X axis scale
            gsrChart.ResetAutoValues();

            int i = 0;
            foreach (SignalDataByTime coordinate in signalValues)
            {
                DateTime coordinateDateTime = DateFromMS((long)coordinate.Time);
                String xAxisLabel = coordinateDateTime.ToString("HH:mm:ss:fff", CultureInfo.InvariantCulture);

                gsrChart.Series[rawSignalLineName].Points.AddXY(xAxisLabel, coordinate.SignalValue);

                if (medianFilterCoordinates != null)
                {
                    DateTime medianCoordinateDateTime = new DateTime((long)medianFilterCoordinates[i].Time * 1000, DateTimeKind.Local);
                    String medianXAxisLabel = medianCoordinateDateTime.ToString("HH:mm:ss:fff", CultureInfo.InvariantCulture);
                    double medianValue = medianFilterCoordinates[i].SignalValue;
                    gsrChart.Series[medianLineName].Points.AddXY(medianXAxisLabel, medianValue);
                    i++;
                }

                butterworthChart.Series[butterworthLowPassLine].Points.AddXY(xAxisLabel, coordinate.LowPassValue);
                butterworthChart.Series[butterworthHighPassLine].Points.AddXY(xAxisLabel, coordinate.HighPassValue);
            }

            RemoveOldPoints(maximumNumberOfChartPoints*5);

            SetChartDetail(gsrChart, "Time", "GSR");
            SetChartDetail(butterworthChart, "Time", "Butterworth");

        }

        private DateTime DateFromMS(long milliSec)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            TimeSpan time = TimeSpan.FromMilliseconds(milliSec);
            return startTime.Add(time).ToLocalTime();
        }

        private static void SetChartDetail(Chart chart, string titleXAxis, string titleYAxis)
        {
            chart.ChartAreas[0].AxisX.Title = titleXAxis;
            chart.ChartAreas[0].AxisY.Title = titleYAxis;

            chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;

            if (chart.Series[0].Points.Count > maximumNumberOfChartPoints)
            {
                chart.ChartAreas[0].AxisX.ScaleView.Zoom(chart.Series[0].Points[chart.Series[0].Points.Count - 100].XValue, chart.Series[0].Points[chart.Series[0].Points.Count - 1].XValue);
                chart.ChartAreas[0].AxisX.ScaleView.Position = chart.Series[0].Points.Count - maximumNumberOfChartPoints;
                chart.ChartAreas[0].AxisX.ScaleView.Size = maximumNumberOfChartPoints;
            }
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

        private void activeChart_ScrollerMove(object sender, ViewEventArgs e)
        {
            ChartArea activeChartArea = e.ChartArea;
            ChartArea inactiveChartArea = (activeChartArea == gsrChart.ChartAreas[0]) ? butterworthChart.ChartAreas[0] : gsrChart.ChartAreas[0];

            if (AxisName.X.Equals(e.Axis.AxisName))
            {
                inactiveChartArea.AxisX.ScaleView.Position = e.NewPosition;
                inactiveChartArea.AxisX.ScaleView.Size = e.NewSize;
                inactiveChartArea.AxisX.ScaleView.SizeType = e.NewSizeType;
            }
        }

        //Handle click on the Stop button
        private void stop_Click(object sender, EventArgs e)
        {
            String errorClosePort = "The port is already closed;";
            try
            {
                gsrAsset.StopSignalsRecord();
                timer1.Stop();
                lblErrors.Text = lblErrors.Text.ToString().Replace(errorClosePort, "");
            }
            catch (Exception exc)
            {
                if (!"BackgroundMode".Equals(settings.FormMode))
                {
                    lblErrors.Text = lblErrors.Text + errorClosePort;
                }
                logger.Log(Severity.Error, PrintException(exc));
            }
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

        private void imgZoomInOut_MouseOver(Object sender, EventArgs e)
        {
            ttpMouseInOut.Show("When stoped the GSR device you can use the mouse's wheel for zoom in/out.", gsrImgZoomInOut);
        }

        private void CheckDenoisedFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (checkDenoisedFilter.Checked) gsrChart.Series[medianLineName].Enabled = true;
            else gsrChart.Series[medianLineName].Enabled = false;
        }

        private void SignalVisualization_Closed(object sender, EventArgs e)
        {
            if (gsrAsset != null)
            {
                gsrAsset.StopSignalsRecord();
                gsrAsset.CloseSocket();
            }
        }

        private void btnStartSocket_Click(object sender, EventArgs e)
        {
            StartSocket();
        }

        private void StartSocket()
        {
            String errorStartSocket = "The socket can not be started. Please, check the log file;";
            try
            {
                gsrAsset.StartSocket();

                if (!"BackgroundMode".Equals(settings.FormMode))
                {
					bool result = gsrAsset.IsSocketConnected();
					if (gsrAsset.IsSocketConnected())
					{
						btnSocketLight.BackColor = Color.GreenYellow;
					}
					else
					{
						btnSocketLight.BackColor = Color.Red;
					}

                    btnStopSocket.Visible = true;
                    btnStartSocket.Visible = false;
                    lblErrors.Text = lblErrors.Text.ToString().Replace(errorStartSocket, "");
                }
            }
            catch (Exception e)
            {
                if (!"BackgroundMode".Equals(settings.FormMode))
                {
                    lblErrors.Text = lblErrors.Text + errorStartSocket;
                }
                logger.Log(Severity.Error, PrintException(e));
            }
        }

        private void btnStopSocket_Click(object sender, EventArgs e)
        {
            gsrAsset.CloseSocket();

            if (gsrAsset.IsSocketConnected()) btnSocketLight.BackColor = Color.GreenYellow;
            else btnSocketLight.BackColor = Color.Red;

            btnStopSocket.Visible = false;
            btnStartSocket.Visible = true;
        }

        private void gsrImgZoomInOut_Click(object sender, EventArgs e)
        {

        }
    }
}
