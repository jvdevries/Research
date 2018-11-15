#################### Standardized Check functions for FrW_RunUnit

# TResult Func<in T,out TResult>(T arg)

Function FrW_Check_NotNull
{
	$args[0] -ne $null
}

Function FrW_Check_StringNotEmpty
{
	if ($args[0] -eq '') { return 0 }

	return 1
}

Function FrW_Check_Is0Or1
{
	(($args[0] -eq 1) -or ($args[0] -eq 0))
}