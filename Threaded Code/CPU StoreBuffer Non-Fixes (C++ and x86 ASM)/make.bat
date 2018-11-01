@Echo on
if NOT exist "restorepath.bat" PATH >restorepath.bat
SET CPUArchitecture=x86
SET windowsSDKVersion=10.0.17134.0
SET windowsSDKPath=S:\Windows Kits\10
SET MSCVPath=S:\Progs\VS2017\VC\Tools\MSVC\14.15.26726
SET NETFXSDK=C:\Program Files (x86)\Windows Kits\NETFXSDK\4.6.1\

SET SDKFullPath=%path%;%windowsSDKPath%\bin\%windowsSDKVersion%\%CPUArchitecture%
SET compilerFullPath=%MSCVPath%\bin\Host%CPUArchitecture%\%CPUArchitecture%

SET path=%path%;%SDKFullPath%;%compilerFullPath%

SET INCLUDE=%MSCVPath%\include;%NETFXSDK%\include\um;%windowsSDKPath%\include\%windowsSDKVersion%\ucrt;%windowsSDKPath%\include\%windowsSDKVersion%\shared;%windowsSDKPath%\include\%windowsSDKVersion%\um
SET LIB=%MSCVPath%\lib\%CPUArchitecture%;%NETFXSDK%\lib\um\%CPUArchitecture%;%windowsSDKPath%\lib\%windowsSDKVersion%\ucrt\%CPUArchitecture%;%windowsSDKPath%\lib\%windowsSDKVersion%\um\%CPUArchitecture%;
SET LIBPATH=%MSCVPath%\lib\%CPUArchitecture%;%MSCVPath%\lib\%CPUArchitecture%\store\references;%windowsSDKPath%\UnionMetadata\%windowsSDKVersion%;%windowsSDKPath%\References\%windowsSDKVersion%;

If NOT exist "TMP" mkdir TMP
for /f "usebackq delims=|" %%f in (`dir /b "*.cpp" ^| findstr /v stdafx.cpp`) do CALL :make_program "%%~nf"
call restorepath.bat
del restorepath.bat
exit /b

:make_program
cl.exe /c /Zi /nologo /W3 /WX- /diagnostics:classic /sdl /O2 /Oi /Oy /GL /D WIN32 /D NDEBUG /D _CONSOLE /D _UNICODE /D UNICODE /Gm- /EHsc /MD /GS /Gy /fp:precise /permissive- /Zc:wchar_t /Zc:forScope /Zc:inline /Fo"TMP\\" /Fd"TMP\vc141.pdb" /Gd /TP /analyze- /FC /errorReport:prompt ^"%~1.cpp^"
link.exe  /ERRORREPORT:PROMPT /OUT:^".\TMP\%~1.exe^" /INCREMENTAL:NO /NOLOGO kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /MANIFEST /MANIFESTUAC:"level='asInvoker' uiAccess='false'" /manifest:embed /SUBSYSTEM:CONSOLE /OPT:REF /OPT:ICF /LTCG:incremental /TLBID:1 /DYNAMICBASE /NXCOMPAT /IMPLIB:.^"\TMP\%~1.lib^" /MACHINE:X86 /SAFESEH ^".\TMP\%~1.obj^"
copy ^".\TMP\%~1.exe^" .
exit /b