@echo off
SET runs=10000
SET printPer=100000
prompt $G
@echo on

@echo|set /p="This should report a find eventually."
"Baseline.exe" %runs% %printPer%
@echo.

@echo|set /p="This should report a find eventually."
"NoFix Volatile class.exe" %runs% %printPer%
@echo.

@echo|set /p="This should report a find eventually."
"NoFix volatile.exe" %runs% %printPer%
@echo.

@echo|set /p="This should report a find eventually."
@echo.
@echo|set /p="It is however hard to trigger...."
"NoFix Thread.Volatile.exe" %runs% %printPer%
@echo.

@echo|set /p="This should not find anything (but the test is too short to consider anything)."
"Fix MemoryBarrier.exe" %runs% %printPer%
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on