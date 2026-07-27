param([switch]$Remove)

Set-Location $PSScriptRoot

if ($Remove) {
    docker stack rm biogama
    return
}

# Cargar variables del .env
Get-Content .env | ForEach-Object {
    if ($_ -match '^\s*([^#=]+)=(.*)\s*$') {
        $name = $matches[1].Trim()
        $value = $matches[2].Trim()
        Set-Item -Path "env:$name" -Value $value
    }
}

docker stack deploy -c docker-stack.yml biogama
