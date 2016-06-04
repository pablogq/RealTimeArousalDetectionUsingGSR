using System.IO.Ports;

namespace SignalDevice
{
    /*
     * The serial port should have only one instance.
     * Therefore we use the Singleton design pattern.
     */
    public class SignalDeviceSerialPort
    {
        private static SerialPort instance;
        private SignalDeviceSerialPort()
        {
            //super();
        }

        public static SerialPort Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SerialPort();
                }
                return instance;
            }
        }
    }
}
