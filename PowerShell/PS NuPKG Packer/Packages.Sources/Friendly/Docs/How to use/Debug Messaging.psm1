#################### Debug Messaging Usage Example
# Debug Messages = DM

$x = FrDM_Host_SetLevel 10000											# Host consumes DM's with lvl < 10000
$x = FrDM_Host_SetSubscription $global:__FrDM_Creator__Writer			# sent from the Writer Creator.
$x = FrDM_Store_SetLevel 10000											# Store consumes DM's with lvl < 10000
$x = FrDM_Store_SetSubscription $global:__FrDM_Creator__TracerWriter		# sent from the Tracer & Writer Creator.

FrW_RunUnit { ls (Join-Path $env:ProgramFiles ModemLogs) } $function:FrW_Check_StringNotEmpty { FrW_Error_Host "ModelLogs is empty or could not be read." } ([ref] ($private:o = 0))

Write-Host "-------------------"
FrDM_Store_Print

##### Output
# FrW_RunUnit going to run execute
# FrW_RunUnit execute result: 
# FrW_RunUnit execute error occured
# FrW_RunUnit going to run error
# ModelLogs is empty or could not be read.
#-------------------
# / FrW_RunUnit
# |  P: ls (Join-Path $env:ProgramFiles ModemLogs) @ scriptblock
# |  P: FrW_Check_StringNotEmpty @ scriptblock
# |  P: FrW_Error_Host "ModelLogs is empty or could not be read." @ scriptblock
# |  P: System.Management.Automation.PSReference`1[System.Int32]
# | DBG: FrW_RunUnit going to run execute
#    / FrW_Execute
#    |  P: ls (Join-Path $env:ProgramFiles ModemLogs) @ scriptblock
#    |  P: 
#    |  P: System.Management.Automation.PSReference`1[System.Int32]
#    \ #
# | DBG: FrW_RunUnit execute result: 
# | DBG: FrW_RunUnit execute error occured
# | DBG: FrW_RunUnit going to run error
#    / FrW_Execute
#    |  P: FrW_Error_Host "ModelLogs is empty or could not be read." @ scriptblock
#    |  P: 
#    |  P: 
#    \ #
# \ #

##### In Settings
# $global:__FrDM_Creator__Writer = 1
# $global:__FrDM_Creator__Tracer = 2
# $global:__FrDM_Creator__TracerWriter = 3

# $global:__FrP_Check__Treshold = 1000
# $global:__FrP_CheckOrExit__Treshold = 1000
# $global:__FrW_Execute__Treshold = 1100
# $global:__FrW_RunUnit__Treshold = 1000