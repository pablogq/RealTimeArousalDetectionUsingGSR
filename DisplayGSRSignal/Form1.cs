using System;
using System.Collections.Generic;
using Assets.Rage.GSRAsset;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Configuration;
using System.Drawing;

namespace DisplayGSRSignal
{
    public partial class Form1 : Form
    {
        int number;

        public Form1()
        {
            InitializeComponent();
            number = 1;
            threeIn1.Checked = true;
            chart1.Visible = false;
            GSRChartDisplay();
            timer1.Tick += timer1_Tick;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
            GSRChartDisplay();
        }

        private void GSRChartDisplay()
        {
            ChartClean();
            GSRSignalProcessor gsrHandler = new GSRSignalProcessor();
            ChartArea chart = gsrChart.ChartAreas[0];

            Dictionary<int, List<int>> channelsValues = gsrHandler.ExtractChannelsValues();
            gsrHandler.FillCoordinates();

            double maxValue = 0.0;
            double minValue = 0.0;
            int numPoints = 0;
            foreach (KeyValuePair<int, Dictionary<double, int>> channelCoordinates in gsrHandler.coordinates)
            {
                foreach (KeyValuePair<double, int> coordinate in channelCoordinates.Value)
                {
                    //numPoints > (number - 1) * 5000 && numPoints < number*5000
                    if (numPoints < number*5000)
                    {
                        if (maxValue.Equals(0.0)) maxValue = coordinate.Key;
                        minValue = coordinate.Key;
                        gsrChart.Series["Channel 1"].Points.AddXY(coordinate.Key, coordinate.Value);
                    }
                    numPoints++;
                }

            }

            chart.AxisX.Title = "Time";
            chart.AxisY.Title = "GSR";

            // Set maximum values.
            chart.AxisX.Maximum = maxValue;

            // enable autoscroll
            chart.CursorX.AutoScroll = true;

            //zoom to [minValue, minvalue+numberOfVisiblePoints]
            chart.AxisX.ScaleView.Zoomable = true;
            chart.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;

            chart.AxisX.ScaleView.Zoom(minValue, minValue + 800);

            // disable zoom-reset button (only scrollbar's arrows are available)
            chart.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;

            // set scrollbar small change 
            chart.AxisX.ScaleView.SmallScrollSize = 800;

            DisplayFakeSeriesForTest(minValue);
        }

        private void DisplayFakeSeriesForTest(double minValue)
        {
            Random rdn = new Random();
            for (int i = 0; i < 4700; i++)
            {
                gsrChart.Series["Channel 2"].Points.AddXY
                                (minValue + i * 20, rdn.Next(1, 8));
                gsrChart.Series["Channel 3"].Points.AddXY
                                (minValue + i * 10, rdn.Next(1, 8));
            }

            gsrChart.Series["Channel 2"].Color = Color.Green;
            gsrChart.Series["Channel 3"].Color = Color.Magenta;

            //gsrChart.Series["Channel 2"].IsVisibleInLegend = false;
            //gsrChart.Series["Channel 3"].IsVisibleInLegend = false;
        }

        private void ChartClean()
        {
            foreach (var series in gsrChart.Series)
            {
                series.Points.Clear();
            }
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            number++;
            GSRChartDisplay();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void threeIn3_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Visible = true;
            chart1.Series.Add(gsrChart.Series["Channel 2"]);
            gsrChart.Series.Remove(gsrChart.Series["Channel 2"]);
        }

        private void threeIn1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var series in chart1.Series)
            {
                if ("Channel 2".Equals(series.Name))
                {
                    gsrChart.Series["Channel 2"] = chart1.Series["Channel 2"];
                    chart1.Series.Remove(gsrChart.Series["Channel 2"]);
                }
            }


            chart1.Visible = false;
            chart1.Series["yyyy"].IsVisibleInLegend = false;
        }
    }
}
