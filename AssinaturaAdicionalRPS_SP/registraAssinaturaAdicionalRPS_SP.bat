@echo off
echo Realiza registro da DLL "AssinaturaAdicionalRPS_SP.dll" utilizando o "RegSvcs.exe"

if EXIST %WINDIR%\SysWOW64 goto Win64
:Win32
set sistema=Framework
echo *** Preparando instalacao ***
goto end

:Win64
set sistema=Framework64
echo *** Preparando instalacao x64 ***
goto end
:end

echo *** Preparando Framework 4.0 ***
set dllName=AssinaturaAdicionalRPS_SP.dll /tlb:AssinaturaAdicionalRPS_SP.tlb
set version=v4.0.30319
set regasm="%WINDIR%\Microsoft.NET\%sistema%\%version%\RegSvcs.exe"

echo *** Desregistrando DLL, caso exista ***
%regasm% /u /quiet %dllName%

echo *** Registrando DLL ***
%regasm% /quiet %dllName%

echo *** Processo concluido! ***
pause