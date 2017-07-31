# dotnet-status
![status](https://jaredcnancedev.visualstudio.com/_apis/public/build/definitions/6a4ea810-b7a7-4616-ac7a-8b98bf6066fc/1/badge)

This project provides an open API to get the current status of your
dotnet project dependencies. 

## Requirements

- You must have a `.sln` file at the root of your repository.
- Your GitHub repository must be public

## API

Currently, we only support GitHub projects. To get the status of your dependencies, simply:

```
curl "http://dotnet-status.com/api/status/gh/{USER}/{PROJECT}"
```

An example, for the [JsonApiDotNetCore](https://github.com/Research-Institute/json-api-dotnet-core) repository:

```bash
curl "http://dotnet-status.com/api/status/gh/Research-Institute/json-api-dotnet-core"
```

## Development

#### Pre-Requisites

- .Net Core (see `dotnet-status.csproj` for current version)
- Yarn
- Ember CLI

#### Installing SignalR Client

Since npm doesn't support per-package registries in package.json, there is an extra step required to 
use the SignalR pre-release client:

```
npm install signalr-client --registry https://dotnet.myget.org/f/aspnetcore-ci-dev/npm/
```
