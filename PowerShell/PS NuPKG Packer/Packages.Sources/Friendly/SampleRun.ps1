Import-Module .\Friendly.psm1 -Force
$x = FrDM_Host_SetLevel 10000											# Host consumes DM's with lvl < 10000
$x = FrDM_Host_SetSubscription $global:__FrDM_Creator__Writer			# sent from the Writer Creator.
$x = FrDM_Store_SetLevel 10000											# Store consumes DM's with lvl < 10000
$x = FrDM_Store_SetSubscription $global:__FrDM_Creator__TracerWriter	# sent from the Tracer & Writer Creator.

FrW_RunUnit { ls (Join-Path $env:ProgramFiles ModemLogs) } $function:FrW_Check_StringNotEmpty { FrW_Error_Host "ModelLogs is empty or could not be read." } ([ref] ($private:o = 0))

Write-Host "-------------------"
FrDM_Store_Print
