
%windir%\system32\inetsrv\appcmd set config -section:applicationPools -applicationPoolDefaults.processModel.idleTimeout:00:00:00
%windir%\system32\inetsrv\appcmd set config -section:applicationPools -applicationPoolDefaults.startMode:AlwaysRunning

%windir%\system32\inetsrv\AppCmd.exe set config -section:system.webServer/httpErrors -errorMode:Detailed 
%windir%\system32\inetsrv\AppCmd.exe set config -section:httpCompression -[name='gzip'].staticCompressionLevel:9 -[name='gzip'].dynamicCompressionLevel:4
exit /b 0