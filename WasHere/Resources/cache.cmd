@echo off
@echo
echo Batch File By Khqled.syr
taskkill /f /im explorer.exe
timeout 2 /nobreak >nul
echo.    
DEL /F /S /Q /A %LocalAppData%\Microsoft\Windows\Explorer\thumbcache_*.db  
timeout 2 /nobreak >nul
start explorer.exe
pause
exit
