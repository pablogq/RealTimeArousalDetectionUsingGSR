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
    public class TetradUtil
    {
        public int channelNumber;
        public int tetradNumber;
        public string tetradValue;
        public string bcdNumber;

        public TetradUtil()
        {
            channelNumber = -1;
            tetradNumber = -1;
            tetradValue = String.Empty;
            bcdNumber = String.Empty;
        }

        public void SetTetradChannelValuesByByte(byte byteValue)
        {
            //Console.WriteLine("10 based number: " + unit);

            this.bcdNumber = Convert.ToString(byteValue, 2).PadLeft(8, '0');
            //Console.WriteLine("2 based number with zeros: " + bcdNumber);

            this.channelNumber = Convert.ToInt16(bcdNumber.Substring(0, 2), 2);
            //Console.WriteLine("Channel number: " + channelNumber);

            this.tetradNumber = Convert.ToInt16(bcdNumber.Substring(2, 2), 2);
            //Console.WriteLine("Tetrad number: " + tetradNumber);

            this.tetradValue = bcdNumber.Substring(4, 4);
            //Console.WriteLine("Tetrad value: " + tetradValue);
        }

        public int GetChannelNumber()
        {
            return this.channelNumber;
        }

        public int GetTetradNumber()
        {
            return this.tetradNumber;
        }

        public string GetTetradValue()
        {
            return this.tetradValue;
        }

        public string GetBCDNumber()
        {
            return this.bcdNumber;
        }

        public TetradArray SetBCDChannelValue(TetradArray bcdChannelValue)
        {
            if (tetradNumber == 0)
            {
                bcdChannelValue.SetFirstTetrad(tetradValue);
            }

            if (tetradNumber == 1)
            {
                bcdChannelValue.SetSecondTetrad(tetradValue);
            }

            if (tetradNumber == 2)
            {
                bcdChannelValue.SetThirtTetrad(tetradValue);
            }

            return bcdChannelValue;
        }
    }
}
