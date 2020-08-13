Import-Module .\bin\Debug\netstandard2.0\PowerScan.dll
# Import-Module .\bin\Debug\net452\PowerScan.dll


$list = foreach($ip in 1..254) { echo "192.168.1.$ip"}
# $list = foreach($sub in 1..5) { foreach($ip in 1..254) { echo "192.168.$sub.$ip"}}

invoke-pingscan -computername $list -verbose -maxthreads 1 -timeout 100 -nodnslookup $true | where {$_.pings -eq $true} | ft -AutoSize