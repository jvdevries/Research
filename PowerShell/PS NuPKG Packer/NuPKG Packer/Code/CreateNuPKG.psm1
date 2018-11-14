Function CreateNuPKG {
	Function GetNuGetSourcesOrExit {
		return FrW_RunUnit { NuGet.exe sources } $function:FrW_Check_NotNull { FrW_Error_HostExit "ERROR: Could not get NuGet Sources (try again later)." } ([ref] ($private:errorOccured))
	}

	Function GetNuSpecOrExit {
		$private:NuSpec = FrW_RunUnit { get-content .nuspec } $function:FrW_Check_NotNull { FrW_Error_HostExit "ERROR: .nuspec not found in ((pwd).Path) (does the file exist?)" } ([ref] ($private:errorOccured))
	}

	Function GetNuGetRepositoryOrExit {
		$__repositoryInputted = $args[0]
		
		$private:nugetsources = GetNuGetSourcesOrExit
		
		$__repositoryContext = FrW_RunUnit { NuGet.exe sources | select-string $__repositoryInputted -context 0,1 } $function:FrW_Check_NotNull { FrW_Error_HostExit "ERROR: Could not get a matching repository context from NuGet Sources for $__repositoryInputted (is the repository name correct?)." } ([ref] ($private:errorOccured))

		$private:repository = FrW_RunUnit { $__repositoryContext.context.PostContext.trim() } $function:FrW_Check_NotNull { FrW_Error_HostExit "ERROR: Could not get a matching repository from NuGet Sources for $__repositoryInputted (is the repository name correct?)." } ([ref] ($private:errorOccured))

		return $private:repository
	}

	Function UpdateNuSpecOrExit	{
		# Get NuSpec
		$__NuSpec = FrW_RunUnit { get-content .nuspec } $function:FrW_Check_NotNull { FrW_Error_HostExit "ERROR: .nuspec not found in ((pwd).Path) (does the file exist?)" } ([ref] ($private:errorOccured))
		
		# Get NuSpec version
		$__NuSpecVersion = FrW_RunUnit { [RegEx]::Match($__NuSpec, "(?<=<version>)([0-9]*).([0-9]*).([0-9]*)(?=<\/version>)") } $function:FrW_Check_NotNull { FrW_Error_HostExit "ERROR: Regex Failed to extract version from .nuspec (is the 3x digit version there?)" } ([ref] ($private:errorOccured))
		
		# Increase NuSpec version
		$__newNuSpecVersion = FrW_RunUnit { $__NuSpecVersion.Captures.Groups[1].Value + "." + $__NuSpecVersion.Captures.Groups[2].Value + "." + (([int] $__NuSpecVersion.Captures.Groups[3].Value) + 1) } $function:FrW_Check_NotNull { FrW_Error_HostExit "ERROR: Could not update version of extracted .nuspec version (are there any non-digit characters in the .nuspec version?)" } ([ref] ($private:errorOccured))

		# Create new NuSpec
		$__newNuSpec = FrW_RunUnit { $__NuSpec -replace "(?<=<version>)(.*?)(?=</version>)", $__newNuSpecVersion } $function:FrW_Check_NotNull { FrW_Error_HostExit "ERROR: Could not replace <version> in .nuspec (this is a script error)" } ([ref] ($private:errorOccured))

		# Write new NuSpec
		FrW_RunUnit { set-content .nuspec $__newNuSpec } { return $true } { FrW_Error_HostExit "ERROR: Could not write new .nuspec (did somebody lock it while the script was running?)" } ([ref] ($private:errorOccured))
	}

	Function CreatePackageOrExit
	{
		# Run NuGet Pack
		$__packLine = $null
		return FrW_RunUnit { 
			$__packResult = nuget.exe pack -IncludeReferencedProjects -properties Configuration=Release;
			$__packResult | % { 
				if ($_.StartsWith("Successfully created package")) {
					$__packLine = [RegEx]::Match($_, "(?<=\')(.*?)(?=\')"); 
					if ($__packLine.Success -eq 1) {
						return $__packLine.Groups[1].Value 
					};	
				};
			}
		}{ Test-Path $args[0] } { FrW_Error_HostExit "ERROR: NuGet could not create Package (was the Release build? If so, see `$private:Error)." } ([ref] ($private:errorOccured))
		Exit
	}

	Function DistributePackageOrExit
	{
		$__NuGetPackage = $args[0]
		$__repositoryLocation = $args[1]

		$__newNuSpec = FrW_RunUnit { nuget.exe add $__NuGetPackage -Source $__repositoryLocation } { ($args[0] | select-object -Last 1).StartsWith("Successfully added package") } { FrW_Error_HostExit "ERROR: NuGet could not add $__NuGetPackage to $__repositoryLocation" } ([ref] ($private:errorOccured))
	
		FrW_RunUnit { cp $__NuGetPackage $__repositoryLocation } { return $true } { FrW_Error_HostExit "ERROR: NuGet could not copy $__NuGetPackage to $__repositoryLocation (see `$Error)" } ([ref] ($private:errorOccured))

		FrW_RunUnit { del $__NuGetPackage } { return $true } { $null } ([ref] ($private:errorOccured))
	}

	if (($args -eq $null) -or ($args.Count -eq 0)) {
		Write-Host "Call"$MyInvocation.MyCommand"with the name of a Registered NuGet Repository, in the .CSProj directory."
		return GetNuGetSourcesOrExit
	}

	GetNuSpecOrExit

	$private:repository = GetNuGetRepositoryOrExit $args[0]

	UpdateNuSpecOrExit

	$private:NuGetPackage = CreatePackageOrExit

	DistributePackageOrExit $private:NuGetPackage $private:repository

	Write-Host "Distributed $private:NuGetPackage to $private:repository."
}