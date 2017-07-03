# dotnet-status

This project provides an open API to get the current status of your
dotnet project dependencies. 

## Requirements

Your project must be using the new `*.csproj` file format.

## API

Currently, we only support GitHub projects. 
To get the status of your dependencies, simply:

```
curl "http://dotnet-status.com/api/status/gh/{USER}/{PROJECT}/{BRANCH}/{PATH_TO_CSPROJ}.csproj/"
```

An example, for this repository:

```bash
curl "http://dotnet-status.com/api/status/gh/Research-Institute/json-api-dotnet-core/master/src/JsonApiDotNetCore/JsonApiDotNetCore.csproj/"
```

```json
{
   "isPreRelease" : false,
   "upToDate" : true,
   "packages" : [
      {
         "name" : "Microsoft.AspNetCore.Routing",
         "latestVersion" : "2.0.0-preview2-final",
         "latestStableVersion" : "1.1.2",
         "currentVersion" : "1.1.2",
         "upToDate" : true,
         "isPreRelease" : false
      },
      {
         "currentVersion" : "1.1.3",
         "name" : "Microsoft.AspNetCore.Mvc",
         "latestVersion" : "2.0.0-preview2-final",
         "latestStableVersion" : "1.1.3",
         "isPreRelease" : false,
         "upToDate" : true
      },
      {
         "latestVersion" : "2.0.0-preview2-final",
         "name" : "Microsoft.EntityFrameworkCore",
         "latestStableVersion" : "1.1.2",
         "currentVersion" : "1.1.2",
         "upToDate" : true,
         "isPreRelease" : false
      },
      {
         "currentVersion" : "1.1.2",
         "name" : "Microsoft.Extensions.Logging",
         "latestVersion" : "2.0.0-preview2-final",
         "latestStableVersion" : "1.1.2",
         "isPreRelease" : false,
         "upToDate" : true
      },
      {
         "isPreRelease" : false,
         "upToDate" : true,
         "currentVersion" : "4.3.1",
         "latestVersion" : "4.4.0-preview2-25405-01",
         "name" : "System.ValueTuple",
         "latestStableVersion" : "4.3.1"
      }
   ]
}
```
