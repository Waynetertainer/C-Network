using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerHandlePackets.InitializePackets();
            TCPServer server=new TCPServer();
            server.InitNetwork();
            Console.ReadLine();
        }
    }
}
