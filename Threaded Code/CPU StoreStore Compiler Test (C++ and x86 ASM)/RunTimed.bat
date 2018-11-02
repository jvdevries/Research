@echo off
call :PROCESSCALL "StoreStoreSTOILong.exe"
call :PROCESSCALL "StoreStoreWHILELong.exe"
goto :READLINE

:PROCESSCALL
timeout 1 /nobreak > nul
call:SWSTART
%~1
call :SWDIFFERENCE %~2
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

:READLINE
@echo off
prompt
set /p input="Press enter to exit..."
@echo on
GOTO DONE

:DONE