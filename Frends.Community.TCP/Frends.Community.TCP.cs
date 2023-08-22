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
            {
                options.Timeout = 60000;
            }
            
            // Timeout after the set limit
            var cancelTask = Task.Delay(options.Timeout);

            using (TcpClient client = new TcpClient()) 
            {

                IPAddress ip = IPAddress.Parse(input.IpAddress);
                var connectTask = client.ConnectAsync(ip, input.Port);

                if (await Task.WhenAny(connectTask, cancelTask) == connectTask)
                {
                    // Await in if-condition awaits WhenAny(), await the actual task in case it failed
                    await connectTask;
                    using (NetworkStream stream = client.GetStream()) 
                    {
                        foreach (var cmd in input.Commands)
                        {
                            byte[] dataIn = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(cmd.CommandString);

                            var writeTask = stream.WriteAsync(dataIn, 0, dataIn.Length, cancellationToken);
                            if (await Task.WhenAny(writeTask, cancelTask) == writeTask)
                            {   
                                await writeTask;

                                // This read loop was previously in a separate Task-method, so when the main task timed out
                                // the read task that got stuck in a loop was still running in the background
                                // Merged the tasks as a fix, another option would have been merging the timeout task with the parent CancellationToken
                                string result = "";
                                byte[] dataOut = new byte[8192]; 
                                while (true) 
                                {
                                    if (cancellationToken.IsCancellationRequested) 
                                    {
                                        break;
                                    }

                                    var readTask = stream.ReadAsync(dataOut, 0, dataOut.Length, cancellationToken);
                                    
                                    if (await Task.WhenAny(readTask, cancelTask) == readTask)
                                    {
                                        int bytes = await readTask;

                                        if (bytes == 0) 
                                        {
                                            // Server closed socket
                                            break;
                                        }

                                        string responseData = System.Text.Encoding.GetEncoding("ISO-8859-1").GetString(dataOut, 0, bytes);
                                        result += responseData;

                                        // Validate response
                                        if (cmd.ResponseStart == "" && cmd.ResponseEnd == "") 
                                        {
                                            output.Responses.Add(result);
                                            break;
                                        }
                                        else if (cmd.ResponseEnd == "" && result.Contains(cmd.ResponseStart)) 
                                        {
                                            output.Responses.Add(result.Substring(result.IndexOf(cmd.ResponseStart)));
                                            break;
                                        } 
                                        else if (cmd.ResponseStart == "" && result.Contains(cmd.ResponseEnd)) 
                                        {
                                            output.Responses.Add(result.Substring(0, result.IndexOf(cmd.ResponseEnd) + cmd.ResponseEnd.Length));
                                            break;
                                        } 
                                        else if (result.Contains(cmd.ResponseStart)) 
                                        {
                                            var index = result.IndexOf(cmd.ResponseStart);
                                            if (result.IndexOf(cmd.ResponseEnd, index) > -1) 
                                            {
                                                var length = result.IndexOf(cmd.ResponseEnd, index) - index + cmd.ResponseEnd.Length;
                                                output.Responses.Add(result.Substring(index, length));
                                                break;
                                            }
                                        }
                                    } 
                                    else 
                                    {
                                        throw new TimeoutException("Timed out");
                                    }
                                }
                            } 
                            else 
                            {
                                throw new TimeoutException("Timed out");
                            }
                        }
                    }
                } 
                else 
                {
                    throw new TimeoutException("Timed out");
                }
            }
            return output;
        }
    }
}