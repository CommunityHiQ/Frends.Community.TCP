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
        /// Documentation: https://github.com/CommunityHiQ/Frends.Community.TCP
        /// </summary>
        public static async Task<Result> ASCIIRequest(Parameters input, Options options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var output = new Result
            {
                Responses = new JArray()
            };

            try
            {
                using (TcpClient client = new TcpClient())
                {
                    IPAddress ip = IPAddress.Parse(input.IpAddress);
                    await client.ConnectAsync(ip, input.Port);

                    using (NetworkStream stream = client.GetStream())

                    {

                        foreach (var cmd in input.Command)
                        {
                            Byte[] dataIn = System.Text.Encoding.ASCII.GetBytes(cmd);

                            await stream.WriteAsync(dataIn, 0, dataIn.Length);

                            int timeout = options.Timeout;
                            var task = ReadAsync(stream, options.ResponseStart, options.ResponseEnd);
                            try
                            {

                                if (task.Wait(timeout, cancellationToken))
                                {
                                    await task;
                                    output.Responses.Add(task.Result);
                                }
                                else
                                    throw new TimeoutException("Timeout. Successfull responses before operation timed out: " + output.Responses);

                            }
                            catch (Exception)
                            {
                                throw;
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

        public static async Task<string> ReadAsync(NetworkStream stream, string start, string end)
        {
            string result;

            while (true)
            {
                Byte[] dataOut = new Byte[8192];
                Int32 bytes = await stream.ReadAsync(dataOut, 0, dataOut.Length);
                string responseData = System.Text.Encoding.ASCII.GetString(dataOut, 0, bytes);

                string responseEnd = end ?? "";
                string responseStart = start ?? "";

                if (responseData != "")
                {

                    int startIx = 0;
                    if (responseStart != "")
                    {
                        startIx = responseData.IndexOf(responseStart);
                    };
                    int endIx = responseData.Length;
                    if (responseEnd != "")
                    {
                        endIx = responseData.IndexOf(responseEnd);
                    };

                    int length = endIx - (startIx + responseStart.Length);

                    result = responseData.Substring(startIx + responseStart.Length, length);
                    if (result!="")
                    {
                        break;                      
                    }
                }

            }

            return result;
        }

    }
}
