@echo off
del summary.txt
call :CLEAN
call :COMPILE
goto DONE

:CLEAN
del *.exe
del file-fast.txt
del file-slow.txt
del console-fast.txt
del console-slow.txt
goto DONE

:COMPILE
set COMPILEOPTIONS=/platform:x86 /optimize+ /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\System.dll" /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\Microsoft.CSharp.dll" /reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\mscorlib.dll"
"S:\Progs\VS2017\MSBuild\15.0\Bin\Roslyn\csc.exe" ConsoleFast.cs 	%COMPILEOPTIONS%
"S:\Progs\VS2017\MSBuild\15.0\Bin\Roslyn\csc.exe" ConsoleSlow.cs 	%COMPILEOPTIONS%
"S:\Progs\VS2017\MSBuild\15.0\Bin\Roslyn\csc.exe" FileFast.cs 		%COMPILEOPTIONS%
"S:\Progs\VS2017\MSBuild\15.0\Bin\Roslyn\csc.exe" FileSlow.cs		%COMPILEOPTIONS%
goto DONE

:DONE