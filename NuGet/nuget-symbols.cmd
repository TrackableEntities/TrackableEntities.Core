rem Pushing Symbols packages to NuGet.org

set version=1.0.0
set source=https://nuget.smbsrc.net/
NuGet.exe SetApiKey 5682793e-2994-4016-b7b4-c11be576703b
rem NuGet.exe push TrackableEntities.Common.Core\TrackableEntities.Common.Core.%version%.symbols.nupkg -source %source%
NuGet.exe push TrackableEntities.EF.Core\TrackableEntities.EF.Core.%version%.symbols.nupkg -source %source%
rem NuGet.exe push Output\TrackableEntities.Patterns.Core\TrackableEntities.Patterns.Core.%version%.symbols.nupkg -source %source%
rem NuGet.exe push Output\TrackableEntities.Patterns.EF.Core\TrackableEntities.Patterns.EF.Core.%version%.symbols.nupkg -source %source%
