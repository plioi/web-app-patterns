function Get-ConnectionString($environment) {

    if ($environment -eq "DEV") {
        $appsettingsPath = "./src/ContactList.Server/appsettings.Development.json"
    } else {
        $appsettingsPath = "./src/ContactList.Server.Tests/appsettings.json"
    }

    return (Get-Content $appsettingsPath | Out-String | ConvertFrom-Json).ConnectionStrings.DefaultConnection
}

function Drop-Database($environment) {
    $connectionString = Get-ConnectionString $environment
    exec { dotnet rh --env $environment `
          --commandtimeout 300 `
          --connectionstring $connectionString `
          --silent `
          --drop }
}

function Migrate-Database($environment) {
    $connectionString = Get-ConnectionString $environment
    exec { dotnet rh --env $environment `
          --commandtimeout 300 `
          --connectionstring $connectionString `
          --files ./database-migrations `
          --versionfile ./src/ContactList.Server/bin/Release/net6.0/ContactList.Server.dll `
          --transaction `
          --silent `
          --simple }
}

function Drop-Databases() {
    Drop-Database "DEV"
    Drop-Database "TEST"
}

function Migrate-Databases() {
    Migrate-Database "DEV"
    Migrate-Database "TEST"
}

function exec($command) {
    write-host ([Environment]::NewLine + $command.ToString().Trim()) -fore CYAN

    & $command
    if ($lastexitcode -ne 0) { throw $lastexitcode }
}
