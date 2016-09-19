using Assets.Rage.GSRAsset.SignalProcessor;

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
