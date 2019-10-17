using System;
using System.Net.Sockets;
using System.Net;

namespace UnityServer
{
    public enum ServerPackets
    {
        S_INFORMATION =1,
        S_EXECUTEMETHODONCLIENT,
        S_PLAYERDATA,
    }

    public enum ClientPackets
    {
        C_THANKYOU = 1,
    }

    class TCPServer
    {
        public static TcpListener serverSocket;

        public static Clients[] Client = new Clients[1500];

        public void InitNetwork()
        {
            for (int i = 0; i < Client.Length; i++)
            {
                Client[i]=new Clients();
            }

            serverSocket = new TcpListener(IPAddress.Any, 2711);
            serverSocket.Start();
            serverSocket.BeginAcceptTcpClient(ClinetConnectCallback, null);
        }

        private void ClinetConnectCallback(IAsyncResult result)
        {
            TcpClient tempClient = serverSocket.EndAcceptTcpClient(result);
            serverSocket.BeginAcceptTcpClient(ClinetConnectCallback, null);
            Console.WriteLine("Player succesfully connected");


            for (int i = 0; i < Client.Length; i++)
            {
                if (Client[i].socket == null)
                {
                    Client[i].socket = tempClient;
                    Client[i].connectionID = i;
                    Client[i].ip = tempClient.Client.RemoteEndPoint.ToString();
                    Client[i].Start();
                    Console.WriteLine("Connection received from "+Client[i].ip);
                    SendInformation(Client[i].connectionID);
                    //SendExecuteMethodOnClient(Client[i].connectionID);
                    SendJoinGame(i);
                    return;
                }
            }
        }

        public void SendDataTo(int connectionID, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((data.GetUpperBound(0) - data.GetLowerBound(0))+1);
            buffer.WriteBytes(data);
            Client[connectionID].myStream.BeginWrite(buffer.ToArray(), 0, buffer.ToArray().Length, null, null);
        }

        public void SendDataToAll(byte[] data)
        {
            for (int i = 0; i < Client.Length; i++)
            {
                if (Client[i].socket != null)
                {
                    SendDataTo(i, data);
                }
            }
        }

        public void SendInformation(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();

            buffer.WriteLong((long)ServerPackets.S_INFORMATION);
            buffer.WriteString("Welcome to the server");
            buffer.WriteString("Now you are able to play the game");
            buffer.WriteInteger(10);

            SendDataTo(connectionID,buffer.ToArray());
        }

        public void SendExecuteMethodOnClient(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.S_EXECUTEMETHODONCLIENT);

            SendDataTo(connectionID, buffer.ToArray());
        }

        public byte[] PlayerData(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ServerPackets.S_PLAYERDATA);
            buffer.WriteInteger(connectionID);
            return buffer.ToArray();
        }

        public void SendJoinGame(int connectionID)
        {
            ByteBuffer buffer = new ByteBuffer();
            //send other playerdata to new player
            for (int i = 0; i < Client.Length; i++)
            {
                if (Client[i].socket != null)
                {
                    if (i != connectionID)
                    {
                        SendDataTo(connectionID,PlayerData(i));
                    }
                }
            }

            //send new player data to everyone (including self)
            SendDataToAll(PlayerData(connectionID));
        }
    }
}
