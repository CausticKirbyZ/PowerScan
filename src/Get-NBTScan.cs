using System;
using System.Management.Automation;  // add powershell directive
using System.Net.NetworkInformation; // 
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerScan.NBTScanner;
using System.Collections.Generic;
using System.Linq;

namespace PowerScan
{
    [Cmdlet(VerbsCommon.Get, "NBTScan")] //Add this PowerShell cmdlet name
    [OutputType(typeof(NBTScanner.NBTResponse))]
    public class Get_NBTScan : Cmdlet
    {
        [Parameter(ValueFromPipeline = true, Position = 0)]
        public string[] ComputerName { get; set; } // incomming computername

        [Parameter]
        public int Timeout { get; set; } = 1000;

        protected Queue<NBTResponse> nbtque {get;set;}

        protected override void BeginProcessing() //Add this Begin function method
        {
            //Add validation codes
        }

        protected override void ProcessRecord() //Add this Process function method
        {
            //startscanner();
            foreach(var comp in ComputerName)
            {
                // Console.WriteLine("foreach comp in computer");
                // Console.WriteLine("Timeout: {0}",Timeout);
                WriteObject( NBTScan.GetNBTResponseAsync(comp,Timeout).Result);
            }
        }

        protected override void EndProcessing() //Add this End function method
        {
            //Add ending output codes
        }
        protected void startscanner()
        {
            foreach(var comp in ComputerName)
            {
                Thread t = new Thread(()=>scanhost(comp,Timeout));
                // t.Join();
            }
        }
        protected void scanhost(string ip, int timeout = 1000)
        {
            Console.WriteLine("Thread {0} is running",Thread.CurrentThread.ToString());
            var res = NBTScan.GetNBTResponse(ip,timeout);
            nbtque.Enqueue(res);
        }
    }
}