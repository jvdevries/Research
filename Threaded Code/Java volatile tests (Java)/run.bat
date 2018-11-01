@Echo off
SET testRuns=100000
SET printPer=30000
prompt $G
@Echo on

@echo|set /p="This should report a find eventually."
java CachingPossibleStoreLoadSwapPossible %testRuns% %printPer%
@echo.

@echo|set /p="This should report a find eventually."
java CachingPossibleNoStoreLoadSwapPossible %testRuns% %printPer%
@echo.

@echo|set /p="This should not find anything (but the test is too short to consider anything)."
java NoCachingPossibleStoreLoadSwapPossible %testRuns% %printPer%
@echo.

@echo|set /p="This should not find anything (but the test is too short to consider anything)."
java NoCachingPossibleNoStoreLoadSwapPossible %testRuns% %printPer%
@echo.

@echo|set /p="This should hang for 3 seconds."
java compilerHang 3
@echo.

@echo|set /p="This should not hang for 3 seconds, but end quickly."
java compilerHangVolatile 3
@echo.

@echo off
prompt
set /p input="Press enter to exit..."
@echo on