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
