#################### Heavyweight Work-Unit Usage Example (only FrW_RunUnit is needed for non-local tracing)
# + Tracing of the Work-Unit execution.
# + Tracing of the function itself.
# + Safe execution of Work-Units.
# + Strict Parameter checking.
function ConnectToDB
{
	$private:maxDBConnectAttempts = $args[0]
	$private:functionName = "ConnectToDB"
	$private:treshold = $global:ConnectToDB__Treshold

	FrDM_Trace_Enter $private:treshold $private:functionName $args # Traces function entering & arguments.

	$private:goodParam = FrP_Check $args $private:functionName ([int]) # Checks arguments against defined types (here [int]).
	if ($private:goodParam -eq 0) { FrTrace_Exit $private:treshold $private:functionName 0; return 0 } # See below*.

	for ($private:i=0; $private:i -lt $private:maxDBConnectAttempts; $private:i++)
	{
		$private:DBConnection = $null
		$private:ErrorOccured = 0
		$usedInConnetStatements = "SQLite" # Scope has to be script!
		$private:DBConnection = FrW_RunUnit { Connect Statements } $function:FrW_Check_NotNull { FrW_Error_Host "DBCon failed" } ([ref] ($private:ErrorOccured))
		# ^ Final Result (the database connection if no error occurs).
		#                                     ^ Statements to execute safely.
		#                                                            ^ Checks that final statement executed != $null (calls Error function otherwise)
		#                                                                                 ^ Error Function (Writes message to host & Exits)
		#                                                                                                                           ^ Error status flag

		if ($private:errorOccured -eq 0)
		{
			FrDM_Trace_Exit $private:treshold $private:functionName 1 # See below*.
			return 1
		}
	}

	# Writes additional information to the Debugger.
	FrDM_Writer_Write ($private:treshold + 1) "$private:functionName failed to connect in $private:maxDBConnectAttempts attempts. Last error: $private:Error"

	FrDM_Trace_Exit $private:treshold $private:functionName 0 # Traces function leaving & the result*.
	return 0
}
