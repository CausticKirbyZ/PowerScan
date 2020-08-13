
if (Test-Path .\bin) # had some issues with these folders moving from computer to computer... easiest to delete them when comipling
{Remove-Item -Recurse -Force .\bin }
if(Test-Path .\obj)
{Remove-Item -Recurse -Force .\obj}


dotnet publish -c debug

