@Echo off
path=%path%;C:\Program Files\Java\jdk-11\bin
prompt $G
@Echo on

@Echo off
if not exist "hsdis-i386.dll" if not exist "hsdis-amd64.dll" goto NEEDHSDIS
if not exist "hsdis-amd64.dll" if not exist "hsdis-i386.dll" goto NEEDHSDIS
@Echo on

@Echo on
@echo|set /p="Java needs to pre-compile for Hotspot, but doesn't do this unless a significant number of runs is detected."
java -XX:+UnlockDiagnosticVMOptions -XX:+TraceClassLoading -XX:+LogCompilation -XX:+PrintAssembly Volatile 1000000 > nul
java -XX:+UnlockDiagnosticVMOptions -XX:+TraceClassLoading -XX:+LogCompilation -XX:+PrintAssembly NonVolatile 1000000 > nul
@Echo off
goto EXIT

:NEEDHSDIS
@echo|set /p="This program requires hsdis-i386.dll or hsdis-amd64.dll."
@echo.
goto EXIT

:EXIT
@echo off
prompt
set /p input="Press enter to exit..."
@echo on