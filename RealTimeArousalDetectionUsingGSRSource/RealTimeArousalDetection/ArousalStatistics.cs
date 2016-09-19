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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Rage.GSRAsset.SignalProcessor
{
    public class ArousalStatistics
    {
        /*
        private int scrNumberOfInflectionPoints;
        private int scrNumberOfPositiveInflectionPoints;
        private int scrNumberOfNegativeInflectionPoints;
        private int scrNumberOfNeutralInflectionPoints;
        
        private double summaryArousal;
        */

        private double scrArousalArea;
        private double scrAchievedArousalLevel;
        private double sclAchievedArousalLevel;
        private ArousalFeature scrAmplitude;
        private ArousalFeature scrRise;
        private ArousalFeature scrRecoveryTime;
        private TonicStatistics tonicStatistics;
        private double movingAverage;

        public ArousalStatistics()
        {
            //super();
        }

        /*
        public int NumberPositiveInflectionPoints
        {
            get
            {
                return scrNumberOfPositiveInflectionPoints;
            }
            set
            {
                scrNumberOfPositiveInflectionPoints = value;
            }
        }

        public double SummaryArousal
        {
            get
            {
                return summaryArousal;
            }
            set
            {
                summaryArousal = value;
            }
        }


        public int NumberNegativeInflectionPoints
        {
            get
            {
                return scrNumberOfNegativeInflectionPoints;
            }
            set
            {
                scrNumberOfNegativeInflectionPoints = value;
            }
        }

        public int NumberOfNeutralInflectionPoints
        {
            get
            {
                return scrNumberOfNeutralInflectionPoints;
            }
            set
            {
                scrNumberOfNeutralInflectionPoints = value;
            }
        }
        

        public int NumberInflectionPoints
        {
            get
            {
                return scrNumberOfInflectionPoints;
            }
            set
            {
                scrNumberOfInflectionPoints = value;
            }
        }
        */

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

        public string ToString(String title)
        {
            StringBuilder str = new StringBuilder();
            str.Append("Arousal statistics for " + title + ": \n\n");
            //str.Append("Number of inflection points: " + scrNumberOfInflectionPoints + "\n");
            //str.Append("Number of positive inflection points: " + scrNumberOfPositiveInflectionPoints + "\n");
            //str.Append("Number of negative inflection points: " + scrNumberOfNegativeInflectionPoints + "\n");
            //str.Append("Number of neutral inflection points: " + scrNumberOfNeutralInflectionPoints + "\n");
            //str.Append("Summary arousal: " + summaryArousal + "\n");
            str.Append("Arousal area: " + scrArousalArea + "\n");
            str.Append("Moving average: " + movingAverage + "\n");
            str.Append(scrAmplitude.ToString());
            //str.Append(scrRise.ToString());
            //str.Append(scrRecoveryTime.ToString());
            str.Append("SCR arousal level: " + scrAchievedArousalLevel);
        
            return str.ToString();
        }
    }
}
