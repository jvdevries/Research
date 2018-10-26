@echo off
SET runs=10000
SET printPer=100000
prompt $G
@echo on

@echo|set /p="This should not find anything (but the test is too short to consider anything)."
"mfence.exe" %runs% %printPer%
@echo.

@echo|set /p="This should not find anything (but the test is too short to consider anything)."
"CPUID.exe" %runs% %printPer%
@echo.

@echo|set /p="This should not find anything (but the test is too short to consider anything)."
"LOCK + ADD.exe" %runs% %printPer%
@echo.

@echo|set /p="This should not find anything (but the test is too short to consider anything)."
"LOCK + OR.exe" %runs% %printPer%
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on