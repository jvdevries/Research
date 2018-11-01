@echo off
SET runs=10000
SET printPer=100000
prompt $G
@echo on

@echo|set /p="This should not find anything (but the test is too short to consider anything)."
"MOVNTI.exe" %runs% %printPer%
@echo.

@echo|set /p="This should not find anything (but the test is too short to consider anything)."
"MOVNTI + SFENCE.exe" %runs% %printPer%
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on