# Quentin Buache 23.05.13
# Met la machine dans le groupe WORKGROUP

$sysInfo = Get-WmiObject -Class Win32_ComputerSystem
$sysInfo.JoinDomainOrWorkgroup("WORKGROUP") 
#$sysInfo.Workgroup