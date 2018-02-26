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

using System.Collections.Generic;

namespace Assets.Rage.RealTimeArousalDetectionUsingGSRAsset.Utils
{
    public class ButterworthFilterCoordinates
    {
        private List<SignalDataByTime> lowPassCoordinates;
        private List<SignalDataByTime> highPassCoordinates;

        public ButterworthFilterCoordinates()
        {
            //
        }

        public ButterworthFilterCoordinates(List<SignalDataByTime> lowPassCoordinates, List<SignalDataByTime> highPassCoordinates)
        {
            this.lowPassCoordinates = lowPassCoordinates;
            this.highPassCoordinates = highPassCoordinates;
        }

        public List<SignalDataByTime> LowPassCoordinates
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

        public List<SignalDataByTime> HighPassCoordinates
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
