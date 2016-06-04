using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Rage.GSRAsset
{
    public class ButterworthFilterCoordinates
    {
        private Dictionary<double, double> lowPassCoordinates;
        private Dictionary<double, double> highPassCoordinates;

        public Dictionary<double, double> LowPassCoordinates
        {
            get
            {
                return lowPassCoordinates;
            }

            set
            {
                lowPassCoordinates = value;
            }
        }

        public Dictionary<double, double> HighPassCoordinates
        {
            get
            {
                return highPassCoordinates;
            }

            set
            {
                highPassCoordinates = value;
            }
        }
    }
}
