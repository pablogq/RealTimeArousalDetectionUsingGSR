using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;

namespace Assets.Rage.GSRAsset
{
    public class GSRSignalProcessor
    {
        public Dictionary<int, List<int>> channelsValues;
        public String pathToBinaryFile;
        public double gsrValuesReadTime;
        public Dictionary<int, Dictionary<double, int>> coordinates;


        public GSRSignalProcessor()
        {
            channelsValues = new Dictionary<int, List<int>>();
            //pathToBinaryFile = "E:/Projects/Rage/sample-binary.txt";
            pathToBinaryFile = ConfigurationManager.AppSettings.Get("BinaryFile");
            gsrValuesReadTime = (double)(DateTime.Now - DateTime.MinValue).TotalMilliseconds;
        }

        public void SetPathToBinaryFile(String path)
        {
            this.pathToBinaryFile = path;
        }

        //read binary file and collect values for each one channel
        public Dictionary<int, List<int>> ExtractChannelsValues()
        {
            using (BinaryReader binaryData = new BinaryReader(File.Open(pathToBinaryFile, FileMode.Open)))
            {
                // Position and length variables.
                int pos = 0;
                // Use BaseStream.
                int length = (int)binaryData.BaseStream.Length;
                while (pos < length)
                {
                    // Read integer.
                    byte[] unitValue = binaryData.ReadBytes(3);

                    //Console.WriteLine("\n\nStart number constricting.........................................");
                    TetradArray currentBCDChannelValue = new TetradArray();
                    TetradUtil tetradUtil = new TetradUtil();
                    int channelNumber = -1;

                    foreach (byte subUnit in unitValue)
                    {
                        tetradUtil.SetTetradChannelValuesByByte(subUnit);

                        string bcdNumber = tetradUtil.GetBCDNumber();
                        //Console.WriteLine("2 based number with zeros: " + bcdNumber);

                        channelNumber = tetradUtil.GetChannelNumber();
                        //Console.WriteLine("Channel number: " + channelNumber);

                        int tetradNumber = tetradUtil.GetTetradNumber();
                        //Console.WriteLine("Tetrad number: " + tetradNumber);

                        string tetradValue = tetradUtil.GetTetradValue();
                        //Console.WriteLine("Tetrad value: " + tetradValue);

                        currentBCDChannelValue = tetradUtil.SetBCDChannelValue(currentBCDChannelValue);
                    }

                    //add currentValue
                    int currentINTChannelValue = currentBCDChannelValue.GetTetradValue();
                    FillChannelsValues(channelNumber, currentINTChannelValue);

                    Console.WriteLine("currentBCDChannelValue: " + currentBCDChannelValue.GetThirdTetrad() + currentBCDChannelValue.GetSecondTetrad() + currentBCDChannelValue.GetFirstTetrad());
                    Console.WriteLine("currentINTChannelValue: " + currentINTChannelValue);

                    // Advance our position variable.
                    pos += 3;
                    //Console.WriteLine("End number construction.........................");
                }
            }

            //gsrValuesReadTime = DateTime.Now.Millisecond;
            return channelsValues;
        }

        private void FillChannelsValues(int channelNumber, int currentINTChannelValue)
        {
            if (channelsValues.ContainsKey(channelNumber) && currentINTChannelValue > -1)
            {
                List<int> channelTetradArray = null;
                channelsValues.TryGetValue(channelNumber, out channelTetradArray);
                if (channelTetradArray != null)
                {
                    channelTetradArray.Add(currentINTChannelValue);
                    channelsValues.Remove(channelNumber);
                    channelsValues.Add(channelNumber, channelTetradArray);
                }
            }
            else if (channelNumber < 4 && channelNumber > -1 && currentINTChannelValue > -1)
            {
                channelsValues.Add(channelNumber, new List<int> { currentINTChannelValue });
            }
        }

        //print all values from the binary file
        public void PrintChannelsValues()
        {
            foreach (KeyValuePair<int, List<int>> member in channelsValues)
            {
                Console.WriteLine("Channel with number: " + member.Key);
                List<int> valuesPerChannel = member.Value;
                foreach (int value in valuesPerChannel)
                {
                    Console.WriteLine("value: " + value);
                }
            }
        }

        public void FillCoordinates()
        {
            coordinates = new Dictionary<int, Dictionary<double, int>>();

            foreach (KeyValuePair<int, List<int>> entry in channelsValues)
            {
                int currentChannel = entry.Key;
                List<int> channelValues = entry.Value;
                Dictionary<double, int> currentChannelCoordinates = new Dictionary<double, int>();
                int channelValuesCount = channelValues.Count;
                for (int i = channelValuesCount - 1; i > -1; i--)
                {
                    double time = gsrValuesReadTime - (channelValuesCount - i - 1) * 10;
                    if (!currentChannelCoordinates.ContainsKey(time))
                    {
                        currentChannelCoordinates.Add(time, channelValues[i]);
                    }
                }

                if (!coordinates.ContainsKey(currentChannel))
                {
                    coordinates.Add(currentChannel, currentChannelCoordinates);
                }
            }
        }

        public Dictionary<int, Dictionary<double, int>> GetCoordinates()
        {
            return coordinates;
        }
    }
}
