#################### Functions for Work-Unit execution

function FrW_RunUnit {
	function FrW_Execute {
		$private:_execute = $args[0]
		$private:_executeArguments = ,$args[1]
		$private:_errorOccured = [ref] $args[2]
		$private:errorOccured = 0

		FrDM_Trace_Enter $global:__FrW_Execute__Treshold "FrW_Execute" $args

		$private:executeResult = $null

		$private:errorsKnown = $global:Error.Count

		# Setup $ErrorActionPreference
		$private:StoredErrorActionPreference = $ErrorActionPreference
		$global:ErrorActionPreference = 'Stop'

		# Setup $PSDefaultParameterValues
		$private:StoredDefaults = $PSDefaultParameterValues.Clone()
		if ($PSDefaultParameterValues.Contains('*:ErrorAction') -eq 0) {
			$PSDefaultParameterValues += @{'*:ErrorAction' = 'Stop'}
		}
		else {
			$PSDefaultParameterValues['*:ErrorAction'] = 'Stop'
		}

		if ($private:_execute -ne $null) { # Just in case of not using this method properly
			try {
					if ($private:_executeArguments -ne $null) {
						$private:executeResult = Invoke-Command $private:_execute -ArgumentList $private:_executeArguments
					}
					else {
						$private:executeResult = Invoke-Command $private:_execute
					}
			}
			catch {
				$private:errorOccured = 1
			}
			finally {
				# Restore changes.
				$PSDefaultParameterValues = $private:StoredDefaults
				$global:ErrorActionPreference = $private:StoredErrorActionPreference
			}
	
			if (($global:Error.Count -ne $private:errorsKnown) -or ($private:errorOccured -eq 1)) {
				$private:_errorOccured.Value = 1
			}
		}

		FrDM_Trace_Exit $global:__FrW_Execute__Treshold "FrW_Execute - Error: $private:_errorOccured" $private:executeResult

		return $private:executeResult
	}

	### FrW_RunUnit Function STARTS HERE ###

	$private:functionName = "FrW_RunUnit"

	FrDM_Trace_Enter 1 $private:functionName $args

	$private:goodParam = FrP_Check $args $private:functionName ([scriptblock]) ([scriptblock]) ([scriptblock]) ([ref] ($private:o = 0)).GetType()

	$private:_execute = $args[0]
	$private:_checker = $args[1]
	$private:_onError = $args[2]
	$private:_errorOccured = $args[3]

	if ($private:goodParam -eq 0) { 
		$private:_errorOccured = 1; 
		FrDM_Trace_Exit $global:__FrW_RunUnit__Treshold $private:functionName $null
		FrW_Execute $private:_onError $null $null; 
		return $null 
	}

	$private:error_execute = 0
	FrDM_Writer_Write ($global:__FrW_RunUnit__Treshold + 2) ("$private:functionName going to run execute")
	$private:executeResult = FrW_Execute $private:_execute $null ([ref] $private:error_execute)

	FrDM_Writer_Write ($global:__FrW_RunUnit__Treshold + 1) "$private:functionName execute result: $private:executeResult"

	if ($private:error_execute -eq 0) # No Error
	{
		FrDM_Writer_Write ($global:__FrW_RunUnit__Treshold + 2) ("$private:functionName going to run check")
		$private:error_checker = 0
		$private:checkerResult = FrW_Execute $private:_checker $private:executeResult ([ref] $private:error_checker)

		FrDM_Writer_Write ($global:__FrW_RunUnit__Treshold + 1) ("$private:functionName check result: $private:checkerResult")

		if (($private:error_checker -eq 1) -or ($private:checkerResult -eq 0))
		{
			if ($private:_errorOccured -ne $null)
			{
				$private:_errorOccured.Value = 1
			}

			FrDM_Writer_Write ($global:__FrW_RunUnit__Treshold + 2) ("$private:functionName going to run error")
			FrW_Execute $private:_onError $null $null
		}
	}
	else # Error
	{
		FrDM_Writer_Write ($global:__FrW_RunUnit__Treshold + 1) ("$private:functionName execute error occured")
		if ($private:_errorOccured -ne $null)
		{
			$private:_errorOccured.Value = 1
		}

		FrDM_Writer_Write ($global:__FrW_RunUnit__Treshold + 2) ("$private:functionName going to run error")
		FrW_Execute $private:_onError $null $null
	}

	FrDM_Trace_Exit $global:__FrW_RunUnit__Treshold $private:functionName $private:executeResult

	return $private:executeResult
}