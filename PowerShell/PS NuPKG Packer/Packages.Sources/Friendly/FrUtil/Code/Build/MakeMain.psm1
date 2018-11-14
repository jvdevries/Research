# Collects all PSM1 files (under inputted folder) and creates a Main.psm1.
Function MakeMain
{
	$private:rootPath = Join-Path $args[0] . -Resolve

	$private:testFileContent = [System.Text.StringBuilder]::new()
	$private:testFileLocation = Join-Path $private:rootPath Main.psm1

	$private:modulesFiles = ls $private:rootPath *.psm1 -Recurse | select -Exp FullName
	foreach ($private:moduleFile in $private:modulesFiles)
	{
		$private:moduleFileAfterRoot = $private:moduleFile.Substring($private:rootPath.Length)
		$private:testFileContent.AppendLine("Import-Module (Join-Path `$PSScriptRoot `"$private:moduleFileAfterRoot`" -Resolve) -Force") | Out-Null
	}

	$private:testFileContent.ToString() | Out-File -filepath $private:testFileLocation | Out-Null
}