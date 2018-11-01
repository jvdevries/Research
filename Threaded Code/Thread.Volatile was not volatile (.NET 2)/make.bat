@Echo off
setlocal enableDelayedExpansion
for /f "usebackq delims=|" %%f in (`dir /b "*.cs" ^| findstr /v AssemblyInfo.cs`) do CALL :make_program "%%~nf"
exit /b

:make_program
C:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe /noconfig /nostdlib+ /platform:AnyCPU /reference:C:\Windows\Microsoft.NET\Framework\v2.0.50727\mscorlib.dll /reference:C:\Windows\Microsoft.NET\Framework\v2.0.50727\System.dll /filealign:512 /optimize- /out:^"%~1.exe^" /target:exe ^"%~1.cs^" AssemblyInfo.cs
exit /b