@echo off

if "%1"=="Debug" exit

scripts\packfolder.exe res ArchiveResource.cs -csharp