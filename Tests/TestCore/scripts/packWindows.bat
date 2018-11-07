@echo off

if "%1"=="Debug" exit

echo ######## Packing '/res' directory to 'ArchiveResource.cs' ########
cd %~dp0
packfolder.exe ../res ../ArchiveResource.cs -csharp