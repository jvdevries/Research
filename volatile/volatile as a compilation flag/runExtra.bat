@echo off
SET waitFor=5
prompt $G
@echo on

@echo|set /p="This should hang for ~%waitFor% seconds."
"Extra 1 - Passing volatile via reference hang.exe" %waitFor%
@echo.

@echo|set /p="This should hang for ~%waitFor% seconds."
"Extra 2a - Passing volatile via argument hang.exe" %waitFor%
@echo.

@echo|set /p="This should not hang for ~%waitFor% seconds (but end fast)."
"Extra 2b - Passing volatile via argument + dynamic cast no-hang.exe" %waitFor%
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on