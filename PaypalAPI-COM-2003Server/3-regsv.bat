@echo off
rem si copia il componente nella stessa cartella di inetserv per problemi di sicurezza
rem e non in system32
rem
cd c:\windows\system32\inetsrv
C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe PaypalAPI.dll /Codebase
pause
