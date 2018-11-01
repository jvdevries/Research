@echo off
SET runs=10000
SET printPer=100000
prompt $G
@echo on

@echo|set /p="This should report a find eventually."
"MOVNTI + SFENCE.exe" %runs% %printPer%
@echo.

@echo|set /p="This should report a find eventually."
"SFENCE + LFENCE.exe" %runs% %printPer%
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on