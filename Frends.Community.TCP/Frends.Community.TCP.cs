using System;
using System.Collections.Generic;
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

            if (options.Timeout.Equals(null))
                options.Timeout = 60000;

            try
            {
                using (TcpClient client = new TcpClient())
                {
                    IPAddress ip = IPAddress.Parse(input.IpAddress);
                    await client.ConnectAsync(ip, input.Port);
                    cancellationToken.ThrowIfCancellationRequested();

                    using (NetworkStream stream = client.GetStream())

                    {

                        foreach (var cmd in input.Commands)
                        {
                            Byte[] dataIn = System.Text.Encoding.ASCII.GetBytes(cmd.CommandString);

                            await stream.WriteAsync(dataIn, 0, dataIn.Length);
                            
                            Thread.Sleep(1000);
                            cancellationToken.ThrowIfCancellationRequested();

                            int timeout = options.Timeout;
                            Task<string> task = Read(stream, cancellationToken, cmd.ResponseStart, cmd.ResponseEnd);

                            try
                            {
                                if (task.Wait(timeout, cancellationToken))
                                {
                                    await task;
                                    output.Responses.Add(task.Result);
                                }
                                else

                                    throw new TimeoutException();

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
            catch (Exception)
            {
                throw;
            }

            return output;
        }

        private static async Task<string> Read(NetworkStream stream, CancellationToken token, string start = "", string end = "")
        {
            token.ThrowIfCancellationRequested();
           
            string result = "";

            try
            {
                while (true)
                {

                    Byte[] dataOut = new Byte[8192];
                    Int32 bytes = await stream.ReadAsync(dataOut, 0, dataOut.Length);
                    string responseData = System.Text.Encoding.ASCII.GetString(dataOut, 0, bytes);
                    token.ThrowIfCancellationRequested();
                    result += responseData;

                    if (result != "")
                    {
                     
                        if (start == "" && end == "") 
                            return result;

                        if (end == "" && result.Contains(start)) 
                            return result.Substring(result.IndexOf(start));

                        if (start == "" && result.Contains(end)) 
                            return result.Substring(0,(result.IndexOf(end)+end.Length));

                        if (result.Contains(start))
                        {
                            var startIndex = result.IndexOf(start);
                            
                            if (result.IndexOf(end, startIndex) > -1)
                            {
                                var length = result.IndexOf(end, startIndex) - startIndex + end.Length;
                                return result.Substring(startIndex, length);
                            }

                        }                        

                    }

                }


            }
            catch (Exception)
            {
                throw;
            }

        }

    }
}
