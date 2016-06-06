using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
