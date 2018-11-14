1) Put "NuSpec Sample\.nuspec.psm1" as ".nuspec" in the .CSProj directory.
2) Compile a Release build in Visual Studio.
3) In "Package Manager Console" enter "Import-Module '.\NuPKGPacker.psm1' -Force" (in the right directory).
4) Run "CreateNuPKG" and follow the instructions.