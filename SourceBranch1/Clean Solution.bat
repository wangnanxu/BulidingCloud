%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\MSBuild ML.BC.sln /t:clean /p:VisualStudioVersion=12.0 /p:Configuration=Release
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\MSBuild ML.BC.sln /t:clean /p:VisualStudioVersion=12.0 /p:Configuration=Debug

@echo off
echo clear the files
rd ML.BC.BCBackData\obj /s /q
rd ML.BC.BCBackData\bin /s /q

rd ML.BC.BCBackWeb\obj /s /q
rd ML.BC.BCBackWeb\bin /s /q

rd ML.BC.EnterpriseData\obj /s /q
rd ML.BC.EnterpriseData\bin /s /q

rd ML.BC.EnterpriseWeb\obj /s /q
rd ML.BC.EnterpriseWeb\bin /s /q

rd ML.BC.Infrastructure\obj /s /q
rd ML.BC.Infrastructure\bin /s /q

rd ML.BC.Services\obj /s /q
rd ML.BC.Services\bin /s /q

rd ML.BC.Web.Framework\obj /s /q
rd ML.BC.Web.Framework\bin /s /q

pause