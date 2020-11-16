using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

#pragma warning disable 1591

namespace Frends.Community.TCP
{
    public static class TCPTasks
    {
        /// <summary>
        /// Documentation: https://github.com/CommunityHiQ/Frends.Community.TCPClient
        /// </summary>
        public static async Task<Result> ASCIIRequest(Parameters input, Options options)
        {
            var output = new Result();
            output.Responses = new JArray();

            try
            {
                using (TcpClient client = new TcpClient())
                {
                    IPAddress ip = IPAddress.Parse(input.IpAddress);
                    await client.ConnectAsync(ip, input.Port);

                    using (NetworkStream stream = client.GetStream())
                    //using (var writer = new StreamWriter(stream))
                    //using (var reader = new StreamReader(stream))
                    {

                        stream.ReadTimeout = options.Timeout;

                        // Translate the passed message into ASCII and store it as a Byte array.

                        foreach (var cmd in input.Command)
                        {

                            //Flush incoming stream

                            Byte[] dataOut = new Byte[8192];
                            if (stream.DataAvailable)                                
                              await stream.ReadAsync(dataOut, 0, dataOut.Length);

                            Byte[] dataIn = System.Text.Encoding.ASCII.GetBytes(cmd);

                            await stream.WriteAsync(dataIn, 0, dataIn.Length);

                            Thread.Sleep(100);

                            if (stream.DataAvailable)
                            {
                                Int32 bytes = await stream.ReadAsync(dataOut, 0, dataOut.Length);
                                string responseData = System.Text.Encoding.ASCII.GetString(dataOut, 0, bytes);
                                output.Responses.Add(responseData);
                            }

                        }

                        // Close everything.
                        stream.Close();
                        client.Close();
                    }
                }
            }
            catch (ArgumentNullException e)
            {
                throw e;
            }
            catch (SocketException e)
            {
                throw e;
            }

            return output;
        }

    }
}
