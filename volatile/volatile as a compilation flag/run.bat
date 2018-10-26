@echo off
SET waitFor=5
prompt $G
@echo on

@echo|set /p="This should hang for ~%waitFor% seconds."
"While loop hang.exe" %waitFor%
@echo.

@echo|set /p="This should not hang for ~%waitFor% seconds (but end fast)."
"While loop with volatile no-hang.exe" %waitFor%
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on