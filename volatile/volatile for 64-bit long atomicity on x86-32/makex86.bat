SET compiler=S:\Progs\VS2017\MSBuild\15.0\Bin\Roslyn\csc.exe
SET assemblyPath=C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\


@Echo off
setlocal enableDelayedExpansion
for /f "usebackq delims=|" %%f in (`dir /b "*.cs" ^| findstr /v AssemblyInfo.cs`) do CALL :make_program "%%~nf"
exit /b

:make_program
%compiler% /noconfig /nowarn:0420 /nostdlib+ /platform:x86 /errorreport:prompt /warn:4 /define:TRACE /errorendlocation /preferreduilang:en-US /highentropyva+ /reference:"%assemblyPath%\System.dll" /reference:"%assemblyPath%\System.Core.dll" /reference:"%assemblyPath%\mscorlib.dll" /reference:"%assemblyPath%\Microsoft.CSharp.dll" /filealign:512 /optimize+ /out:^"%~1.exe^" /target:exe /utf8output /deterministic+ /langversion:latest ^"%~1.cs^" AssemblyInfo.cs
exit /b