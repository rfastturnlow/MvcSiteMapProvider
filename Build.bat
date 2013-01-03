set nuget=.\src\MvcSiteMapProvider\.nuget\NuGet.exe

mkdir packages
%nuget% pack .\src\MvcSiteMapProvider\MvcSiteMapProvider.Core\MvcSiteMapProvider.Core.csproj -NonInteractive -Build -Symbols -OutputDirectory .\packages
%nuget% pack .\src\MvcSiteMapProvider\MvcSiteMapProvider.Web\MvcSiteMapProvider.Web.csproj -NonInteractive -Build -Symbols -OutputDirectory .\packages