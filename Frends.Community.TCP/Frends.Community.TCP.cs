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
                    using (var writer = new StreamWriter(stream))
                    using (var reader = new StreamReader(stream))
                    {

                        stream.ReadTimeout = options.Timeout;

                        // Translate the passed message into ASCII and store it as a Byte array.

                        foreach (var cmd in input.Command)
                        {
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(cmd);

                            await stream.WriteAsync(data, 0, data.Length);

                            data = new Byte[256];

                            Int32 bytes = await stream.ReadAsync(data, 0, data.Length);
                            string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                            output.Responses.Add(responseData);

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
