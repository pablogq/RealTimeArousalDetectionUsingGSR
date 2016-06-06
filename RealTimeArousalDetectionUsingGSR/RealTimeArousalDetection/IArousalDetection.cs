using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Rage.GSRAsset.SignalProcessor
{
    public enum GSRFeature { SCRArousalArea, SCRAchievedArousalLevel,
                             SCLAchievedArousalLevel, MovingAverage };

    public interface IArousalDetection
    {
        int SetTimeWindow(int timeWindow);
        int SetMaxArousalLevel(int numberOfLevels);
        double GetGSRFeature(string featureName);
    }
}
