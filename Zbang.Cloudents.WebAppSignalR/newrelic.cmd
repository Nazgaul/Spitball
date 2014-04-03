SETLOCAL EnableExtensions

for /F "usebackq tokens=1,2 delims==" %%i in (`wmic os get LocalDateTime /VALUE 2^>NUL`) do if '.%%i.'=='.LocalDateTime.' set ldt=%%j
set ldt=%ldt:~0,4%-%ldt:~4,2%-%ldt:~6,2% %ldt:~8,2%:%ldt:~10,2%:%ldt:~12,6%

:: Update with your license key
SET LICENSE_KEY=3c7434bc408ced6499a908faffb43f35a74b90c8

SET NR_ERROR_LEVEL=0

:: Comment out the line below if you do not want to install the New Relic Agent
CALL:INSTALL_NEWRELIC_AGENT

:: Comment out the line below if you do not want to install the New Relic Windows Server Monitor
CALL:INSTALL_NEWRELIC_SERVER_MONITOR

IF %NR_ERROR_LEVEL% EQU 0 (
	EXIT /B 0
) ELSE (
	EXIT %NR_ERROR_LEVEL%
)

:: --------------
:: Functions
:: --------------
:INSTALL_NEWRELIC_AGENT
	ECHO %ldt% : Begin installing the New Relic .net Agent >> "%RoleRoot%\nr.log" 2>&1

	:: Current version of the installer
	SET NR_INSTALLER_NAME=NewRelicAgent_x64_2.24.218.0.msi
	:: Path used for custom configuration and worker role environment varibles
	SET NR_HOME=%ALLUSERSPROFILE%\New Relic\.NET Agent\

	ECHO Installing the New Relic .net Agent. >> "%RoleRoot%\nr.log" 2>&1

	IF "%IsWorkerRole%" EQU "true" (
	    msiexec.exe /i %NR_INSTALLER_NAME% /norestart /quiet NR_LICENSE_KEY=%LICENSE_KEY% INSTALLLEVEL=50 /lv* %RoleRoot%\nr_install.log
	) ELSE (
	    msiexec.exe /i %NR_INSTALLER_NAME% /norestart /quiet NR_LICENSE_KEY=%LICENSE_KEY% /lv* %RoleRoot%\nr_install.log
	)

	:: CUSTOM newrelic.xml : Uncomment the line below if you want to copy a custom newrelic.xml file into your instance
	REM copy /Y "newrelic.xml" "%NR_HOME%" >> %RoleRoot%\nr.log

	:: CUSTOM INSTRUMENTATION : Uncomment the line below to copy custom instrumentation into the agent directory.
	REM copy /Y "CustomInstrumentation.xml" "%NR_HOME%\extensions" >> %RoleRoot%\nr.log

	:: WEB ROLES : Restart the service to pick up the new environment variables
	:: 	if we are in a Worker Role then there is no need to restart W3SVC _or_
	:: 	if we are emulating locally then do not restart W3SVC
	IF "%IsWorkerRole%" EQU "false" IF "%EMULATED%" EQU "false" (
		ECHO Restarting IIS and W3SVC to pick up the new environment variables >> "%RoleRoot%\nr.log" 2>&1
		IISRESET
		NET START W3SVC
	)

	IF %ERRORLEVEL% EQU 0 (
	  REM  The New Relic .net Agent installed ok and does not need to be installed again.
	  ECHO New Relic .net Agent was installed successfully. >> "%RoleRoot%\nr.log" 2>&1

	) ELSE (
	  REM   An error occurred. Log the error to a separate log and exit with the error code.
	  ECHO  An error occurred installing the New Relic .net Agent 1. Errorlevel = %ERRORLEVEL%. >> "%RoleRoot%\nr_error.log" 2>&1

	  SET NR_ERROR_LEVEL=%ERRORLEVEL%
	)

GOTO:EOF

:INSTALL_NEWRELIC_SERVER_MONITOR
	ECHO %ldt% : Begin installing the New Relic Server Monitor >> "%RoleRoot%\nr_server.log" 2>&1

	:: Current version of the installer
	SET NR_INSTALLER_NAME=NewRelicServerMonitor_x64_3.0.230.0.msi

	ECHO Installing the New Relic Server Monitor. >> "%RoleRoot%\nr_server.log" 2>&1
	msiexec.exe /i %NR_INSTALLER_NAME% /norestart /quiet NR_LICENSE_KEY=%LICENSE_KEY% /lv* %RoleRoot%\nr_server_install.log

	IF %ERRORLEVEL% EQU 0 (
	  REM  The New Relic Server Monitor installed ok and does not need to be installed again.
	  ECHO New Relic Server Monitor was installed successfully. >> "%RoleRoot%\nr_server.log" 2>&1

	  NET STOP "New Relic Server Monitor Service"
	  NET START "New Relic Server Monitor Service"

	) ELSE (
	  REM   An error occurred. Log the error to a separate log and exit with the error code.
	  ECHO  An error occurred installing the New Relic Server Monitor 1. Errorlevel = %ERRORLEVEL%. >> "%RoleRoot%\nr_server_error.log" 2>&1

	  SET NR_ERROR_LEVEL=%ERRORLEVEL%
	)

GOTO:EOF




