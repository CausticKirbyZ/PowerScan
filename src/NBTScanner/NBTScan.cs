using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerScan.NBTScanner
{
    public partial class NBTScan
    {
        public static async Task<List<NBTResponse>> RunNBTScanAsync(string[] iplist, int timeout = 1000)
        {
            var tasks = new List<Task<NBTResponse>>();
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.EnableBroadcast = true;
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            sock.Bind(new IPEndPoint(IPAddress.Any, 0));

            for (int i = 0; i < iplist.Length; i++)
            {
                var task = GetNBTResponseAsync(iplist[i],timeout);
                tasks.Add(task);
            }

            var result = await Task.WhenAll(tasks);
            sock.Close();
            return result.ToList();


        }
        public static async Task<NBTResponse> GetNBTResponseAsync(string ip, Socket sock, int Timeout = 1000)
        {
            NBTResponse response = null;
            // Task.Run(() => Console.WriteLine("hello"));
            try
            {
                EndPoint targetEndpoint = new IPEndPoint(IPAddress.Parse(ip), 137);
                response = new NBTResponse();

                sock.ReceiveTimeout = Timeout;
                sock.SendTimeout = Timeout;
                
                sock.SendTo(QueryPacket, QueryPacket.Length, SocketFlags.None, targetEndpoint);
                response.IPAddress = IPAddress.Parse(ip);
                response.rawResult = new byte[250];
                response.received = sock.ReceiveFrom(response.rawResult, ref targetEndpoint);
                response.hostname = Encoding.ASCII.GetString(response.rawResult, 57, 16).TrimEnd();
                response.domain = Encoding.ASCII.GetString(response.rawResult, 75, 16).TrimEnd();
                response.service = Encoding.ASCII.GetString(response.rawResult, 93, 16).TrimEnd();
                response.macAddress = BitConverter.ToString(response.rawResult).Replace("-", "").Substring(222, 12);
                response.IsdomainController = (response.rawResult[2] == 0x84);
                //string dcstr = String.Format("{0:X}",response.rawResult[2]); //BitConverter.ToString(message).Replace("-","").Substring(5,6);
            }
            catch (Exception e)
            {
                throw e;
            }
            sock.Close();

            return response;
        }
        public static async Task<NBTResponse> GetNBTResponseAsync(string ip, int Timeout = 1000)
        {
            NBTResponse response = await GetNBTResponseAsTask(ip,Timeout);

            return response;
        }

        public static Task<NBTResponse> GetNBTResponseAsTask(string ip, int Timeout = 1000)
        {
            return Task.Factory.StartNew(() => GetNBTResponse(ip,Timeout) );
        }

        public static NBTResponse GetNBTResponse(string ip, int Timeout=1000)
        {
            // Console.WriteLine("getNBTResponse called");
            NBTResponse response = null;
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sock.EnableBroadcast = true;
            
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            sock.Bind(new IPEndPoint(IPAddress.Any, 0));
            // Console.WriteLine("Socket Created and bound");

            try
            {
                EndPoint targetEndpoint = new IPEndPoint(IPAddress.Parse(ip), 137);
                // Console.WriteLine("Endpoint created");
                response = new NBTResponse();
                sock.ReceiveTimeout = 1000;
                response.IPAddress = IPAddress.Parse(ip);
                response.rawResult = new byte[250];
                response.received = 0;
                // Console.WriteLine("socket setup and response object created");
                sock.SendTo(QueryPacket, QueryPacket.Length, SocketFlags.None, targetEndpoint);
                // Console.WriteLine("sent packet");
                // Console.WriteLine("Socket Receive Timeout: {0}",sock.ReceiveTimeout);
                response.received = sock.ReceiveFrom(response.rawResult, ref targetEndpoint);
                // Console.WriteLine("packet received");
                response.reachable = response.received != 0;
                response.hostname = Encoding.ASCII.GetString(response.rawResult, 57, 16).TrimEnd();
                response.domain = Encoding.ASCII.GetString(response.rawResult, 75, 16).TrimEnd();
                response.service = Encoding.ASCII.GetString(response.rawResult, 93, 16).TrimEnd();
                response.macAddress = BitConverter.ToString(response.rawResult).Replace("-", "").Substring(222, 12);
                response.IsdomainController = (response.rawResult[108] == 0x1c);
                //string dcstr = String.Format("{0:X}",response.rawResult[108]); //BitConverter.ToString(message).Replace("-","").Substring(5,6);
                // Console.WriteLine(dcstr);
            }
            catch(TimeoutException e)
            {
                //Console.WriteLine("timeout");
                throw e;
            }
            catch(SocketException e)
            {
                //throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                sock.Close();
            }

            return response;
        }
    }

}