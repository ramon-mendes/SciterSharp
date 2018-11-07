@echo off

echo ######## Packing '../LibConsole' directory to 'ArchiveResource.cs' ########
cd %~dp0
packfolder.exe ../LibConsole ../ArchiveResource.cs -csharp
fart.exe -- ..\ArchiveResource.* SciterAppResource SciterSharp
echo OK