using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer
{
    class ServerHandlePackets
    {
        private delegate void Packet_(long connectionID, byte[] data);

        private static Dictionary<long, Packet_> packets;
        private static long pLength;

        public static void InitializePackets()
        {
            packets = new Dictionary<long, Packet_>();
            packets.Add((long)ClientPackets.C_THANKYOU, PACKET_THANKYOU);
        }

        public static void HandleData(long connectionID, byte[] data)
        {
            byte[] Buffer;
            Buffer = (byte[])data.Clone();

            if (TCPServer.Client[connectionID].Buffer == null)
            {
                TCPServer.Client[connectionID].Buffer = new ByteBuffer();
            }
            TCPServer.Client[connectionID].Buffer.WriteBytes(Buffer);

            if (TCPServer.Client[connectionID].Buffer.Count() == 0)
            {
                TCPServer.Client[connectionID].Buffer.Clear();
                return;
            }

            if (TCPServer.Client[connectionID].Buffer.Length() >= 4)
            {
                pLength = TCPServer.Client[connectionID].Buffer.ReadLong(false);

                if (pLength <= 0)
                {
                    TCPServer.Client[connectionID].Buffer.Clear();
                    return;
                }
            }

            while (pLength > 0 & pLength <= TCPServer.Client[connectionID].Buffer.Length() - 8)
            {
                if (pLength <= TCPServer.Client[connectionID].Buffer.Length() - 8)
                {
                    TCPServer.Client[connectionID].Buffer.ReadLong();
                    data = TCPServer.Client[connectionID].Buffer.ReadBytes((int)pLength);
                    HandleDataPackets(connectionID, data);
                }

                pLength = 0;
                if (TCPServer.Client[connectionID].Buffer.Length() >= 4)
                {
                    pLength = TCPServer.Client[connectionID].Buffer.ReadLong(false);

                    if (pLength <= 0)
                    {
                        TCPServer.Client[connectionID].Buffer.Clear();
                        return;
                    }
                }

                if (pLength <= 1)
                {
                    TCPServer.Client[connectionID].Buffer.Clear();
                }
            }
        }

        private static void HandleDataPackets(long connectionID, byte[] data)
        {
            long packetIdentifier;
            ByteBuffer buffer;
            Packet_ packet;

            buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            packetIdentifier = buffer.ReadLong();
            buffer.Dispose();

            if (packets.TryGetValue(packetIdentifier, out packet))
            {
                packet.Invoke(connectionID,data);
            }
        }

        private static void PACKET_THANKYOU(long connectionID, byte[]data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            long packetIdentifier = buffer.ReadLong();
            string msg = buffer.ReadString();

            Console.WriteLine(msg);
        }
    }
}
