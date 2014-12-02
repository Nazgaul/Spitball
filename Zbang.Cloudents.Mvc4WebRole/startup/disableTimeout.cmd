
%windir%\system32\inetsrv\appcmd set config -section:applicationPools -applicationPoolDefaults.processModel.idleTimeout:00:00:00
%windir%\system32\inetsrv\appcmd set config -section:applicationPools -applicationPoolDefaults.startMode:AlwaysRunning

%windir%\system32\inetsrv\AppCmd.exe set config -section:system.webServer/httpErrors -errorMode:Detailed 
%windir%\system32\inetsrv\AppCmd.exe set config -section:httpCompression -[name='gzip'].staticCompressionLevel:9 -[name='gzip'].dynamicCompressionLevel:4

%windir%\system32\inetsrv\appcmd set config  -section:system.webServer/httpCompression /+"dynamicTypes.[mimeType='image/svg+xml',enabled='True']" /commit:apphost
%windir%\system32\inetsrv\appcmd set config  -section:system.webServer/httpCompression /+"staticTypes.[mimeType='image/svg+xml',enabled='True']" /commit:apphost
exit /b 0