@echo off
for /l %%x in (1, 1, 9) do (call :MAKESUMMARY > summaryReverse%%x.txt)
goto DONE

: MAKESUMMARY
echo ### The FAST/SLOW difference is only insignificant lines ###
call :PRINT_PROOF_FILE_DIFFERENCES
echo.

echo ### The time used for executing each EXE is the same according to batch ###
call :PROCESSCALL "ConsoleSlow.exe > console-slow.txt" "Console-Slow"
call :PROCESSCALL "ConsoleFast.exe > console-fast.txt" "Console-Fast"
del file-slow.txt
call :PROCESSCALL "FileSlow.exe" "File-Slow"
ren file.txt file-slow.txt
del file-fast.txt
call :PROCESSCALL "FileFast.exe" "File-Fast"
ren file.txt file-fast.txt
del file-slowQPC.txt
call :PROCESSCALL "FileSlowQPC.exe" "File-Slow"
ren file.txt file-slowQPC.txt
del file-fastQPC.txt
call :PROCESSCALL "FileFastQPC.exe" "File-Fast"
ren file.txt file-fastQPC.txt
echo.

echo ### Although SUM results are the same, the stopwatch ticks is not (yet freq is) ###
call :PRINTRESULT "console-fast.txt"
call :PRINTRESULT "console-slow.txt"
call :PRINTRESULT "file-fast.txt"
call :PRINTRESULT "file-slow.txt"
call :PRINTRESULT "file-fastQPC.txt"
call :PRINTRESULT "file-slowQPC.txt"
goto DONE

:PRINT_PROOF_FILE_DIFFERENCES
fc ConsoleFast.cs ConsoleSlow.cs
fc FileFast.cs FileSlow.cs
fc FileFastQPC.cs FileSlowQPC.cs
goto DONE

:PROCESSCALL
timeout 1 /nobreak > nul
call:SWSTART
%~1
call :SWDIFFERENCE %~2
goto DONE

:PRINTRESULT
echo ----- %~1 -----
type %~1
echo.
goto DONE

:SWSTART
set STARTTIME=%TIME%
goto DONE

:SWDIFFERENCE
set ENDTIME=%TIME%
for /F "tokens=1-4 delims=:.," %%a in ("%STARTTIME%") do (
   set /A "start=(((%%a*60)+1%%b %% 100)*60+1%%c %% 100)*100+1%%d %% 100"
)

for /F "tokens=1-4 delims=:.," %%a in ("%ENDTIME%") do (
   set /A "end=(((%%a*60)+1%%b %% 100)*60+1%%c %% 100)*100+1%%d %% 100"
)

set /A elapsed=end-start

set /A hh=elapsed/(60*60*100), rest=elapsed%%(60*60*100), mm=rest/(60*100), rest%%=60*100, ss=rest/100, cc=rest%%100
if %hh% lss 10 set hh=0%hh%
if %mm% lss 10 set mm=0%mm%
if %ss% lss 10 set ss=0%ss%
if %cc% lss 10 set cc=0%cc%

set DURATION=%hh%:%mm%:%ss%,%cc%
echo %~1 Time Taken : %DURATION%
GOTO DONE

:DONE