#################### Standardized Error functions for FrW_RunUnit

# void Action<in T>(T obj)

Function FrW_Error_HostExit
{
	Write-Host $args[0]
	exit
}

Function FrW_Error_Host
{
	Write-Host $args[0]
}


