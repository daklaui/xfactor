@echo off
REM: print new line
echo.
echo -----------------------------------------
echo *** Build Xfactor Web App Script  ***
echo -----------------------------------------
REM: print new line
echo Select which environment to build (type xpertise or medfactor):
SET /P BUILD_ENV=">"
2>NUL CALL :CASE_%BUILD_ENV% # jump to :CASE_xpertise, :CASE_medfactor, etc.
IF ERRORLEVEL 1 CALL :DEFAULT_CASE # If label doesn't exist

ECHO Done.
EXIT /B

:CASE_xpertise
  set fnew='<text/>'
   powershell -Command "(gc templateConfigWebApp.txt) -replace "'{{CONECTIONS_TAGS}}'", %fnew% | Out-File config.xml -encoding utf8"
 echo "xpertise %fnew%"  
GOTO END_CASE
:CASE_medfactor
    echo "medfactor"
  GOTO END_CASE
:DEFAULT_CASE
  ECHO Unknown color "%BUILD_ENV%"
  GOTO END_CASE
:END_CASE
pause
  VER > NUL # reset ERRORLEVEL
  GOTO :EOF # return from CALL

