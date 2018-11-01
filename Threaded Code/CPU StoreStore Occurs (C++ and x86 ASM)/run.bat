@echo off
SET runs=10000
SET printPer=100000
prompt $G
@echo on

@echo|set /p="This should not find anything (but the test is too short to consider anything)."
"StoreStore NonOccurence with MOV.exe" %runs% %printPer%
@echo.

@echo|set /p="This should report a find eventually."
"StoreStore Occurence with MOVNTI.exe" %runs% %printPer%
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on