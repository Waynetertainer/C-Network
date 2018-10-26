using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Client.Scripts
{
    public class ClientTCP : MonoBehaviour
    {
        public static ClientTCP instance;

        public TcpClient client;
        public NetworkStream myStream;
        private byte[] asyncBuffer;
        public bool isConnected;

        public byte[] receivedBytes;
        public bool handleData = false;

        private string IP_ADDRESS = "10.1.4.255";
        private IPAddress IP_Address = IPAddress.Parse("10.1.4.55");
        private int PORT = 2711;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (handleData)
            {
                ClientHandlePackets.HandleData(receivedBytes);
                handleData = false;
            }
        }

        public void Connect()
        {
            Debug.Log("Trying to connect to the server");
            client=new TcpClient();
            client.ReceiveBufferSize = 4096;
            client.SendBufferSize = 4096;
            asyncBuffer= new byte[8192];
            try
            {
                Debug.Log("Trying to connect");
                client.BeginConnect(IP_Address, PORT, new AsyncCallback(ConnectCallback), client);
                Debug.Log("Connected");
            }
            catch (Exception)
            {
                Debug.Log("Unable to connect to the server");
                throw;
            }
        }

        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                client.EndConnect(result);
                if (client.Connected == false)
                {
                    return;
                }
                else
                {
                    myStream = client.GetStream();
                    myStream.BeginRead(asyncBuffer, 0, 8192, OnReceiveData, null);
                    isConnected = true;
                    Debug.Log("You are connected to the server succesfully");
                }
            }
            catch (Exception)
            {
                isConnected = false;
                throw;
            }
        }

        private void OnReceiveData(IAsyncResult result)
        {
            try
            {
                int packetLength = myStream.EndRead(result);
                receivedBytes = new byte[packetLength];
                Buffer.BlockCopy(asyncBuffer,0,receivedBytes,0,packetLength);

                if (packetLength == 0)
                {
                    Debug.Log("Disconnected");
                    Application.Quit();
                    return;
                }

                handleData = true;
                myStream.BeginRead(asyncBuffer, 0, 8192, OnReceiveData, null);


            }
            catch (Exception)
            {
                Debug.Log("Disconnected");
                Application.Quit();
                return;
            }
        }

        public void SendData(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((data.GetUpperBound(0) - data.GetLowerBound(0)) + 1);
            buffer.WriteBytes(data);
            myStream.Write(buffer.ToArray(),0,buffer.ToArray().Length);
        }

        public void SEND_THANKYOU()
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteLong((long)ClientPackets.C_THANKYOU);
            buffer.WriteString("Thank you, server");
            SendData(buffer.ToArray());
        }
    }
}
