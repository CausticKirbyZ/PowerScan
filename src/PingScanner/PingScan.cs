using System;
using System.Net;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;


namespace PowerScan.PingScanner
{
    public partial class PingScan
    {
        protected IPAddress[] HOSTIP;
        public const int TIMEOUT = 4000;
        public static bool PingHost(string host, int Timeout = TIMEOUT)
        {
            bool pingable = false;
            Ping pinger = null;
            try
            {
                pinger = new Ping();
                PingReply pingReply = pinger.SendPingAsync(host, Timeout).Result;
                pingable = pingReply.Status == IPStatus.Success;
            }
            catch (PingException) { }
            catch (AggregateException e) { throw (e); }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }
            return pingable;
        }

        public static async Task<List<PingResponse>> PingScanAsync(string[] iplist, int Timeout = TIMEOUT, bool NoDNSLookup = false)
        {
           
            // Console.WriteLine("PincScanAsync iplist.length: {0}", iplist.Length);
            var tasks = new List<Task<PingResponse>>();
            // Console.WriteLine("Staring Ping tasks...");
            for (int i = 0; i < iplist.Length; i++)
            {
                var task = GetPingResponseAsync(iplist[i], Timeout, NoDNSLookup);
                tasks.Add(task);
            }
            // Console.WriteLine("awaiting tasks...");


            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }
        public static List<PingResponse>[] PingScanAsyncMthread(string[] iplist, int mthreads, int Timeout = TIMEOUT)
        {
            //Console.WriteLine("PincScanAsync Called");
            var tasktasks = new List<PingResponse>[mthreads];
            
            return tasktasks;
        }

        public static async Task<PingResponse> GetPingResponseAsync(string ip, int Timeout = TIMEOUT, bool NoDNSLookup = false)
        {
            PingResponse pr = new PingResponse();

            if( NoDNSLookup )
            {
                pr.Target = ip;
            }
            else
            {
                // pr.Target = Dns.GetHostEntryAsync(ip).Result.HostName;
                // Console.WriteLine("doing dns lookup");
                try
                {
                    // Console.WriteLine( Dns.GetHostAddresses(ip)[0].ToString()          );
                    // Console.WriteLine( Dns.GetHostEntryAsync(ip).Result.AddressList[0] );
                    // pr.Target = Dns.GetHostEntryAsync(ip).Result.AddressList[0].ToString();
                    pr.Target = Dns.GetHostEntryAsync(ip).Result.HostName.ToString();
                    // pr.Target = (await Dns.GetHostEntryAsync(ip)).HostName.ToString();
                }
                catch (Exception e)
                {
                    pr.Target = null;
                }
                
                // Console.WriteLine("got dns lookup ");
            }

            // Console.WriteLine("pr.target = {0}", pr.Target);

            if(pr.Target == null)
            {
                // Console.WriteLine("Do i even get here?");
                pr.Target = ip;
                return pr;
            }
                     
            

            try
            {
                PingReply pra = await new Ping().SendPingAsync(ip, Timeout);
                pr.Pings = pra.Status == IPStatus.Success;
                pr.RoundtripTime = pra.RoundtripTime;
                pr.Destination = pra.Address;
                pr.ttl = pra.Options.Ttl;
                pr.bytes = pra.Buffer.Length;
                pr.Timeout = Timeout;
            }
            catch (PingException) { }
            catch (System.AggregateException)
            {
                Console.WriteLine("agg except");
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.StackTrace.ToString());
                //throw (e);
            }
            // catch (AggregateException e)
            // {
            //     //throw (e); 
            // }

            return pr;
        }
        private static async Task<PingReply> sendpingasync(string ip, int Timeout = TIMEOUT)
        {

            Ping p = new Ping();
            var res = await p.SendPingAsync(ip, Timeout);
            p.Dispose();
            return res;
        }

    }
}