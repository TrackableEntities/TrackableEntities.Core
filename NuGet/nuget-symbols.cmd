rem Pushing Symbols packages to NuGet.org

set version=1.0.0-alpha2
NuGet.exe SetApiKey 5682793e-2994-4016-b7b4-c11be576703b
NuGet.exe push TrackableEntities.Common.Core\TrackableEntities.Common.Core.%version%.symbols.nupkg
NuGet.exe push TrackableEntities.EF.Core\TrackableEntities.EF.Core.%version%.symbols.nupkg
rem NuGet.exe push Output\TrackableEntities.Patterns.Core\TrackableEntities.Patterns.Core.%version%.symbols.nupkg
rem NuGet.exe push Output\TrackableEntities.Patterns.EF.Core\TrackableEntities.Patterns.EF.Core.%version%.symbols.nupkg
