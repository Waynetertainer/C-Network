using System;
using System.Net.Sockets;

namespace UnityServer
{
    class Clients
    {
        public int connectionID;
        public string ip;
        public TcpClient socket;
        public NetworkStream myStream;
        private byte[] readBuffer;
        public ByteBuffer Buffer;

        public void Start()
        {
            socket.SendBufferSize = 4096;
            socket.ReceiveBufferSize = 4096;
            myStream = socket.GetStream();
            readBuffer = new byte[4096];
            myStream.BeginRead(readBuffer, 0, socket.ReceiveBufferSize, ReceiveDataCallback, null);
        }

        private void ReceiveDataCallback(IAsyncResult result)
        {
            try
            {
                int readbytes = myStream.EndRead(result);
                if (readbytes <= 0)
                {
                    CloseConnection();
                    return;
                }
                byte[] newBytes = new byte[readbytes];
                System.Buffer.BlockCopy(readBuffer, 0, newBytes, 0, readbytes);

                ServerHandlePackets.HandleData(connectionID,newBytes);

                myStream.BeginRead(readBuffer, 0, socket.ReceiveBufferSize, ReceiveDataCallback, null);
            }
            catch (Exception)
            {
                CloseConnection();
                throw;
            }
        }

        private void CloseConnection()
        {
            Console.WriteLine("Connection from {0} got terminated", ip);
            socket.Close();
            socket = null;
        }
    }
}
