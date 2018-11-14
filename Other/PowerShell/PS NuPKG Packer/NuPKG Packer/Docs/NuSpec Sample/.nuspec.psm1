<?xml version="1.0"?>
<!-- THIS .NUSPEC BELONGS IN A CSPROJ DIRECTORY -->
<package>
  <metadata>
    <id>NuPGKMaker</id>
    <version>1.0.0</version>
    <!-- Keep [digits].[digits].[digits] pattern for script (last digits is used by script) -->
    <title>For quickly creating and publishing a NuPKG.</title>
    <authors>.</authors>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <description>A Script and NuSpec sample for quickly creating and publishing a NuPKG. Comes with instructions.</description>
    <dependencies>
      <dependency id="NuGet.CommandLine" version="4.7.1"/>
    </dependencies>
  </metadata>
  <files>
    <file src="NuPKG Packer\**" target="content\Packages.Sources\NuPKG Packer" />
    <!-- Note the content prefix in target! -->
  </files>
</package>
