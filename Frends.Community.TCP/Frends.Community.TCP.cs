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

            if (options.Timeout==0)
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
                                    //await Task.Delay(1000);
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
                    
                    if (responseData != "")
                    {
                        int startIx = 0;
                        if (start != "" && responseData.Contains(start))
                            startIx = responseData.IndexOf(start);
                            
                        int endIx = responseData.Length-1;
                        if (end != "" && responseData.Contains(end))
                            endIx = responseData.IndexOf(end);

                        int length = endIx - startIx + 1;

                        string subStr = responseData.Substring(startIx, length);

                        result += subStr;

                        if (responseData.Contains(end))
                            break;
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;

        }

    }
}
