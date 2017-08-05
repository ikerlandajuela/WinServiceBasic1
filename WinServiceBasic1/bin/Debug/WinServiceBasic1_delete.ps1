# sc delete MyNewService
$serviceName = "MyNewService"
$serviceToRemove = Get-WmiObject -Class Win32_Service -Filter "name='$serviceName'"
$serviceToRemove.delete()


