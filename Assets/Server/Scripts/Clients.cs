
using System;
using System.Net.Sockets;

class Clients
{
    public int connectionID;
    public string ip;
    public TcpClient socket;
    public NetworkStream myStream;
    private byte[] readBuffer;

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
                return;
            }
            byte[] newBytes = new byte[readbytes];
            Buffer.BlockCopy(readBuffer, 0, newBytes, 0, readbytes);
            myStream.BeginRead(readBuffer, 0, socket.ReceiveBufferSize, ReceiveDataCallback, null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

