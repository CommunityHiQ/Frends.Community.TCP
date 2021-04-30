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
        public static async Task<dynamic> ASCIIRequest(Parameters input, Options options, CancellationToken cancellationToken)
        {

            var output = new Result
            {
                Responses = new JArray()
            };

            if (options.Timeout.Equals(null))
                options.Timeout = 60000;


            using (TcpClient client = new TcpClient())
            {
                IPAddress ip = IPAddress.Parse(input.IpAddress);

                var cancelTask = Task.Delay(options.Timeout);
                var connectTask = client.ConnectAsync(ip, input.Port);

                //double await so if cancelTask throws exception, this throws it
                await await Task.WhenAny(connectTask, cancelTask);

                if (cancelTask.IsCompleted)
                {
                    //If cancelTask and connectTask both finish at the same time,
                    //we'll consider it to be a timeout. 
                    throw new TimeoutException("Timed out");
                }

                using (NetworkStream stream = client.GetStream())

                {

                    foreach (var cmd in input.Commands)
                    {
                        Byte[] dataIn = System.Text.Encoding.ASCII.GetBytes(cmd.CommandString);

                        await stream.WriteAsync(dataIn, 0, dataIn.Length, cancellationToken);

                        Thread.Sleep(1000);

                        int timeout = options.Timeout;
                        Task<string> task = Read(stream, cancellationToken, cmd.ResponseStart, cmd.ResponseEnd);

                        if (task.Wait(timeout, cancellationToken))
                        {
                            await task;
                            output.Responses.Add(task.Result);
                        }
                        else

                            throw new TimeoutException();
                    }

                    // Close everything.
                    stream.Close();
                    client.Close();
                }
            }


            return output;
        }

        private static async Task<string> Read(NetworkStream stream, CancellationToken cancellationToken, string start = "", string end = "")
        {
            string result = "";

            while (true)
            {

                Byte[] dataOut = new Byte[8192];
                Int32 bytes = await stream.ReadAsync(dataOut, 0, dataOut.Length, cancellationToken);
                string responseData = System.Text.Encoding.ASCII.GetString(dataOut, 0, bytes);
                
                result += responseData;

                if (result != "")
                {

                    if (start == "" && end == "")
                        return result;

                    if (end == "" && result.Contains(start))
                        return result.Substring(result.IndexOf(start));

                    if (start == "" && result.Contains(end))
                        return result.Substring(0, (result.IndexOf(end) + end.Length));

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
    }
}
