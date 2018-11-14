#################### Consumes Debug Messages by writing them to the Host.

function FrDM_Host_Getlevel {
	return $global:_FrDM_Host_Level
}

function FrDM_Host_SetLevel {
	$private:functionName = "FrDM_Host_SetLevel"

	$private:goodParam = FrP_Check $args $private:functionName ([int])
	if ($private:goodParam -eq 0) { 
		FrDM_Trace_Enter ($private:_FrDM_Host_Threshold + 1) $private:functionName $args
		FrDM_Trace_Exit ($private:_FrDM_Host_Threshold + 1) $private:functionName 0
		return 0
	}
		
	$global:_FrDM_Host_Level = $args[0]

	return 1
}

function FrDM_Host_GetSubscription {
	return $global:_FrDM_Host_Subscription
}

function FrDM_Host_SetSubscription {
	$private:functionName = "FrDM_Host_SetSubscription"
	
	$private:goodParam = FrP_Check $args $private:functionName ([int])
	if ($private:goodParam -eq 0) {
		FrDM_Trace_Enter ($private:_FrDM_Host_Threshold + 1) $private:functionName $args
		FrDM_Trace_Exit ($private:_FrDM_Host_Threshold + 1) $private:functionName 0
		return 0 
	}
		
	$global:_FrDM_Host_Subscription = $args[0]

	return 1
}

function _FrDM_Host_Supply {
	$private:functionName = "_FrDM_Host_Supply"

	$private:goodParam = FrP_Check $args $private:functionName ([string])
	if ($private:goodParam -eq 0) { return }
		
	Write-Host $args[0]
}