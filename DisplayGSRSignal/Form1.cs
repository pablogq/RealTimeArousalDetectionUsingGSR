using System;
using System.Collections.Generic;
using Assets.Rage.GSRAsset;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Linq;

namespace DisplayGSRSignal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ChartClean();
            GSRSignalProcessor gsrHandler = new GSRSignalProcessor();
            ChartArea chart = chart1.ChartAreas[0];
            ///*
            Dictionary<int, List<int>> channelsValues = gsrHandler.ExtractChannelsValues();
            gsrHandler.FillCoordinates();

            int blockSize = 100;
            double maxValue = 0.0;
            double minValue = 0.0;
            int numPoints = 0;
            foreach (KeyValuePair<int, Dictionary<double, int>> channelCoordinates in gsrHandler.coordinates)
            {
                foreach (KeyValuePair<double, int> coordinate in channelCoordinates.Value)
                {
                    if(numPoints < 4700)
                    {
                        if (maxValue.Equals(0.0)) maxValue = coordinate.Key;
                        Log(maxValue);
                        minValue = coordinate.Key;
                        Log("minValue: " + minValue);
                        chart1.Series["GSR Channel 1"].Points.AddXY(coordinate.Key, coordinate.Value);
                    }
                    numPoints++;
                }
                
            }

            chart1.Series["GSR Channel 1"].ChartType = SeriesChartType.Spline;

            chart.AxisX.Title = "Time";
            chart.AxisY.Title = "GSR";

            // Set maximum values.
            chart.AxisX.Maximum = maxValue;

            // enable autoscroll
            chart.CursorX.AutoScroll = true;

            // let's zoom to [0,blockSize] (e.g. [0,100])
            chart.AxisX.ScaleView.Zoomable = true;
            chart.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;

            chart.AxisX.ScaleView.Zoom(minValue, minValue+800);

            // disable zoom-reset button (only scrollbar's arrows are available)
            chart.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;

            // set scrollbar small change 
            chart.AxisX.ScaleView.SmallScrollSize = 800;
            //*/
            /*

            int blockSize = 100;

            // generates random data (i.e. 30 * blockSize random numbers)
            Random rand = new Random();
            var valuesArray = Enumerable.Range(0, blockSize * 30).Select(x => rand.Next(1, 10)).ToArray();

            // clear the chart
            chart1.Series.Clear();

            // fill the chart
            var series = chart1.Series.Add("My Series");
            series.ChartType = SeriesChartType.Line;
            series.XValueType = ChartValueType.Int32;
            for (int i = 0; i < valuesArray.Length; i++)
                series.Points.AddXY(i, valuesArray[i]);
            var chartArea = chart1.ChartAreas[series.ChartArea];

            // set view range to [0,max]
            chartArea.AxisX.Minimum = 0.0;
            chartArea.AxisX.Maximum = 5000.20;

            // enable autoscroll
            chartArea.CursorX.AutoScroll = true;

            // let's zoom to [0,blockSize] (e.g. [0,100])
            chartArea.AxisX.ScaleView.Zoomable = true;
            chartArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            int position = 0;
            int size = blockSize;
            chartArea.AxisX.ScaleView.Zoom(position, 1000);

            // disable zoom-reset button (only scrollbar's arrows are available)
            chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;

            // set scrollbar small change to blockSize (e.g. 100)
            chartArea.AxisX.ScaleView.SmallScrollSize = blockSize;
            */

        }

        private void ChartClean()
        {
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }
        }

        public static void Log(object logMessage)
        {
            using (StreamWriter w = File.AppendText("c:\\Users\\ddessy\\log.txt"))
            {
                logMessage = logMessage.ToString();
                w.Write("\r\nLog Entry : ");
                w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                w.WriteLine("  :");
                w.WriteLine("  :{0}", logMessage);
                w.WriteLine("-------------------------------");
            }
        }
    }
}
