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
    public class ArousalStatistics
    {
        private double scrArousalArea;
        private double scrAchievedArousalLevel;
        private double sclAchievedArousalLevel;
        private double generalArousalLevel;
        private ArousalFeature scrAmplitude;
        private ArousalFeature scrRise;
        private ArousalFeature scrRecoveryTime;
        private TonicStatistics tonicStatistics;
        private double movingAverage;
        private double lastValue;
        private double lastRawSignalValue;
        private double lastMedianFilterlValue;
        private double highPassSignalValue;
        private double lowPassSignalValue;

        public ArousalStatistics()
        {
            //super();
        }

        public double SCRArousalArea
        {
            get
            {
                return scrArousalArea;
            }
            set
            {
                scrArousalArea = value;
            }
        }

        public ArousalFeature SCRAmplitude
        {
            get
            {
                return scrAmplitude;
            }
            set
            {
                scrAmplitude = value;
            }
        }

        public ArousalFeature SCRRise
        {
            get
            {
                return scrRise;
            }
            set
            {
                scrRise = value;
            }
        }

        public ArousalFeature SCRRecoveryTime
        {
            get
            {
                return scrRecoveryTime;
            }
            set
            {
                scrRecoveryTime = value;
            }
        }

        public double SCRAchievedArousalLevel
        {
            get
            {
                return scrAchievedArousalLevel;
            }
            set
            {
                scrAchievedArousalLevel = value;
            }
        }

        public double GeneralArousalLevel
        {
            get
            {
                return generalArousalLevel;
            }
            set
            {
                generalArousalLevel = value;
            }
        }

        public TonicStatistics TonicStatistics
        {
            get
            {
                return tonicStatistics;
            }

            set
            {
                tonicStatistics = value;
            }
        }

        public double SCLAchievedArousalLevel
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

        public double MovingAverage
        {
            get
            {
                return movingAverage;
            }

            set
            {
                movingAverage = value;
            }
        }

        public double LastValue
        {
            get
            {
                return lastValue;
            }

            set
            {
                lastValue = value;
            }
        }

        public double LastMedianFilterValue
        {
            get
            {
                return lastMedianFilterlValue;
            }

            set
            {
                lastMedianFilterlValue = value;
            }
        }

        public double LastRawSignalValue
        {
            get
            {
                return lastRawSignalValue;
            }

            set
            {
                lastRawSignalValue = value;
            }
        }

        public double HighPassSignalValue
        {
            get
            {
                return highPassSignalValue;
            }

            set
            {
                highPassSignalValue = value;
            }
        }

        public double LowPassSignalValue
        {
            get
            {
                return lowPassSignalValue;
            }

            set
            {
                lowPassSignalValue = value;
            }
        }
    }
}
