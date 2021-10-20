@echo off

echo ######## Packing '../res' directory to 'ArchiveResource.cs' ########
cd %~dp0
packfolder.exe ../res ../ArchiveResource.cs -csharp
fart.exe -- ..\ArchiveResource.* SciterAppResource SciterSharp
echo OK