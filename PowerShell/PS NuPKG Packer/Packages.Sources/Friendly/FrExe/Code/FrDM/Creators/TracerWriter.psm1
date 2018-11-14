#################### Facilitates Debug Message Creation by providing Tracing functions (pretends to be a Tracer & Writer component).

# To be called at the beginning of a function with: Treshold FunctionName Arguments[]
function FrDM_Trace_Enter {
	$private:functionName = "FrDM_Trace_Enter"
	
	$private:goodParam = FrP_Check $args $private:functionName ([int]) ([String]) ([object[]])
	if ($private:goodParam -eq 0) { return }

	$private:spaces = _FrDM_Trace_GetSpaces
	
	_FrDM_D_Publish $args[0] ($private:spaces+"/ "+$args[1]) $global:__FrDM_Creator__Tracer
	_FrDM_D_Publish $args[0] ($private:spaces+"/ "+$args[1]) $global:__FrDM_Creator__TracerWriter

	for ($private:i=0; $private:i -lt $args[2].Count; $private:i++)	{
		
		if ($args[2][$private:i] -ne $null) {
			$private:type = " @ " + $args[2][$private:i].GetType()
			if ($private:type.ToString().Length -ge 25) { # Don't expose type if they are too long.
				$private:type = $null
			}

			_FrDM_D_Publish $args[0] ($private:spaces+"|  P: "+([string]$args[2][$private:i]).Trim() + $private:type) $global:__FrDM_Creator__Tracer
			_FrDM_D_Publish $args[0] ($private:spaces+"|  P: "+([string]$args[2][$private:i]).Trim() + $private:type) $global:__FrDM_Creator__TracerWriter
		}
		else {
			_FrDM_D_Publish $args[0] ($private:spaces+"|  P: "+([string]$args[2][$private:i]).Trim()) $global:__FrDM_Creator__Tracer
			_FrDM_D_Publish $args[0] ($private:spaces+"|  P: "+([string]$args[2][$private:i]).Trim()) $global:__FrDM_Creator__TracerWriter
		}
	}

	_FrDM_Trace_IncreaseSpaces
}

# To be called at the end of a function with: Treshold FunctionName Result
function FrDM_Trace_Exit {
	$private:functionName = "FrDM_Trace_Exit"

	$private:goodParam = _FrP_CheckUntraced $args $private:functionName ([int]) ([String]) ([object])
	if ($private:goodParam -eq 0) { return }

	_FrDM_Trace_DecreaseSpaces

	$private:spaces = _FrDM_Trace_GetSpaces
	
	if ($args[2] -eq $null) {
		_FrDM_D_Publish $args[0] ($private:spaces+"\ #") $global:__FrDM_Creator__Tracer
		_FrDM_D_Publish $args[0] ($private:spaces+"\ #") $global:__FrDM_Creator__TracerWriter
	}
	else
	{
		_FrDM_D_Publish $args[0] ($private:spaces+"\ R: " +$args[2]) $global:__FrDM_Creator__Tracer
		_FrDM_D_Publish $args[0] ($private:spaces+"\ R: " +$args[2]) $global:__FrDM_Creator__TracerWriter
	}
}

# Returns an amount of spaces for lining up Function calls in the trace.
function _FrDM_Trace_GetSpaces {
	Param([int] $private:spaceCount = $global:_FrDM_Trace_Spaces)

	$private:spaces = "  " + "".PadLeft(($private:spaceCount * 3),' ')

	return $private:spaces
}

function _FrDM_Trace_IncreaseSpaces {
	$global:_FrDM_Trace_Spaces = ($global:_FrDM_Trace_Spaces + 1)
}

function _FrDM_Trace_DecreaseSpaces {
	$global:_FrDM_Trace_Spaces = ($global:_FrDM_Trace_Spaces - 1)
}

function FrDM_Writer_Write {
	$private:functionName = "FrDM_Writer_Write"

	$private:goodParam = _FrP_CheckUntraced $args $private:functionName ([int]) ([String])
	if ($private:goodParam -eq 0) { return }

	if (-not [string]::IsNullOrEmpty($args[1]))	{
		_FrDM_D_Publish $args[0] ("DBG: "+$args[1]) $global:__FrDM_Creator__Writer # Fake Writer only Output (no |, no spaces)
		_FrDM_D_Publish $args[0] ((_FrDM_Trace_GetSpaces ($global:_FrDM_Trace_Spaces - 1))+"| DBG: "+$args[1]) $global:__FrDM_Creator__TracerWriter
	}
}