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
