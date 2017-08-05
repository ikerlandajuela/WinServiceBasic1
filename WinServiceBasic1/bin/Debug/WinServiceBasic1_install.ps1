$serviceName = "MyNewService"
$binaryPath = "C:\bin\WinServiceBasic1.exe"
$serviceDescription = "Servicio de test WinServiceBasic1.exe"

cp WinServiceBasic1.exe c:\bin\
cp NLog.config c:\bin\
cp NLog.dll c:\bin\

if (Get-Service $serviceName -ErrorAction SilentlyContinue)
{
	# sc delete WinServiceBasic1
    $serviceToRemove = Get-WmiObject -Class Win32_Service -Filter "name='$serviceName'"
    $serviceToRemove.delete()
    "service removed"
}
else
{
    "service does not exists"
}
# Install service 
"installing service"
New-Service -name $serviceName -binaryPathName $binaryPath -displayName $serviceName -startupType Manual -Description $serviceDescription
"installation completed"
# sc query WinServiceBasic1
Get-WmiObject win32_service -Filter "name='$serviceName'"

