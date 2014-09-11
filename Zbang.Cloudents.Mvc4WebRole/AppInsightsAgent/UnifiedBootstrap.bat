REM /*********************************************************
REM *                                                        *
REM *    Copyright (C) Microsoft. All rights reserved.       *
REM *                                                        *
REM *********************************************************/
@echo off

set ExecutionPolicyDesired=RemoteSigned

FOR /F "tokens=*" %%i IN ('powershell -noprofile -command Get-ExecutionPolicy') DO Set ExecutionPolicyCurrent=%%i

echo Setting desired execution policy: %ExecutionPolicyDesired% >> %~dp0ApmAgentBootstrapper.log
Powershell.exe -NoProfile -Command "Set-ExecutionPolicy %ExecutionPolicyDesired%" >> %~dp0ApmAgentBootstrapper.log

echo Executing power shell script >> %~dp0ApmAgentBootstrapper.log
Powershell.exe -NoProfile -Command "& '%~dp0UnifiedBootstrap.ps1'" >> %~dp0ApmAgentBootstrapper.log

echo Setting execution policy back: %ExecutionPolicyCurrent% >> %~dp0ApmAgentBootstrapper.log
Powershell.exe -NoProfile -Command "Set-ExecutionPolicy %ExecutionPolicyCurrent%" >> %~dp0ApmAgentBootstrapper.log

exit 0