@echo off
SET runs=100000
SET printPer=100000
prompt $G
@echo on

@echo|set /p="This should report a find fast."
"ADD loop.exe" %runs% %printPer% 1
@echo.

@echo|set /p="This should report a find much slower."
"ADD loop.exe" %runs% %printPer% 20
@echo.

@echo|set /p="This should report a find fast."
"SFENCE loop.exe" %runs% %printPer% 1
@echo.

@echo|set /p="This should report a find much slower."
"SFENCE loop.exe" %runs% %printPer% 30
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on