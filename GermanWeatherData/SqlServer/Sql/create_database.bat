@echo off

:: Copyright (c) Philipp Wagner. All rights reserved.
:: Licensed under the MIT license. See LICENSE file in the project root for full license information.

set SQLCMD_EXECUTABLE="C:\Program Files\Microsoft SQL Server\110\Tools\Binn\SQLCMD.EXE"
set STDOUT=stdout.log
set STDERR=stderr.log
set LOGFILE=query_output.log

set ServerName=.\MSSQLSERVER2017
set DatabaseName=GermanWeatherDatabase

call :AskQuestionWithYdefault "Use Server (%ServerName%) [Y,n]?" reply_
if /i [%reply_%] NEQ [y] (
	set /p ServerName="Enter Server: "
)

call :AskQuestionWithYdefault "Use Database (%DatabaseName%) [Y,n]?" reply_
if /i [%reply_%] NEQ [y]  (
	set /p DatabaseName="Enter Database: "
)

1>%STDOUT% 2>%STDERR% (

	:: Database
	%SQLCMD_EXECUTABLE% -S %ServerName% -i "create_database.sql" -v dbname=%DatabaseName% -o %LOGFILE%
	
)

goto :end

:: The question as a subroutine
:AskQuestionWithYdefault
	setlocal enableextensions
	:_asktheyquestionagain
	set return_=
	set ask_=
	set /p ask_="%~1"
	if "%ask_%"=="" set return_=y
	if /i "%ask_%"=="Y" set return_=y
	if /i "%ask_%"=="n" set return_=n
	if not defined return_ goto _asktheyquestionagain
	endlocal & set "%2=%return_%" & goto :EOF

:end
pause