using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Assets.Rage.GSRAsset;

namespace SocketServer.Socket
{
    class ClientHandler
    {
        private TcpClient ClientSocket;
        private NetworkStream networkStream;
        bool ContinueProcess = false;
        private byte[] bytes;       // Data buffer for incoming data.
        private StringBuilder sb = new StringBuilder(); // Received data string.
        private string data = null; // Incoming data from the client.

        public ClientHandler(TcpClient ClientSocket)
        {
            ClientSocket.ReceiveTimeout = 100; // 100 miliseconds
            this.ClientSocket = ClientSocket;
            networkStream = ClientSocket.GetStream();
            bytes = new byte[ClientSocket.ReceiveBufferSize];
            ContinueProcess = true;
        }

        public void Process()
        {
            try
            {
                int BytesRead = networkStream.Read(bytes, 0, (int)bytes.Length);
                if (BytesRead > 0)
                    // There might be more data, so store the data received so far.
                    sb.Append(Encoding.ASCII.GetString(bytes, 0, BytesRead));
                else
                    // All the data has arrived; put it in response.
                    ProcessDataReceived();
            }
            catch (IOException)
            {
                // All the data has arrived; put it in response.
                ProcessDataReceived();
            }
            catch (SocketException)
            {
                networkStream.Close();
                ClientSocket.Close();
                ContinueProcess = false;
                Logger.Log("Conection is broken!");
            }

        }  // Process()

        private void ProcessDataReceived()
        {
            GSRSignalProcessor gsrHandler = new GSRSignalProcessor();
            if (sb.Length > 0)
            {
                bool bQuit = (String.Compare(sb.ToString(), "quit", true) == 0);

                data = sb.ToString();

                sb.Length = 0; // Clear buffer

                Logger.Log("Text received from client:" + data);

                StringBuilder response = new StringBuilder();
                response.Append("Received at ");
                response.Append(DateTime.Now.ToString());
                response.Append("\r\n");
                response.Append(data);

                if(data.Equals("DoCalibration"))
                {
                    response.Append("\r\n");
                    response.Append(gsrHandler.EndOfCalibrationPeriod());
                }

                if(data.Equals("GetEDA"))
                {
                    response.Append("\r\n");
                    string result = gsrHandler.GetJSONArousalStatistics(gsrHandler.GetArousalStatistics());
                    response.Append(result);
                    //Logger.Log("Received request: GetEDA");
                    //Logger.Log("Send answer: " + result);
                }

                if(data.Equals("EndOfMesuarement"))
                {
                    response.Append("\r\n");
                    response.Append(gsrHandler.EndOfMeasurement());
                }

                // Echo the data back to the client.
                byte[] sendBytes = Encoding.ASCII.GetBytes(response.ToString());
                networkStream.Write(sendBytes, 0, sendBytes.Length);

                // Client stop processing  
                if (bQuit)
                {
                    networkStream.Close();
                    ClientSocket.Close();
                    ContinueProcess = false;
                }
            }
        }

        public void Close()
        {
            networkStream.Close();
            ClientSocket.Close();
        }

        public bool Alive
        {
            get
            {
                return ContinueProcess;
            }
        }
    }
}
