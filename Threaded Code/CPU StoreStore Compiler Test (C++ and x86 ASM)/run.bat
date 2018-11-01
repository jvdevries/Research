@echo off
prompt $G
@echo on

@echo|set /p="This shows the only difference in the C++ files."
"fc" "StoreStore Occurence with MOVNTI STOI.cpp" "StoreStore Occurence with MOVNTI WHILE.cpp"
@echo.

@echo|set /p="This should report a find, but it won't."
"StoreStore Occurence with MOVNTI STOI.exe"
@echo.

@echo|set /p="This should report a find."
"StoreStore Occurence with MOVNTI WHILE.exe"
@echo.

@echo|set /p="This shows the only difference in the C++ files."
"fc" "StoreStore Occurence with MOVNTI STOI + ThreadAffinity.cpp" "StoreStore Occurence with MOVNTI WHILE + ThreadAffinity.cpp"
@echo.

@echo|set /p="This should report a find, but it won't."
"StoreStore Occurence with MOVNTI STOI + ThreadAffinity.exe"
@echo.

@echo|set /p="This should report a find."
"StoreStore Occurence with MOVNTI WHILE + ThreadAffinity.exe"
@echo.


@echo off
prompt
set /p input="Press enter to exit..."
@echo on