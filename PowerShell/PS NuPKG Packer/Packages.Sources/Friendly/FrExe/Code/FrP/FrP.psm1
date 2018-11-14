#################### Checks parameters of a Function

# Checks a Functions's Arguments against the defined Parameters.
# Example: $private:goodParam = FrP_Check $args "FrW_RunUnit" ([scriptblock]) ([scriptblock]) ([scriptblock]) ([ref] ($private:oInt = 0)).GetType()
#                            RECOMMENDED    MAND  MANDATORY  ____________    ____________   ____________    ________________________
#                                                                         [underlined].Count == $args.Count
function FrP_Check {
	$private:arguments = New-Object 'object[]' (2)
	$private:arguments[0] = 0
	$private:arguments[1] = $args

	Invoke-Command $function:__FrP_Check -ArgumentList $private:arguments
}

# Same as FrP_Check but exits if parameters are incorrect (best used in conjunction with Tracing options).
function FrP_CheckOrExit {
	$private:goodParam = Invoke-Command $private:function:FrP_Check -ArgumentList $args
	if ($private:goodParam -eq 0) { Exit }

	return $private:goodParam
}

# Untraced Check for anti-loop prevention.
function _FrP_CheckUntraced {
	$private:arguments = New-Object 'object[]' (2)
	$private:arguments[0] = 1
	$private:arguments[1] = $args

	Invoke-Command $function:__FrP_Check -ArgumentList $private:arguments
}

function __FrP_Check {
	$private:functionName = "FrP_Check"

	if (($args -eq $null) -or ($args.Count -ne 2)) { return 0 }

	$private:Untraced = $args[0]
	$private:originalArgs = $args[1]

	if (($private:originalArgs -eq $null) -or ($private:originalArgs.Count -lt 2)) {
		if ($private:Untraced -eq 0) {
			FrDM_Trace_Enter $global:__FrP_Check__Treshold $private:functionName $args
			FrDM_Writer_Write ($global:__FrP_Check__Treshold + 1) ("I was called ($private:functionName) without mandatory arguments")
			FrDM_Trace_Exit $global:__FrP_Check__Treshold $private:functionName 0
		}
	}

	if ($private:originalArgs[0].Count -ne ($private:originalArgs.Count - 2)) {
		if ($private:Untraced -eq 0) { 
			FrDM_Trace_Enter $global:__FrP_Check__Treshold $private:functionName $args
			FrDM_Writer_Write ($global:__FrP_Check__Treshold + 1) ("" + $private:originalArgs[2] + " called me ($private:functionName) with too few/many arguments")
			FrDM_Trace_Exit $global:__FrP_Check__Treshold $private:functionName 0
		}
		return 0
	}

	for ($private:i=0; $private:i -lt $private:originalArgs[0].Count; $private:i++) {
		$private:typesMatch = 1
		
		try {
			if ($private:originalArgs[1][$private:i].GetType().FullName -ne $args[($private:i + 2)].FullName) { 
				$private:typesMatch = 0 
			}
			$private:typesMatch = 1
		}
		catch { 
			$private:typesMatch = 0 
		}

		if ($private:typesMatch -eq 0) {
			if ($private:Untraced -eq 0) { 
				FrDM_Trace_Enter $global:__FrP_Check__Treshold $private:functionName $args
				FrDM_Writer_Write ($global:__FrP_Check__Treshold + 1) ("" + $private:originalArgs[2] + " called me ($private:functionName) with an incorrect argument-type")
				FrDM_Trace_Exit $global:__FrP_Check__Treshold $private:functionName 0
			}
			return 0
		}
	}

	return 1
}