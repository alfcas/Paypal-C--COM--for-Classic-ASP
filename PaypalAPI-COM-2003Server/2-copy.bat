@echo off
rem si copia il componente nella stessa cartella di inetserv per problemi di sicurezza
rem e non in system32
rem
copy /y PaypalAPI.dll c:\windows\system32\inetsrv
