@echo off
SET waitFor=3
prompt $G
@echo on

@echo|set /p="This should hang for ~%waitFor% seconds."
"Extra 1 - Passing volatile via reference hang.exe" %waitFor%
@echo.

@echo|set /p="This should hang for ~%waitFor% seconds."
"Extra 2 - While loop via dynamic no-hang.exe" %waitFor%
@echo.

@echo|set /p="This should not hang for ~%waitFor% seconds (but end fast)."
"Extra 2b - Passing non-volatile via dynamic argument no-hang.exe" %waitFor%
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on