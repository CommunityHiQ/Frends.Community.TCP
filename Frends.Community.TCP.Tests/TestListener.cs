using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
                    client.NoDelay = true;

                    data = null;

                    NetworkStream stream = client.GetStream();

                    //msgStart should be ignored by the task. Only responses to commands should be returned.
                    byte[] msgStart = System.Text.Encoding.ASCII.GetBytes("Message on listener launch");
                    stream.Write(msgStart, 0, msgStart.Length);

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {

                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        
                        data = data.ToUpper();

                        if (data.Equals("STOP"))
                            break;
                        else if (data.Equals("SEND_EMPTY_RESP"))
                        {
                            byte[] msgEmpty = System.Text.Encoding.ASCII.GetBytes("");
                            stream.Write(msgEmpty, 0, msgEmpty.Length);
                        }

                        else
                        {
                            byte[] msg1 = System.Text.Encoding.ASCII.GetBytes("...<" + data + "Response");
                            byte[] msg2 = System.Text.Encoding.ASCII.GetBytes("_ResponseContinues>...");
                            byte[] msg3 = System.Text.Encoding.ASCII.GetBytes("_this should be discarded only for command1 response");
                            
                            stream.Write(msg1, 0, msg1.Length);
                            Thread.Sleep(1000);
                            stream.Write(msg2, 0, msg2.Length);
                            Thread.Sleep(1000);
                            stream.Write(msg3, 0, msg3.Length);

                        }


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
