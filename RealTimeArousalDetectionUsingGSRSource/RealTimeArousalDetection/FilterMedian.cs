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
using System.Linq;

namespace Assets.Rage.GSRAsset.Utils
{
    class FilterMedian
    {
        private SignalDataByTime[] signalCoordinates;

        public FilterMedian()
        {
            //super();
        }

        public FilterMedian(SignalDataByTime[] signalCoordinates)
        {
            this.signalCoordinates = signalCoordinates;
        }

        public SignalDataByTime[] GetMedianFilterPoints()
        {
            if (this.signalCoordinates == null) return null;

            SignalDataByTime[]  signalCoordinatesExtented = extendSignalCoordinates(signalCoordinates);

            SignalDataByTime[] result = new SignalDataByTime[signalCoordinates.Length];
            for (int i = 2; i < signalCoordinatesExtented.Length - 2; ++i)
            {
                double[] window = new double[5];
                //store each 5 consecutive elements
                for (int j = 0; j < 5; ++j)
                {
                    window[j] = signalCoordinatesExtented.ElementAt(i - 2 + j).SignalValue;
                }

                //sort elements of the array window
                for (int j = 0; j < 3; ++j)
                {
                    //   Find position of minimum element
                    int min = j;
                    for (int k = j + 1; k < 5; ++k)
                        if (window[k] < window[min])
                            min = k;
                    //   Put found minimum element in its place
                    double temp = window[j];
                    window[j] = window[min];
                    window[min] = temp;
                }

                //   Get result - the middle element
                result[i - 2] = new SignalDataByTime(signalCoordinatesExtented.ElementAt(i-2).Time, window[2], signalCoordinatesExtented.ElementAt(i - 2).HighPassValue, signalCoordinatesExtented.ElementAt(i - 2).LowPassValue);
            }

            signalCoordinatesExtented = null;

            return result;
        }

        private SignalDataByTime[] extendSignalCoordinates(SignalDataByTime[] signalCoordinates)
        {
            if (signalCoordinates == null || signalCoordinates.Length == 0 || signalCoordinates.Length < 2) return signalCoordinates;

            SignalDataByTime[] result = new SignalDataByTime[signalCoordinates.Length + 4];
            result[0] = new SignalDataByTime(signalCoordinates.ElementAt(1).Time - 20.0, signalCoordinates.ElementAt(1).SignalValue);
            result[1] = new SignalDataByTime(signalCoordinates.ElementAt(0).Time - 10.0, signalCoordinates.ElementAt(0).SignalValue);
            for(int i = 0; i < signalCoordinates.Length; i++)
            {
                result[i + 2] = signalCoordinates.ElementAt(i);
            }
            result[signalCoordinates.Length + 2] = new SignalDataByTime(signalCoordinates.ElementAt(signalCoordinates.Length - 1).Time+ 10.0, signalCoordinates.ElementAt(signalCoordinates.Length - 1).SignalValue);
            result[signalCoordinates.Length + 3] = new SignalDataByTime(signalCoordinates.ElementAt(signalCoordinates.Length - 2).Time+ 20.0, signalCoordinates.ElementAt(signalCoordinates.Length - 2).SignalValue);

            return result;
        }
    }
}
