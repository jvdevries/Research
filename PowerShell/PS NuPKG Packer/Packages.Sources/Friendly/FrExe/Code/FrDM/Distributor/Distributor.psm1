#################### Filters Debug message and passes them on to Consumter Components when appropriate.

# Sends a Debug Message to Display components if the component Debug Level >= Treshold Debug level.
function _FrDM_D_Publish {
	$private:functionName = "_FrDM_D_Publish"

	# Use the non-tracing or it will loop!
	$private:goodParam = _FrP_CheckUntraced $args $private:functionName ([int]) ([string]) ([int])
	if ($private:goodParam -eq 0) { return }

	$private:tlvl = $args[0]
	$private:tsub = $args[2]
	$private:lvl = FrDM_Host_GetLevel
	$private:sub = FrDM_Host_GetSubscription
	$private:slvl = FrDM_Store_GetLevel
	$private:ssub = FrDM_Store_GetSubscription

	if (($private:lvl -ge $private:tlvl) -and ($private:sub -eq $private:tsub)) {
		_FrDM_Host_Supply $args[1]
	}

	if (($private:slvl -ge $private:tlvl) -and ($private:ssub -eq $private:tsub)) {
		_FrDM_Store_Supply $args[1]
	}
}