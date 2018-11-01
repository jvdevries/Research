@echo off
SET waitFor=3
prompt $G
@echo on

@echo|set /p="This should hang for ~%waitFor% seconds."
"While loop hang.exe" %waitFor%
@echo.

@echo|set /p="This should not hang for ~%waitFor% seconds (but end fast)."
"Extra 2b - Passing non-volatile via dynamic argument no-hang.exe" %waitFor%
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on