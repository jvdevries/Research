path=%path%;C:\Program Files\Java\jdk-11\bin

@Echo off
setlocal enableDelayedExpansion
for /f "usebackq delims=|" %%f in (`dir /b "*.java"`) do CALL :make_program "%%~nf"
exit /b

:make_program
javac ^"%~1.java^"
exit /b