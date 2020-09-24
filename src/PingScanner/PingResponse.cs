using System;
using System.Net;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PowerScan.PingScanner
{
    public class PingResponse
    {
        public string Target { get; set; } = null;
        public bool Pings { get; set; } = false;
        public int Timeout { get; set; }
        //public int bytes {get;set;}
        public IPAddress Destination { get; set; } = null;
        public long RoundtripTime { get; set; }
        public int ttl { get; set; }
        public int bytes { get; set; }

        // public PingResponse(PingReply pr)
        // {

        // }
    }
}