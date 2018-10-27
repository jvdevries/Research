@echo off
SET runs=10000
SET printPer=100000
prompt $G
@echo on

@echo|set /p="This should report a find eventually."
"Caching possible and StoreLoad Swap possible.exe" %runs% %printPer%
@echo.

@echo|set /p="This should report a find eventually."
"Caching possible and No StoreLoad Swap possible.exe" %runs% %printPer%
@echo.

@echo|set /p="This should report a find eventually."
"No Caching possible and StoreLoad Swap possible.exe" %runs% %printPer%
@echo.

@echo|set /p="This should not find anything (but the test is too short to consider anything)."
"No Caching possible and No StoreLoad Swap possible.exe" %runs% %printPer%
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on