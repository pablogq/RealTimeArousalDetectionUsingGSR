using SignalDevice.Properties;
using System;
using System.Configuration;
using System.IO;
using System.IO.Ports;
//using SocketServer.Socket;

namespace Assets.Rage.GSRAsset.SignalDevice
{
    public class GSRHRDevice : ISignalDeviceController
    {
        // The main control for communicating through the RS-232 port
        private SerialPort comport = SignalDeviceSerialPort.Instance;
        private Settings settings = Settings.Default;
        private SignalDeviceUtils signalDeviceUtils;
        private Configuration appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        // Add TCP/IP socket for sending device data to remote applications
        //private SocketListener socketListener = new SocketListener();

        public GSRHRDevice()
        {
            signalDeviceUtils = new SignalDeviceUtils();

            // When data is recieved through the port, call this method
            comport.DataReceived += new SerialDataReceivedEventHandler(signalDeviceUtils.PortDataReceiver);
        }

        public void GetSignalData(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void OpenPort()
        {
            bool error = false;

            // If the port is open, close it.
            if (comport.IsOpen) comport.Close();
            else
            {
                SetSignalSettings();

                try
                {
                    // Open the port
                    comport.Open();
                }
                catch (UnauthorizedAccessException)
                {
                    error = true;
                }
                catch (IOException)
                {
                    error = true;
                }
                catch (ArgumentException)
                {
                    error = true;
                }
            }

            // If the port is open, send data
            if (comport.IsOpen)
            {
                signalDeviceUtils.SendData();
            }
        }

        public void SelectCOMPort(string portName)
        {
            if(!comport.IsOpen) comport.PortName = portName;
        }

        public void SetSignalSettings()
        {
            // Set the port's settings
            comport.BaudRate = settings.BaudRate;
            comport.DataBits = settings.DataBits;
            comport.StopBits = settings.StopBits;
            comport.Parity = settings.Parity;

            if (comport.PortName == null || comport.PortName.Length < 1)

            {
                comport.PortName = settings.PortName;
            }

            if (Convert.ToInt32(appConfig.AppSettings.Settings["Samplerate"].Value) < 1 && comport.IsOpen)
            {
                //default samplerate(speed/per second)
                comport.Write("1");
            }
            else
            {
                SetSignalSamplerate(appConfig.AppSettings.Settings["SamplerateLabel"].Value);
            }
        }

        public int SetSignalSamplerate(string samplerate)
        {
            if (comport.IsOpen)
            {
                comport.Write(appConfig.AppSettings.Settings["SamplerateLabel"].Value);
                return 0;
            }

            return -1;
        }

        public void SetSignalSamplerate()
        {
            if (comport.IsOpen) comport.Write(appConfig.AppSettings.Settings["SamplerateLabel"].Value);
        }

        public int GetSignalSampleRate()
        {
            return GetSamplerateByLabel(Convert.ToInt32(appConfig.AppSettings.Settings["Samplerate"].Value));
        }

        public int GetSamplerateByLabel(int sampleRate)
        {
            if (sampleRate == 2) return 2;
            if (sampleRate == 3) return 4;
            if (sampleRate == 4) return 5;
            if (sampleRate == 5) return 10;
            if (sampleRate == 6) return 20;
            if (sampleRate == 7) return 25;
            if (sampleRate == 8) return 50;
            if (sampleRate == 9) return 100;

            return 1;
            
        }

        public int StartSignalsRecord()
        {
            if (comport.IsOpen)
            {
                comport.Write(appConfig.AppSettings.Settings["SamplerateLabel"].Value);
                comport.Write("S");
                return 0;
            }

            return -1;
        }

        public int StopSignalsRecord()
        {
            if (comport.IsOpen)
            {
                comport.Write("E");
                return 0;
            }

            return -1;
        }
/*
        public void CloseSocket()
        {
            socketListener.CloseSocket();
        }

        public void StartSocketListening()
        {
            socketListener.Start();
        }
        */
    }
}
