@echo off
SET runs=10000
SET printPer=100000
prompt $G
@echo on

@echo|set /p="This should report a find eventually."
"NoFix Cache on Volatile class.exe" %runs% %printPer%
@echo.

@echo|set /p="This should report a find eventually."
"NoFix Cache on volatile.exe" %runs% %printPer%
@echo.

@echo|set /p="This should report a find eventually."
@echo|set /p="It is however hard to trigger...."
"NoFix Swap on Thread.Volatile.exe" %runs% %printPer%
@echo.

@echo|set /p="This should not find anything (but the test is too short to consider anything)."
"Cache on Thread.Volatile.exe" %runs% %printPer%
@echo.

@echo|set /p="This should not find anything (but the test is too short to consider anything)."
"Cache and Swap on MemoryBarrier.exe" %runs% %printPer%
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on