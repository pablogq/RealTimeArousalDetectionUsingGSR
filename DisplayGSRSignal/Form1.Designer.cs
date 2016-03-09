namespace DisplayGSRSignal
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.gsrChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.threeIn1 = new System.Windows.Forms.RadioButton();
            this.threeIn3 = new System.Windows.Forms.RadioButton();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.gsrChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1354, 188);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(69, 30);
            this.button1.TabIndex = 0;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // gsrChart
            // 
            this.gsrChart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.gsrChart.BackImageWrapMode = System.Windows.Forms.DataVisualization.Charting.ChartImageWrapMode.Unscaled;
            chartArea1.Name = "ChartArea1";
            this.gsrChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.gsrChart.Legends.Add(legend1);
            this.gsrChart.Location = new System.Drawing.Point(21, 31);
            this.gsrChart.Name = "gsrChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Legend = "Legend1";
            series1.Name = "Channel 1";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            series1.YValuesPerPoint = 20;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Legend = "Legend1";
            series2.Name = "Channel 2";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series3.Legend = "Legend1";
            series3.Name = "Channel 3";
            this.gsrChart.Series.Add(series1);
            this.gsrChart.Series.Add(series2);
            this.gsrChart.Series.Add(series3);
            this.gsrChart.Size = new System.Drawing.Size(1511, 447);
            this.gsrChart.TabIndex = 1;
            this.gsrChart.Text = "gsrChart";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1429, 187);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(74, 31);
            this.button2.TabIndex = 2;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // threeIn1
            // 
            this.threeIn1.AutoSize = true;
            this.threeIn1.Location = new System.Drawing.Point(1354, 118);
            this.threeIn1.Name = "threeIn1";
            this.threeIn1.Size = new System.Drawing.Size(161, 21);
            this.threeIn1.TabIndex = 3;
            this.threeIn1.TabStop = true;
            this.threeIn1.Text = "3 channels in 1 chart";
            this.threeIn1.UseVisualStyleBackColor = true;
            this.threeIn1.CheckedChanged += new System.EventHandler(this.threeIn1_CheckedChanged);
            // 
            // threeIn3
            // 
            this.threeIn3.AutoSize = true;
            this.threeIn3.Location = new System.Drawing.Point(1354, 145);
            this.threeIn3.Name = "threeIn3";
            this.threeIn3.Size = new System.Drawing.Size(168, 21);
            this.threeIn3.TabIndex = 4;
            this.threeIn3.TabStop = true;
            this.threeIn3.Text = "3 channels in 3 charts";
            this.threeIn3.UseVisualStyleBackColor = true;
            this.threeIn3.CheckedChanged += new System.EventHandler(this.threeIn3_CheckedChanged);
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(21, 484);
            this.chart1.Name = "chart1";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series4.Legend = "Legend1";
            series4.Name = "yyyy";
            this.chart1.Series.Add(series4);
            this.chart1.Size = new System.Drawing.Size(1511, 300);
            this.chart1.TabIndex = 5;
            this.chart1.Text = "chart1";
            this.chart1.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1535, 635);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.threeIn3);
            this.Controls.Add(this.threeIn1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.gsrChart);
            this.Name = "Form1";
            this.Text = "GSRSignal";
            ((System.ComponentModel.ISupportInitialize)(this.gsrChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataVisualization.Charting.Chart gsrChart;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RadioButton threeIn1;
        private System.Windows.Forms.RadioButton threeIn3;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}

