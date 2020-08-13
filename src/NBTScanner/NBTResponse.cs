using System.Net;
namespace PowerScan.NBTScanner
{
    public class NBTResponse 
    {
        public bool reachable {get;set;}
        public string hostname{get;set;}
        public string domain{get;set;}
        public string service{get;set;}
        public string macAddress{get;set;}
        public IPAddress IPAddress{get;set;}
        public byte[] rawResult{get;set;}
        public int received{get;set;}
        public bool IsdomainController{get;set;}
    }
}