#################### Consumes Debug Messages by writing them to a Store.

function FrDM_Store_GetLevel {
	return $global:_FrDM_Store_Level
}

function FrDM_Store_SetLevel {
	$private:functionName = "FrDM_Host_SetLevel"

	$private:goodParam = FrP_Check $args $private:functionName ([int])
	if ($private:goodParam -eq 0) { 
		FrDM_Trace_Enter ($private:_FrDM_Host_Threshold + 1) $private:functionName $args
		FrDM_Trace_Exit ($private:_FrDM_Host_Threshold + 1) $private:functionName 0
		return 0
	}
		
	$global:_FrDM_Store_Level = $args[0]

	return 1
}

function FrDM_Store_GetSubscription {
	return $global:_FrDM_Store_Subscription
}

function FrDM_Store_SetSubscription {
	$private:functionName = "FrDM_Store_SetSubscription"

	$private:goodParam = FrP_Check $args $private:functionName ([int])
	if ($private:goodParam -eq 0) {
		FrDM_Trace_Enter ($private:_FrDM_Host_Threshold + 1) $private:functionName $args
		FrDM_Trace_Exit ($private:_FrDM_Host_Threshold + 1) $private:functionName 0;
		Exit
		return 0 
	}
		
	$global:_FrDM_Store_Subscription = $args[0]

	return 1
}

function _FrDM_Store_Supply {
	$private:functionName = "_FrDM_Store_Supply"
	
	$private:goodParam = FrP_Check $args $private:functionName ([string])
	if ($private:goodParam -eq 0) { return }
		
	$global:_FrDM_Store_Collection.Add($args[0]) | Out-Null
}

function FrDM_Store_Get {
	return $global:_FrDM_Store_Collection
}

function FrDM_Store_Print {
	$global:_FrDM_Store_Collection | % { Write-Host $_ }
}

function FrDM_Store_Clear {
	$global:_FrDM_Store_Collection = New-Object 'System.Collections.ArrayList'
}


