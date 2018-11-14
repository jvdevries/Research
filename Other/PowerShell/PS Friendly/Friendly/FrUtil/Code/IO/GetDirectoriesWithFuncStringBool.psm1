Function FrU_IO_GetDirectoriesWithFuncStringBool
{
	$private:FuncStringBool = $args[1]

	$private:dirsFound = @{}
	$private:counter = 0
	$private:dirs = ls -Directory $args[0]

	$private:dirs | % { if ($private:FuncStringBool.Invoke($_.Name.ToString()) -eq 1) { $private:dirsFound.Add($private:counter++, $_) } }

	# ToDo: somehow detect the need for , in script... wrap every function in a ExecuteFunctionGetResultType & check types...
	return ,$private:dirsFound.Values
}
