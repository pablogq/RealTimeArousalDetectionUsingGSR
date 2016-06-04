using Assets.Rage.GSRAsset;
using System.Configuration;

namespace ReInitializeEDAValues
{
    class ReInitializer
    {
        static void Main(string[] args)
        {
            GSRSignalProcessor gsrHandler = new GSRSignalProcessor();
            gsrHandler.InitializeMinMaxValues();
        }
    }
}
