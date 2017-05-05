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

using System;
using System.Text;

namespace Assets.Rage.GSRAsset.Utils
{
    public class TonicStatistics
    {
        private double slope;
        private double meanAmp;
        private double minAmp;
        private double maxAmp;
        private Decimal stdDeviation;

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
                return stdDeviation;
            }

            set
            {
                stdDeviation = value;
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("Slope: " + slope + ": \n");
            str.Append("Minimum value: " + minAmp + "\n");
            str.Append("Maximum value: " + maxAmp + "\n");
            str.Append("Mean value: " + meanAmp + "\n");
            str.Append("Standard deviation: " + stdDeviation + "\n");

            return str.ToString();
        }
    }
}