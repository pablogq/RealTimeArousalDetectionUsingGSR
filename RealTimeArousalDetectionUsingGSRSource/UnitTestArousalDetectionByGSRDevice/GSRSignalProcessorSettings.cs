using Assets.Rage.GSRAsset.Integrator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assets.Rage.GSRAsset.UnitTest
{
    [TestClass]
    public class GSRSignalProcessorSettings
    {
        [TestMethod]
        public void SetMaxArousalLevel_SetNewValue_ReturnTrue()
        {
            //arrange
            ArousalDetectionGSRDeviceIntegrator gsrHandler = new ArousalDetectionGSRDeviceIntegrator();

            //act
            int newMaxArousalLevel = gsrHandler.SetMaxArousalLevel(2);

            //assert
            Assert.AreEqual(gsrHandler.GetGSRSignalProcessor().ArousalLevel, 2);
        }

        [TestMethod]
        public void SetMaxArousalLevel_SetNewValue_ReturnFalse()
        {
            //arrange
            ArousalDetectionGSRDeviceIntegrator gsrHandler = new ArousalDetectionGSRDeviceIntegrator();
            int oldArousalLevel = gsrHandler.GetGSRSignalProcessor().ArousalLevel;

            //act
            int newMaxArousalLevel = gsrHandler.SetMaxArousalLevel(oldArousalLevel + 1);

            //assert
            Assert.AreNotEqual(gsrHandler.GetGSRSignalProcessor().ArousalLevel, oldArousalLevel);
        }

        [TestMethod]
        public void SetTimeWindow_SetNewValue_ReturnTrue()
        {
            //arrange
            ArousalDetectionGSRDeviceIntegrator gsrHandler = new ArousalDetectionGSRDeviceIntegrator();

            //act
            int newMaxArousalLevel = gsrHandler.SetTimeWindow(15);

            //assert
            Assert.AreEqual(gsrHandler.GetGSRSignalProcessor().DefaultTimeWindow, 15);
        }

        [TestMethod]
        public void SetTimeWindow_SetNewValue_ReturnFalse()
        {
            //arrange
            ArousalDetectionGSRDeviceIntegrator gsrHandler = new ArousalDetectionGSRDeviceIntegrator();
            double oldTimeWindow = gsrHandler.GetGSRSignalProcessor().DefaultTimeWindow;

            //act
            int newMaxArousalLevel = gsrHandler.SetTimeWindow(oldTimeWindow + 10);

            //assert
            Assert.AreNotEqual(gsrHandler.GetGSRSignalProcessor().DefaultTimeWindow, oldTimeWindow);
        }
    }
}
