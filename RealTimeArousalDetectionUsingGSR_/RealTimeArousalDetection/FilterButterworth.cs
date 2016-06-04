using System;

namespace Assets.Rage.GSRAsset
{
    public class FilterButterworth
    {
        /// <summary>
        /// rez amount, from sqrt(2) to ~ 0.1
        /// </summary>
        private readonly double resonance = 0.0;

        private readonly double frequency;
        private readonly int sampleRate;
        private readonly ButterworthPassType passType;

        private readonly double c, a1, a2, a3, b1, b2;

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
        public FilterButterworth(double frequency, int sampleRate, ButterworthPassType passType)
        {
            //this.resonance = resonance;
            /*
             -  we used a 1st order, low-pass Butterworth filter set to 0.05 Hz to extract the tonic signal.
             */
            this.frequency = frequency;
            this.sampleRate = sampleRate;
            this.passType = passType;

            switch (passType)
            {
                case ButterworthPassType.Lowpass:
                    c = 1.0 / Math.Tan(Math.PI * frequency / sampleRate);
                    a1 = 1.0 / (1.0 + c * c);
                    //a1 = 1.0 / (1.0 + resonance * c + c * c);
                    a2 = 2.0 * a1;
                    a3 = a1;
                    b1 = 2.0 * (1.0 - c * c) * a1;
                    //b2 = (1.0 + c * c) * a1;
                    b2 = (1.0 - resonance * c + c * c) * a1;
                    break;
                case ButterworthPassType.Highpass:
                    c = Math.Tan(Math.PI * frequency / sampleRate);
                    a1 = 1.0 / (1.0 + c * c);
                    //a1 = 1.0 / (1.0 + resonance * c + c * c);
                    a2 = -2.0 * a1;
                    a3 = a1;
                    b1 = 2.0 * (c * c - 1.0) * a1;
                    //b2 = (1.0 + c * c) * a1;
                    b2 = (1.0 - resonance * c + c * c) * a1;
                    break;
            }
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
    }
}
