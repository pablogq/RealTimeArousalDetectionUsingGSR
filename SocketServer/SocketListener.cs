using SocketServer.Properties;
using System;
using System.Net.Sockets;
using System.Threading;

namespace SocketServer.Socket
{
    public class SocketListener
    {
        private ClientService ClientTask;
        private TcpListener listener;

        public void StartListening()
        {
            // Client Connections Pool
            ClientConnectionPool ConnectionPool = new ClientConnectionPool();

            // Client Task to handle client requests
            ClientTask = new ClientService(ConnectionPool);

            ClientTask.Start();

            listener = new TcpListener(Settings.Default.SocketPortNumber);
            try
            {
                listener.Start();

                int TestingCycle = 15; // Number of testing cycles
                int ClientNbr = 0;

                // Start listening for connections.
                Logger.Log("Waiting for a connection...");
                while (TestingCycle > 0)
                {
                    TcpClient handler = listener.AcceptTcpClient();

                    if (handler != null)
                    {
                        Logger.Log("Client#" + (++ClientNbr) + " accepted!" );

                        // An incoming connection needs to be processed.
                        ConnectionPool.Enqueue(new ClientHandler(handler));

                        --TestingCycle;
                    }
                    else
                        break;
                }
                listener.Stop();

                // Stop client requests handling
                ClientTask.Stop();
            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
            }
        }

        public void CloseSocket()
        {
            try
            {
                ClientTask.Stop();
                listener.Stop();
            }
            catch(Exception e)
            {
                Logger.Log(e.ToString());
            }
        }

        public void Start()
        {
            Thread t = new Thread(StartListening);
            t.Start();
        }
    }
}
