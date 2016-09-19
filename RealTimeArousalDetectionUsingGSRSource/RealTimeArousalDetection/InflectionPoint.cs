using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Rage.GSRAsset.SignalProcessor
{
    public class InflectionPoint
    {
        private double x;
        private double y;
        private int index;
        private ExtremaType extremumType;

        public InflectionPoint(double x1, double y1, int indexOrigin)
        {
            this.x = x1;
            this.y = y1;
            this.index = indexOrigin;
        }


        public ExtremaType ExtremaType
        {
            get
            {
                return extremumType;
            }
            set
            {
                extremumType= value;
            }
        }

        public int IndexOrigin
        {
            get
            {
                return index;
            }
            set
            {
                x = value;
            }
        }

        public double CoordinateX
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public double CoordinateY
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
    }
}
