using System;
using System.Text;

namespace Assets.Rage.GSRAsset
{
    public class TonicStatistics
    {
        private double slope;
        private double meanAmp;
        private double minAmp;
        private double maxAmp;
        private Decimal std;
        private int sclAchievedArousalLevel;

        public double Slope
        {
            get
            {
                return slope;
            }

            set
            {
                slope = value;
            }
        }

        public double MeanAmp
        {
            get
            {
                return meanAmp;
            }

            set
            {
                meanAmp = value;
            }
        }

        public double MinAmp
        {
            get
            {
                return minAmp;
            }

            set
            {
                minAmp = value;
            }
        }

        public double MaxAmp
        {
            get
            {
                return maxAmp;
            }

            set
            {
                maxAmp = value;
            }
        }

        public Decimal StdDeviation
        {
            get
            {
                return std;
            }

            set
            {
                std = value;
            }
        }

        public int SCLAchievedArousalLevel
        {
            get
            {
                return sclAchievedArousalLevel;
            }

            set
            {
                sclAchievedArousalLevel = value;
            }
        }

        public string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("Slope: " + slope + ": \n");
            str.Append("Minimum value: " + minAmp + "\n");
            str.Append("Maximum value: " + maxAmp + "\n");
            str.Append("Mean value: " + meanAmp + "\n");
            str.Append("Standard deviation: " + std + "\n");
            str.Append("Tonic level: " + sclAchievedArousalLevel + "\n");

            return str.ToString();
        }
    }
}