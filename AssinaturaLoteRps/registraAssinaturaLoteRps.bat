@echo off
echo Realiza registro da DLL "AssinaturaLoteRPS.dll" utilizando o "RegAsm.exe"

echo *** Preparando Framework 4.0 ***
set path=%~dp0
set dllName=%path%AssinaturaLoteRPS.dll /tlb:%path%AssinaturaLoteRPS.tlb
set dllName2=AssinaturaLoteRPS.dll /tlb:AssinaturaLoteRPS.tlb
set version=v4.0.30319
set regasm="%WINDIR%\Microsoft.NET\Framework\%version%\RegAsm.exe"

echo *** Desregistrando DLL, caso exista ***
%regasm% /u %dllName2%

echo *** Registrando DLL ***
%regasm% %dllName% /registered

echo *** Processo concluido! ***
pause