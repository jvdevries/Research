Function Get { return 2 }

Write-Host "Observe the behaviour of Get in IF:"
Write-Host -NoNewline "Get -eq 0 is: "; if (Get -eq 0) { "true" } else { "false" }						# true
Write-Host -NoNewline "Get -eq 2 is: "; if (Get -eq 2) { "true" } else { "false" }						# true
Write-Host -NoNewline "`$G = Get; `$G -eq 0 is: "; $G = Get; if ($G -eq 0) { "true" } else { "false" }	# false
Write-Host -NoNewline "`$G = Get; `$G -eq 2 is: "; $G = Get; if ($G -eq 2) { "true" } else { "false" }	# true

Write-Host "This is because of parameters are fed with spaces:"
Write-Host "(get -eq 0) returns:" (Get -eq 2)		# 2		(-eq and 2 are fed as argument for Get)
Write-Host "((get) -eq 0) returns:" ((Get) -eq 2)	# True

# Note that finding Function calls in IF blocks is easily automated via the Abstract Syntax Tree processing Powershell provides, but checking argument behaviour of chained operations is daunting.