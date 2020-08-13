# PowerScan
A C# powershell module for network scanning.

# Usage
Import-Module ./powerscan.dll

Invoke-PingScan [[-ComputerName] <computernames>] [-Timeout \<valuein_ms>] [-MaxThreads \<MaxThreads>] [-MinHostGroup \<minimum_computers_to_scan_at_once>]
    [-NoDNSLookup \<bool\>] [\<CommonParameters>]

Invoke-PingScan -Computername Host1.domain.com -Timeout 1000

# How
Invoke-PingScan is an asynchronus ping sweep.
DNS hostname are supported. However using dns can slow a scan down tremendously.  
Fix this by only using ip addresses and the flag -NoDNSLookup $true

If -max-threads \<threadcount\> is specified then this will make it a multithreaded asynchronus ping scan. 

Pipe values are supported.

Powershell arrays are recomended if you have multiple hosts to try to ping. 
Ex. 
> "Invoke-PingScan -Computername $computers" will load the entire array and itterate over that. 

> "echo $computers | invoke-pingscan " will still work but will run once for every computer in $computers.




# Why
The default timeout on a icmp packet is 4 seconds. However most devices respond within 1000ms or less i like to use 100ms. If you need to scan a /24 address but not everything is there you will be waiting 4 * number of unresponsive address seconds. So i needed a ping command with a timeout parameter available.

I kept writing scripts that would go something like 

> if ( ping -c 1 device ) { do something }

ping -c 1 keeps timing out and it slows the script to a crawl

tl;dr  
I wanted a faster ping command with more options.



## But Powershell has a ForEach -Parallel option. 
yes it does. in powershell 6/7 which are not always installed on a windows machine. 

## Why write your own and not use something more robust like nmap?

Because this is 1 dll that i can load into memory and use quickly in powershell.

nmap has too many dependencies.



# But nmap is a better port scanner
yes.... everyone knows this.

But this is lighter weight and is designed for powershell consumption from the start without the hastle of the xml ingestion. If you need a quick ping scanner for a subnet this works well. 

# ToDo
* > better documentation
* > Implement the multithreaded portion better
* > Implement more features
    * NBTScanning is already started but need to tidy it up
    * maybe something with dns... idk
* > maybe ports or something.
* > rename commands to be more appropriate

# Author
CausticKirbyZ

# Source
https://github.com/CausticKirbyZ/PowerScan
