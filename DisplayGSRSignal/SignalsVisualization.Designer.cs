using System;
using System.Windows.Forms;

namespace DisplayGSRSignal
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
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.refreshButton = new System.Windows.Forms.Button();
            this.gsrChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.stopButton = new System.Windows.Forms.Button();
            this.MinYLbl = new System.Windows.Forms.Label();
            this.MinYTxtBox = new System.Windows.Forms.TextBox();
            this.MinYBtn = new System.Windows.Forms.Button();
            this.MaxYLbl = new System.Windows.Forms.Label();
            this.MaxYTxtBox = new System.Windows.Forms.TextBox();
            this.MaxYBtn = new System.Windows.Forms.Button();
            this.checkDenoisedFilter = new System.Windows.Forms.CheckBox();
            this.butterworthChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.MinYFltBtn = new System.Windows.Forms.Button();
            this.MinYFltTxtBox = new System.Windows.Forms.TextBox();
            this.MinYFltLbl = new System.Windows.Forms.Label();
            this.MaxYFltBtn = new System.Windows.Forms.Button();
            this.MaxYFltTxtBox = new System.Windows.Forms.TextBox();
            this.MaxYFltLbl = new System.Windows.Forms.Label();
            this.ArousalInfo = new System.Windows.Forms.Label();
            this.ArousalInfoButterworth = new System.Windows.Forms.Label();
            this.btnStartSocket = new System.Windows.Forms.Button();
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
            this.refreshButton.Text = "Refresh";
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
            this.gsrChart.Location = new System.Drawing.Point(73, 31);
            this.gsrChart.Name = "gsrChart";
            series1.BorderWidth = 2;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Legend = "Legend1";
            series1.Name = "GSR 1";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series1.YValuesPerPoint = 20;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series2.BorderWidth = 2;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Color = System.Drawing.Color.Fuchsia;
            series2.Legend = "Legend1";
            series2.Name = "Moving Average";
            series3.BorderWidth = 2;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Color = System.Drawing.Color.MediumSpringGreen;
            series3.Legend = "Legend1";
            series3.Name = "De-noised filter";
            this.gsrChart.Series.Add(series1);
            this.gsrChart.Series.Add(series2);
            this.gsrChart.Series.Add(series3);
            this.gsrChart.Size = new System.Drawing.Size(1511, 502);
            this.gsrChart.TabIndex = 22;
            this.gsrChart.Text = "gsrChart";
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
            // MinYLbl
            // 
            this.MinYLbl.AutoSize = true;
            this.MinYLbl.BackColor = System.Drawing.SystemColors.Window;
            this.MinYLbl.Location = new System.Drawing.Point(58, 422);
            this.MinYLbl.Name = "MinYLbl";
            this.MinYLbl.Size = new System.Drawing.Size(47, 17);
            this.MinYLbl.TabIndex = 14;
            this.MinYLbl.Text = "Min Y:";
            // 
            // MinYTxtBox
            // 
            this.MinYTxtBox.Location = new System.Drawing.Point(58, 442);
            this.MinYTxtBox.Name = "MinYTxtBox";
            this.MinYTxtBox.Size = new System.Drawing.Size(47, 22);
            this.MinYTxtBox.TabIndex = 15;
            // 
            // MinYBtn
            // 
            this.MinYBtn.BackgroundImage = global::DisplayGSRSignal.Properties.Resources.go_512;
            this.MinYBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.MinYBtn.Location = new System.Drawing.Point(111, 442);
            this.MinYBtn.Name = "MinYBtn";
            this.MinYBtn.Size = new System.Drawing.Size(25, 27);
            this.MinYBtn.TabIndex = 16;
            this.MinYBtn.UseVisualStyleBackColor = true;
            this.MinYBtn.Click += new System.EventHandler(this.MinYBtn_Click);
            // 
            // MaxYLbl
            // 
            this.MaxYLbl.AutoSize = true;
            this.MaxYLbl.BackColor = System.Drawing.SystemColors.Window;
            this.MaxYLbl.Location = new System.Drawing.Point(58, 31);
            this.MaxYLbl.Name = "MaxYLbl";
            this.MaxYLbl.Size = new System.Drawing.Size(50, 17);
            this.MaxYLbl.TabIndex = 18;
            this.MaxYLbl.Text = "Max Y:";
            // 
            // MaxYTxtBox
            // 
            this.MaxYTxtBox.Location = new System.Drawing.Point(58, 55);
            this.MaxYTxtBox.Name = "MaxYTxtBox";
            this.MaxYTxtBox.Size = new System.Drawing.Size(47, 22);
            this.MaxYTxtBox.TabIndex = 19;
            // 
            // MaxYBtn
            // 
            this.MaxYBtn.BackgroundImage = global::DisplayGSRSignal.Properties.Resources.go_512;
            this.MaxYBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.MaxYBtn.Location = new System.Drawing.Point(111, 55);
            this.MaxYBtn.Name = "MaxYBtn";
            this.MaxYBtn.Size = new System.Drawing.Size(25, 26);
            this.MaxYBtn.TabIndex = 20;
            this.MaxYBtn.UseVisualStyleBackColor = true;
            this.MaxYBtn.Click += new System.EventHandler(this.MaxYBtn_Click);
            // 
            // checkDenoisedFilter
            // 
            this.checkDenoisedFilter.AutoSize = true;
            this.checkDenoisedFilter.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.checkDenoisedFilter.Checked = true;
            this.checkDenoisedFilter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkDenoisedFilter.Location = new System.Drawing.Point(1377, 133);
            this.checkDenoisedFilter.Name = "checkDenoisedFilter";
            this.checkDenoisedFilter.Size = new System.Drawing.Size(178, 21);
            this.checkDenoisedFilter.TabIndex = 21;
            this.checkDenoisedFilter.Text = "Activate de-noised filter";
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
            series4.BorderWidth = 2;
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series4.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            series4.Legend = "Legend1";
            series4.Name = "Tonic line";
            series5.BorderWidth = 2;
            series5.ChartArea = "ChartArea1";
            series5.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series5.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            series5.Legend = "Legend1";
            series5.Name = "Phasic line";
            this.butterworthChart.Series.Add(series4);
            this.butterworthChart.Series.Add(series5);
            this.butterworthChart.Size = new System.Drawing.Size(1511, 502);
            this.butterworthChart.TabIndex = 22;
            this.butterworthChart.Text = "butterworthfilter";
            title1.ForeColor = System.Drawing.Color.DodgerBlue;
            title1.Name = "Butterworth filter";
            this.butterworthChart.Titles.Add(title1);
            this.butterworthChart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.butterworthChart_MouseMove);
            // 
            // MinYFltBtn
            // 
            this.MinYFltBtn.BackgroundImage = global::DisplayGSRSignal.Properties.Resources.go_512;
            this.MinYFltBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.MinYFltBtn.Location = new System.Drawing.Point(111, 745);
            this.MinYFltBtn.Name = "MinYFltBtn";
            this.MinYFltBtn.Size = new System.Drawing.Size(25, 27);
            this.MinYFltBtn.TabIndex = 24;
            this.MinYFltBtn.UseVisualStyleBackColor = true;
            this.MinYFltBtn.Click += new System.EventHandler(this.MinYFltBtn_Click);
            // 
            // MinYFltTxtBox
            // 
            this.MinYFltTxtBox.Location = new System.Drawing.Point(58, 745);
            this.MinYFltTxtBox.Name = "MinYFltTxtBox";
            this.MinYFltTxtBox.Size = new System.Drawing.Size(47, 22);
            this.MinYFltTxtBox.TabIndex = 23;
            // 
            // MinYFltLbl
            // 
            this.MinYFltLbl.AutoSize = true;
            this.MinYFltLbl.BackColor = System.Drawing.SystemColors.Window;
            this.MinYFltLbl.Location = new System.Drawing.Point(58, 725);
            this.MinYFltLbl.Name = "MinYFltLbl";
            this.MinYFltLbl.Size = new System.Drawing.Size(47, 17);
            this.MinYFltLbl.TabIndex = 25;
            this.MinYFltLbl.Text = "Min Y:";
            // 
            // MaxYFltBtn
            // 
            this.MaxYFltBtn.BackgroundImage = global::DisplayGSRSignal.Properties.Resources.go_512;
            this.MaxYFltBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.MaxYFltBtn.Location = new System.Drawing.Point(111, 570);
            this.MaxYFltBtn.Name = "MaxYFltBtn";
            this.MaxYFltBtn.Size = new System.Drawing.Size(25, 26);
            this.MaxYFltBtn.TabIndex = 28;
            this.MaxYFltBtn.UseVisualStyleBackColor = true;
            this.MaxYFltBtn.Click += new System.EventHandler(this.MaxYFltBtn_Click);
            // 
            // MaxYFltTxtBox
            // 
            this.MaxYFltTxtBox.Location = new System.Drawing.Point(58, 570);
            this.MaxYFltTxtBox.Name = "MaxYFltTxtBox";
            this.MaxYFltTxtBox.Size = new System.Drawing.Size(47, 22);
            this.MaxYFltTxtBox.TabIndex = 27;
            // 
            // MaxYFltLbl
            // 
            this.MaxYFltLbl.AutoSize = true;
            this.MaxYFltLbl.BackColor = System.Drawing.SystemColors.Window;
            this.MaxYFltLbl.Location = new System.Drawing.Point(58, 546);
            this.MaxYFltLbl.Name = "MaxYFltLbl";
            this.MaxYFltLbl.Size = new System.Drawing.Size(50, 17);
            this.MaxYFltLbl.TabIndex = 26;
            this.MaxYFltLbl.Text = "Max Y:";
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
            this.btnStartSocket.Location = new System.Drawing.Point(1377, 207);
            this.btnStartSocket.Name = "btnStartSocket";
            this.btnStartSocket.Size = new System.Drawing.Size(164, 30);
            this.btnStartSocket.TabIndex = 31;
            this.btnStartSocket.Text = "Start socket connection";
            this.btnStartSocket.UseVisualStyleBackColor = true;
            this.btnStartSocket.Click += new System.EventHandler(this.btnStartSocket_Click);
            // 
            // SignalsVisualization
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(1554, 898);
            this.Controls.Add(this.btnStartSocket);
            this.Controls.Add(this.ArousalInfoButterworth);
            this.Controls.Add(this.ArousalInfo);
            this.Controls.Add(this.MaxYFltBtn);
            this.Controls.Add(this.MaxYFltTxtBox);
            this.Controls.Add(this.MaxYFltLbl);
            this.Controls.Add(this.MinYFltLbl);
            this.Controls.Add(this.MinYFltBtn);
            this.Controls.Add(this.MinYFltTxtBox);
            this.Controls.Add(this.butterworthChart);
            this.Controls.Add(this.checkDenoisedFilter);
            this.Controls.Add(this.MaxYBtn);
            this.Controls.Add(this.MaxYTxtBox);
            this.Controls.Add(this.MaxYLbl);
            this.Controls.Add(this.MinYBtn);
            this.Controls.Add(this.MinYTxtBox);
            this.Controls.Add(this.MinYLbl);
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
        private System.Windows.Forms.Label MinYLbl;
        private System.Windows.Forms.TextBox MinYTxtBox;
        private System.Windows.Forms.Button MinYBtn;
        private System.Windows.Forms.Label MaxYLbl;
        private System.Windows.Forms.TextBox MaxYTxtBox;
        private System.Windows.Forms.Button MaxYBtn;
        private CheckBox checkDenoisedFilter;
        private System.Windows.Forms.DataVisualization.Charting.Chart butterworthChart;
        private Button MinYFltBtn;
        private TextBox MinYFltTxtBox;
        private Label MinYFltLbl;
        private Button MaxYFltBtn;
        private TextBox MaxYFltTxtBox;
        private Label MaxYFltLbl;
        private Label ArousalInfo;
        private Label ArousalInfoButterworth;
        private Button btnStartSocket;
    }
}

