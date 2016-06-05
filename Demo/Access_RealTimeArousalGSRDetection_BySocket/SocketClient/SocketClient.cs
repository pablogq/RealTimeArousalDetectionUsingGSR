using System;
using System.Net.Sockets;
using System.Text;
using System.Timers;

class SocketClient
{
    private static Timer calibrationPeriodTimer;
    private static Timer getEDAPeriodTimer;
    private const int portNum = 10116;
    private static int count;
    private static bool calibrationStatus = true;
    static int timesCallingEda = 15;

    private static bool getDataStatus = true;

    static public void Main()
    {
        int calibrationPeriod = 20 * 1000;
        int getEDAPeriod = 5 * 1000;

        count = 0;
        calibrationPeriodTimer = new System.Timers.Timer();
        calibrationPeriodTimer.Interval = calibrationPeriod;
        calibrationPeriodTimer.Elapsed += new ElapsedEventHandler(SetCalibrationData);
        calibrationPeriodTimer.Start();

        while (calibrationStatus)
        { }

        count = 0;
        getEDAPeriodTimer = new Timer();
        getEDAPeriodTimer.Interval = getEDAPeriod;
        getEDAPeriodTimer.Elapsed += new ElapsedEventHandler(GetGSRData);
        getEDAPeriodTimer.Start();

        while (getDataStatus)
        { }
    }       

    static void SendMessage(string dataToSend)
    {
        TcpClient tcpClient = new TcpClient();
        try
        {
            tcpClient.Connect("localhost", portNum);
            NetworkStream networkStream = tcpClient.GetStream();
            if (networkStream.CanWrite && networkStream.CanRead)
            {
                Console.WriteLine("Send message: " + dataToSend);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(dataToSend);
                networkStream.Write(sendBytes, 0, sendBytes.Length);

                // Reads the NetworkStream into a byte buffer.
                byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
                int BytesRead = networkStream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);

                // Returns the data received from the host to the console.
                string returndata = Encoding.ASCII.GetString(bytes, 0, BytesRead);
                Console.WriteLine("This is what the host returned to you: \r\n{0}", returndata);

                networkStream.Close();
                tcpClient.Close();
            }
            else if (!networkStream.CanRead)
            {
                Console.WriteLine("You can not write data to this stream");
                networkStream.Close();
                tcpClient.Close();
            }
            else if (!networkStream.CanWrite)
            {
                Console.WriteLine("You can not read data from this stream");
                networkStream.Close();
                tcpClient.Close();
            }
            networkStream.Close();
            tcpClient.Close();
        }
        catch (SocketException)
        {
            Console.WriteLine("Sever not available!");
        }
        catch (System.IO.IOException)
        {
            Console.WriteLine("Sever not available!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    static void SetCalibrationData(object sender, EventArgs e)
    {
        if (count++ >= 0)
        {
            calibrationPeriodTimer.Stop();
            calibrationStatus = false;
        }
        SendMessage("EOCP");
        Console.WriteLine("calibration");
    }

    static void GetGSRData(object sender, EventArgs e)
    {
        Console.WriteLine("count: " + count);
        Console.WriteLine("timesCallingEda: " + timesCallingEda);
        if (count++ >= (timesCallingEda))
        {
            SendMessage("EOM");
            Console.WriteLine("End...");

            getEDAPeriodTimer.Stop();
            getDataStatus = false;
        }
        SendMessage("GET_EDA");
        Console.WriteLine("eda");
    }
} 
