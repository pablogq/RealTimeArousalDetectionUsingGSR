using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Assets.Rage.RealTimeArousalDetectionUsingGSRAsset.DisplayGSRSignal
{
    partial class SignalsVisualization
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.refreshButton = new System.Windows.Forms.Button();
            this.gsrChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.stopButton = new System.Windows.Forms.Button();
            this.checkDenoisedFilter = new System.Windows.Forms.CheckBox();
            this.butterworthChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.ArousalInfo = new System.Windows.Forms.Label();
            this.ArousalInfoButterworth = new System.Windows.Forms.Label();
            this.btnStartSocket = new System.Windows.Forms.Button();
            this.btnStopSocket = new System.Windows.Forms.Button();
            this.lblSocketAddress = new System.Windows.Forms.Label();
            this.lblSocketAddressValue = new System.Windows.Forms.Label();
            this.lblTimeWindow = new System.Windows.Forms.Label();
            this.lblTimeWindowValue = new System.Windows.Forms.Label();
            this.lblSampleRate = new System.Windows.Forms.Label();
            this.lblSampleRateValue = new System.Windows.Forms.Label();
            this.lblLowPassValue = new System.Windows.Forms.Label();
            this.lblHighPassValue = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ttpMouseInOut = new System.Windows.Forms.ToolTip(this.components);
            this.lblImgZoomInOut = new System.Windows.Forms.Label();
            this.gsrImgZoomInOut = new System.Windows.Forms.Button();
            this.btnSocketLight = new System.Windows.Forms.Button();
            this.lblErrors = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gsrChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.butterworthChart)).BeginInit();
            this.SuspendLayout();
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(1377, 171);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(69, 30);
            this.refreshButton.TabIndex = 0;
            this.refreshButton.Text = "Start";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refresh_Click);
            // 
            // gsrChart
            // 
            this.gsrChart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.gsrChart.BackImageWrapMode = System.Windows.Forms.DataVisualization.Charting.ChartImageWrapMode.Unscaled;
            chartArea1.Name = "ChartArea1";
            this.gsrChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.gsrChart.Legends.Add(legend1);
            this.gsrChart.Location = new System.Drawing.Point(73, 32);
            this.gsrChart.Name = "gsrChart";
            series1.BorderWidth = 2;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Legend = "Legend1";
            series1.Name = "Raw signal";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series1.YValuesPerPoint = 20;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series2.BorderWidth = 2;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.MediumSpringGreen;
            series2.Legend = "Legend1";
            series2.Name = "Denoised signal";
            this.gsrChart.Series.Add(series1);
            this.gsrChart.Series.Add(series2);
            this.gsrChart.Size = new System.Drawing.Size(1511, 502);
            this.gsrChart.TabIndex = 22;
            this.gsrChart.Text = "gsrChart";
            this.gsrChart.AxisViewChanged += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(this.activeChart_ScrollerMove);
            this.gsrChart.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gsrChart_MouseClick);
            this.gsrChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gsrChart_MouseMove);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(1467, 170);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(74, 31);
            this.stopButton.TabIndex = 2;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stop_Click);
            // 
            // checkDenoisedFilter
            // 
            this.checkDenoisedFilter.AutoSize = true;
            this.checkDenoisedFilter.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.checkDenoisedFilter.Checked = true;
            this.checkDenoisedFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkDenoisedFilter.Location = new System.Drawing.Point(1377, 133);
            this.checkDenoisedFilter.Name = "checkDenoisedFilter";
            this.checkDenoisedFilter.Size = new System.Drawing.Size(173, 21);
            this.checkDenoisedFilter.TabIndex = 21;
            this.checkDenoisedFilter.Text = "Activate denoised filter";
            this.checkDenoisedFilter.UseVisualStyleBackColor = false;
            this.checkDenoisedFilter.CheckedChanged += new System.EventHandler(this.CheckDenoisedFilter_CheckedChanged);
            // 
            // butterworthChart
            // 
            chartArea2.Name = "ChartArea1";
            this.butterworthChart.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.butterworthChart.Legends.Add(legend2);
            this.butterworthChart.Location = new System.Drawing.Point(73, 552);
            this.butterworthChart.Name = "butterworthChart";
            series3.BorderWidth = 2;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            series3.Legend = "Legend1";
            series3.Name = "Tonic line";
            series4.BorderWidth = 2;
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            series4.Legend = "Legend1";
            series4.Name = "Phasic line";
            this.butterworthChart.Series.Add(series3);
            this.butterworthChart.Series.Add(series4);
            this.butterworthChart.Size = new System.Drawing.Size(1511, 502);
            this.butterworthChart.TabIndex = 22;
            this.butterworthChart.Text = "butterworthfilter";
            title1.ForeColor = System.Drawing.Color.DodgerBlue;
            title1.Name = "Butterworth filter";
            this.butterworthChart.Titles.Add(title1);
            this.butterworthChart.AxisViewChanged += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(this.activeChart_ScrollerMove);
            this.butterworthChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.butterworthChart_MouseMove);
            // 
            // ArousalInfo
            // 
            this.ArousalInfo.AutoSize = true;
            this.ArousalInfo.Location = new System.Drawing.Point(1374, 258);
            this.ArousalInfo.Name = "ArousalInfo";
            this.ArousalInfo.Size = new System.Drawing.Size(0, 17);
            this.ArousalInfo.TabIndex = 29;
            // 
            // ArousalInfoButterworth
            // 
            this.ArousalInfoButterworth.AutoSize = true;
            this.ArousalInfoButterworth.Location = new System.Drawing.Point(1374, 638);
            this.ArousalInfoButterworth.MaximumSize = new System.Drawing.Size(350, 0);
            this.ArousalInfoButterworth.Name = "ArousalInfoButterworth";
            this.ArousalInfoButterworth.Size = new System.Drawing.Size(0, 17);
            this.ArousalInfoButterworth.TabIndex = 30;
            // 
            // btnStartSocket
            // 
            this.btnStartSocket.Location = new System.Drawing.Point(1409, 208);
            this.btnStartSocket.Name = "btnStartSocket";
            this.btnStartSocket.Size = new System.Drawing.Size(132, 30);
            this.btnStartSocket.TabIndex = 31;
            this.btnStartSocket.Text = "Start socket connection";
            this.btnStartSocket.UseVisualStyleBackColor = true;
            this.btnStartSocket.Click += new System.EventHandler(this.btnStartSocket_Click);
            // 
            // btnStopSocket
            // 
            this.btnStopSocket.Location = new System.Drawing.Point(1409, 209);
            this.btnStopSocket.Name = "btnStopSocket";
            this.btnStopSocket.Size = new System.Drawing.Size(132, 30);
            this.btnStopSocket.TabIndex = 32;
            this.btnStopSocket.Text = "Stop socket connection";
            this.btnStopSocket.UseVisualStyleBackColor = true;
            this.btnStopSocket.Click += new System.EventHandler(this.btnStopSocket_Click);
            // 
            // lblSocketAddress
            // 
            this.lblSocketAddress.AutoSize = true;
            this.lblSocketAddress.Location = new System.Drawing.Point(1377, 257);
            this.lblSocketAddress.Name = "lblSocketAddress";
            this.lblSocketAddress.Size = new System.Drawing.Size(110, 17);
            this.lblSocketAddress.TabIndex = 33;
            this.lblSocketAddress.Text = "Socket address:";
            // 
            // lblSocketAddressValue
            // 
            this.lblSocketAddressValue.AutoSize = true;
            this.lblSocketAddressValue.Location = new System.Drawing.Point(1493, 258);
            this.lblSocketAddressValue.Name = "lblSocketAddressValue";
            this.lblSocketAddressValue.Size = new System.Drawing.Size(0, 17);
            this.lblSocketAddressValue.TabIndex = 34;
            // 
            // lblTimeWindow
            // 
            this.lblTimeWindow.AutoSize = true;
            this.lblTimeWindow.Location = new System.Drawing.Point(1377, 278);
            this.lblTimeWindow.Name = "lblTimeWindow";
            this.lblTimeWindow.Size = new System.Drawing.Size(92, 17);
            this.lblTimeWindow.TabIndex = 35;
            this.lblTimeWindow.Text = "TimeWindow:";
            // 
            // lblTimeWindowValue
            // 
            this.lblTimeWindowValue.AutoSize = true;
            this.lblTimeWindowValue.Location = new System.Drawing.Point(1493, 278);
            this.lblTimeWindowValue.Name = "lblTimeWindowValue";
            this.lblTimeWindowValue.Size = new System.Drawing.Size(0, 17);
            this.lblTimeWindowValue.TabIndex = 36;
            // 
            // lblSampleRate
            // 
            this.lblSampleRate.AutoSize = true;
            this.lblSampleRate.Location = new System.Drawing.Point(1380, 299);
            this.lblSampleRate.Name = "lblSampleRate";
            this.lblSampleRate.Size = new System.Drawing.Size(88, 17);
            this.lblSampleRate.TabIndex = 37;
            this.lblSampleRate.Text = "Sample rate:";
            // 
            // lblSampleRateValue
            // 
            this.lblSampleRateValue.AutoSize = true;
            this.lblSampleRateValue.Location = new System.Drawing.Point(1493, 299);
            this.lblSampleRateValue.Name = "lblSampleRateValue";
            this.lblSampleRateValue.Size = new System.Drawing.Size(0, 17);
            this.lblSampleRateValue.TabIndex = 38;
            // 
            // lblLowPassValue
            // 
            this.lblLowPassValue.AutoSize = true;
            this.lblLowPassValue.Location = new System.Drawing.Point(1528, 602);
            this.lblLowPassValue.Name = "lblLowPassValue";
            this.lblLowPassValue.Size = new System.Drawing.Size(0, 17);
            this.lblLowPassValue.TabIndex = 39;
            // 
            // lblHighPassValue
            // 
            this.lblHighPassValue.AutoSize = true;
            this.lblHighPassValue.Location = new System.Drawing.Point(1528, 618);
            this.lblHighPassValue.Name = "lblHighPassValue";
            this.lblHighPassValue.Size = new System.Drawing.Size(0, 17);
            this.lblHighPassValue.TabIndex = 40;
            // 
            // lblImgZoomInOut
            // 
            this.lblImgZoomInOut.AutoSize = true;
            this.lblImgZoomInOut.Location = new System.Drawing.Point(1436, 385);
            this.lblImgZoomInOut.Margin = new System.Windows.Forms.Padding(0);
            this.lblImgZoomInOut.Name = "lblImgZoomInOut";
            this.lblImgZoomInOut.Size = new System.Drawing.Size(307, 34);
            this.lblImgZoomInOut.TabIndex = 42;
            this.lblImgZoomInOut.Text = "When stoped the GSR device \r\nyou can use the mouse\'s wheel for zoom in/out.";
            // 
            // gsrImgZoomInOut
            // 
            this.gsrImgZoomInOut.BackColor = System.Drawing.Color.Transparent;
            this.gsrImgZoomInOut.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.gsrImgZoomInOut.FlatAppearance.BorderSize = 0;
            this.gsrImgZoomInOut.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.gsrImgZoomInOut.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.gsrImgZoomInOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gsrImgZoomInOut.Image = global::DisplayGSRSignal.Properties.Resources.mouseScrollUpDown_small_2;
            this.gsrImgZoomInOut.Location = new System.Drawing.Point(1380, 371);
            this.gsrImgZoomInOut.Name = "gsrImgZoomInOut";
            this.gsrImgZoomInOut.Size = new System.Drawing.Size(53, 83);
            this.gsrImgZoomInOut.TabIndex = 41;
            this.gsrImgZoomInOut.UseVisualStyleBackColor = false;
            this.gsrImgZoomInOut.Click += new System.EventHandler(this.gsrImgZoomInOut_Click);
            this.gsrImgZoomInOut.MouseHover += new System.EventHandler(this.imgZoomInOut_MouseOver);
            // 
            // btnSocketLight
            // 
            this.btnSocketLight.BackColor = System.Drawing.Color.Green;
            this.btnSocketLight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSocketLight.Location = new System.Drawing.Point(1377, 210);
            this.btnSocketLight.Name = "btnSocketLight";
            this.btnSocketLight.Size = new System.Drawing.Size(26, 27);
            this.btnSocketLight.TabIndex = 43;
            this.btnSocketLight.UseVisualStyleBackColor = false;
            // 
            // lblErrors
            // 
            this.lblErrors.AutoSize = true;
            this.lblErrors.ForeColor = System.Drawing.Color.Red;
            this.lblErrors.Location = new System.Drawing.Point(144, 9);
            this.lblErrors.Name = "lblErrors";
            this.lblErrors.Size = new System.Drawing.Size(0, 17);
            this.lblErrors.TabIndex = 44;
            // 
            // SignalsVisualization
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1906, 898);
            this.Controls.Add(this.gsrImgZoomInOut);
            this.Controls.Add(this.lblErrors);
            this.Controls.Add(this.btnSocketLight);
            this.Controls.Add(this.lblImgZoomInOut);
            this.Controls.Add(this.lblHighPassValue);
            this.Controls.Add(this.lblLowPassValue);
            this.Controls.Add(this.lblSampleRateValue);
            this.Controls.Add(this.lblSampleRate);
            this.Controls.Add(this.lblTimeWindowValue);
            this.Controls.Add(this.lblTimeWindow);
            this.Controls.Add(this.lblSocketAddressValue);
            this.Controls.Add(this.lblSocketAddress);
            this.Controls.Add(this.btnStopSocket);
            this.Controls.Add(this.btnStartSocket);
            this.Controls.Add(this.ArousalInfoButterworth);
            this.Controls.Add(this.ArousalInfo);
            this.Controls.Add(this.butterworthChart);
            this.Controls.Add(this.checkDenoisedFilter);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.gsrChart);
            this.Name = "SignalsVisualization";
            this.Text = "GSRSignal";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SignalVisualization_Closed);
            ((System.ComponentModel.ISupportInitialize)(this.gsrChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.butterworthChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.DataVisualization.Charting.Chart gsrChart;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button stopButton;
        private CheckBox checkDenoisedFilter;
        private System.Windows.Forms.DataVisualization.Charting.Chart butterworthChart;
        private Label ArousalInfo;
        private Label ArousalInfoButterworth;
        private Button btnStartSocket;
        private Button btnStopSocket;
        private Label lblSocketAddress;
        private Label lblSocketAddressValue;
        private Label lblTimeWindow;
        private Label lblTimeWindowValue;
        private Label lblSampleRate;
        private Label lblSampleRateValue;
        private Label lblLowPassValue;
        private Label lblHighPassValue;
        private Button gsrImgZoomInOut;
        private ToolTip toolTip1;
        private ToolTip ttpMouseInOut;
        private Label lblImgZoomInOut;
        private Button btnSocketLight;
        private Label lblErrors;
    }
}

