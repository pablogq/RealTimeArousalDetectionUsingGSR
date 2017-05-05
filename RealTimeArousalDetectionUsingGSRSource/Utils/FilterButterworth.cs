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

namespace Assets.Rage.GSRAsset.Utils
{
    public class FilterButterworth
    {
        /// <summary>
        /// rez amount should be from sqrt(2) to ~ 0.1.
        /// </summary>
        private readonly double resonance = Math.Sqrt(2);

        private readonly double frequency;
        private readonly int sampleRate;
        private readonly ButterworthPassType passType;

        private double c, a1, a2, a3, b1, b2;

        /// <summary>
        /// Array of input values, latest are in front
        /// </summary>
        private double[] inputHistory = new double[2];

        /// <summary>
        /// Array of output values, latest are in front
        /// </summary>
        private double[] outputHistory = new double[3];

        /// <summary>
        /// Initialize parameters of the Butterworth filter
        /// </summary>
        public FilterButterworth(double frequency, int sampleRate, ButterworthPassType passType, String calledBy)
        {
            //this.resonance = resonance;
            /*
             -  we used a 1st order, low-pass Butterworth filter set to 0.05 Hz to extract the tonic signal.
             */
            this.frequency = frequency;
            this.sampleRate = sampleRate;
            this.passType = passType;
            this.inputHistory[0] = 0.0;
            this.inputHistory[1] = 0.0;
            this.outputHistory[0] = 0.0;
            this.outputHistory[1] = 0.0;
            this.outputHistory[2] = 0.0;
            SetButterworthParameters(passType);
        }

        public FilterButterworth(double frequency, int sampleRate)
        {
            this.frequency = frequency;
            this.sampleRate = sampleRate;
        }

        public void Update(double newInput)
        {
            double newOutput = a1 * newInput + a2 * this.inputHistory[0] + a3 * this.inputHistory[1] - b1 * this.outputHistory[0] - b2 * this.outputHistory[1];

            this.inputHistory[1] = this.inputHistory[0];
            this.inputHistory[0] = newInput;

            this.outputHistory[2] = this.outputHistory[1];
            this.outputHistory[1] = this.outputHistory[0];
            this.outputHistory[0] = newOutput;
        }

        public double GetFilterValue(double newInput)
        {
            Update(newInput);
            return Math.Round(this.outputHistory[0], 3); 
        }

        public double GetFilterValue(double newInput, ButterworthPassType passType)
        {
            SetButterworthParameters(passType);
            Update(newInput);
            return Math.Round(this.outputHistory[0], 3);
        }

        private void SetButterworthParameters(ButterworthPassType passType)
        {
            switch (passType)
            {
                case ButterworthPassType.Lowpass:
                    c = 1.0 / Math.Tan(Math.PI * frequency / sampleRate);
                    //a1 = 1.0 / (1.0 + c * c);
                    a1 = 1.0 / (1.0 + resonance * c + c * c);
                    a2 = 2.0 * a1;
                    a3 = a1;
                    b1 = 2.0 * (1.0 - c * c) * a1;
                    //b2 = (1.0 + c * c) * a1;
                    b2 = (1.0 - resonance * c + c * c) * a1;
                    break;
                case ButterworthPassType.Highpass:
                    c = Math.Tan(Math.PI * frequency / sampleRate);
                    //a1 = 1.0 / (1.0 + c * c);
                    a1 = 1.0 / (1.0 + resonance * c + c * c);
                    a2 = -2.0 * a1;
                    a3 = a1;
                    b1 = 2.0 * (c * c - 1.0) * a1;
                    //b2 = (1.0 + c * c) * a1;
                    b2 = (1.0 - resonance * c + c * c) * a1;
                    break;
            }
        }
    }
}
