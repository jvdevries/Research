@echo off
SET runs=10000
SET printPer=100000
prompt $G
@echo on

@echo|set /p="This should report a find eventually."
"SFENCE + LFENCE.exe" %runs% %printPer%
@echo.

@echo|set /p="This should report a find eventually."
"CLFLUSH.exe" %runs% %printPer%
@echo.

@echo|set /p="This should report a find eventually."
@echo.
@echo|set /p="But it is hard to trigger."
"CLFLUSH Extra.exe" %runs% %printPer%
@echo.

@echo|set /p="This should report a find eventually."
"_mm_clflush.exe" %runs% %printPer%
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on