using System;
using System.Management.Automation;  // add powershell directive
using System.Threading;
using System.Threading.Tasks;
using PowerScan.NBTScanner;
using System.Collections.Generic;
using System.Linq;
using PowerScan.PingScanner;

namespace PowerScan
{
    [Cmdlet(VerbsLifecycle.Invoke, "PingScan")] //Add this PowerShell cmdlet name
    [OutputType(typeof(PingScanner.PingResponse))]
    public class Invoke_PingScan : Cmdlet
    {
        [Parameter(ValueFromPipeline = true, Position = 0)]
        public string[] ComputerName { get; set; } // incomming computername

        [Parameter]
        public int Timeout { get; set; } = 4000;
        [Parameter]
        public int MaxThreads { get; set; } = 4;
        [Parameter]
        public int MinHostGroup { get; set; } = 50;
        [Parameter]
        public bool NoDNSLookup {get;set;} = false;


        protected override void BeginProcessing() //Add this Begin function method
        {
            //Add validation codes
        }

        protected override void ProcessRecord() //Add this Process function method
        {

            if (MinHostGroup < 1)
                MinHostGroup = 1;

            MaxThreads = Math.Min(ComputerName.Length, MaxThreads);

            if (MaxThreads == 1) // if only 1 host or 1 thread max call pinscanasync on main thread
            {
                for (int i = 0; i < ComputerName.Length; i += MinHostGroup)
                {
                    WriteVerbose("Starting Ping Scan...");
                    List<PingScanner.PingResponse> plist = PingScanner.PingScan.PingScanAsync(ComputerName.Skip(i).Take(MinHostGroup).ToArray(), Timeout, NoDNSLookup).Result;
                    WriteVerbose("Writing objects now...");
                    foreach (var item in plist)
                    {
                        WriteObject(item);
                    }
                }
                return;
            }

            WriteVerbose(string.Format("MaxThreads:    {0}", MaxThreads));
            WriteVerbose(string.Format("MinHostGroup:  {0}", MinHostGroup));

            Thread[] ta = new Thread[MaxThreads];
            object[] pra = new object[MaxThreads];
            int[] d = new int[MaxThreads];

            for (int i = 0; i < MaxThreads; i++)
            {
                pra[i] = null;
            }

            for (int i = 0; i < MaxThreads; i++)
            {
                // Console.WriteLine("I:{0}",i);
                int j = i;
                ta[j] = new Thread(delegate ()
                {
                    // Console.WriteLine("ComputerName[ComputerSegment*j]: {0}", ComputerName[ComputerSegment * j]);
                    // Console.WriteLine("ComputerSgment: {0}    j: {1}", ComputerSegment, j);
                    // Console.WriteLine(j);
                    pra[j] = PingScan.PingScanAsync(ComputerName.Skip(MinHostGroup * j).Take(MinHostGroup).ToArray(), Timeout,NoDNSLookup).Result;

                });
                ta[i].Start();
            }

            for (int i = 0; i < MaxThreads; i++)
            {
                // ta[i].Start();
            }
            for (int i = 0; i < MaxThreads; i++)
            {
                ta[i].Join();
            }

            for (int i = 0; i < MaxThreads; i++)
            {
                List<PingResponse> pr = (List<PingResponse>)pra[i];
                // Console.WriteLine(pr.ElementAt(0).Destination);
                for (int j = 0; j < pr.Count(); j++)
                    WriteObject(pr.ElementAt(j));
            }




        }
        protected override void EndProcessing() //Add this End function method
        {
            //Add ending output codes
        }
    }
}