# Rocket League Scrim Finder

Project was created with Visual Studio 2019, but feel free to develop with whatever IDE if you can make it work. Front end was developed with Visual Studio Code. Project is currently deployed to IIS on an Azure VM. Backend is published using a Web Deploy package, and the front end can be packaged up by running `yarn build:production` in VS Code.

## Requirements

- Visual Studio
- .NET Core 3.1

## App secrets

We're using .NET's Secret Manager to retrieve the [TRN](https://tracker.gg/) and [Steam](https://steamcommunity.com/dev) API keys, as well as the database (Azure SQL Server). Your `secrets.json` file should look like the following:

```
{
  "TrnApiKey": "API_KEY",
  "SteamApiKey": "API_KEY",
  "ConnectionStrings": {
    "RlsfDatabase": "DB_CONNECTION_STRING"
  }
}
```

### Setting secrets without Visual Studio

To set your secrets without opening up Visual Studio, create a `secrets.json` file at `C:\Users\{YOUR_USERNAME}\AppData\Roaming\Microsoft\UserSecrets\c007cd12-1fe7-4843-947e-ddecfc0d8913\secrets.json` if it does not already exist.

### Setting secrets from within Visual Studio

Right-click on the project in the solution explorer, and click "Manage User Secrets". Then edit the JSON file accordingly.
