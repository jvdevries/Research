@echo off
prompt $G
@echo on

@echo|set /p="This should not find anything (but as always, be carefull when drawing conclusions)."
"StoreStore NonOccurence with MOV.exe"
@echo.

@echo|set /p="This should report a find eventually."
"StoreStore Occurence with MOVNTI.exe"
@echo.

@echo|set /p="This should not find anything (but as always, be carefull when drawing conclusions)."
"StoreStore NonOccurence with MOVNTI + SFENCE.exe"
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on