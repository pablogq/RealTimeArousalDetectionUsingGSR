using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Rage.GSRAsset
{
    public class ArousalFeature
    {
        private double minimum;
        private double maximum;
        private Decimal mean;
        private Decimal std;
        private String name;
        private int count;

        public ArousalFeature()
        {
            //super();
        }

        public ArousalFeature(String name)
        {
            this.name = name;
        }

        public double Minimum
        {
            get
            {
                return minimum;
            }
            set
            {
                minimum = value;
            }
        }

        public double Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                maximum = value;
            }
        }

        public Decimal Mean
        {
            get
            {
                return mean;
            }
            set
            {
                mean = value;
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

        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("Statistics for " + name + ": \n");
            str.Append("Minimum value: " + minimum + "\n");
            str.Append("Maximum value: " + maximum + "\n");
            str.Append("Mean value: " + mean + "\n");
            str.Append("Standard deviation: " + std + "\n");
            str.Append("Count: " + count + "\n");

            return str.ToString();
        }
    }
}
