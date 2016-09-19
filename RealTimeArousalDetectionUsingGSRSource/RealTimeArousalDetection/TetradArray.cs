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

namespace Assets.Rage.GSRAsset.SignalProcessor
{
    public class TetradArray
    {
        public string firstTetrad;
        public string secondTetrad;
        public string thirdTetrad;

        public TetradArray()
        {
            firstTetrad = "0000";
            secondTetrad = "0000";
            thirdTetrad = "0000";
        }

        public void SetFirstTetrad(string firstTetrad)
        {
            this.firstTetrad = firstTetrad;
        }

        public void SetSecondTetrad(string secondTetrad)
        {
            this.secondTetrad = secondTetrad;
        }

        public void SetThirtTetrad(string thirdTetrad)
        {
            this.thirdTetrad = thirdTetrad;
        }

        public string GetFirstTetrad()
        {
            return firstTetrad;
        }

        public string GetSecondTetrad()
        {
            return secondTetrad;
        }


        public string GetThirdTetrad()
        {
            return thirdTetrad;
        }

        public int GetTetradValue()
        {
            return Convert.ToInt16(thirdTetrad + secondTetrad + firstTetrad, 2);
        }

    }
}
