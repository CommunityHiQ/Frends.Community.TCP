using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Frends.Community.TCP.Tests
{
    class TestListener
    {
        public static void Listener()
        {
            TcpListener server = null;
            try
            {
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                server.Start();

                Byte[] bytes = new Byte[256];
                String data = null;

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();

                    data = null;

                    NetworkStream stream = client.GetStream();

                    //msgStart should be ignored by the task. Only responses to commands should be returned.
                    byte[] msgStart = System.Text.Encoding.ASCII.GetBytes(data + "Message on listener launch");
                    stream.Write(msgStart, 0, msgStart.Length);

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        data = data.ToUpper();

                        if (data.Equals("STOP"))
                            break;

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data + "Response");

                        stream.Write(msg, 0, msg.Length);

                    }

                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

        }


    }
}
