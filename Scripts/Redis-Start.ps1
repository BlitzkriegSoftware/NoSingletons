<#
	
    .SYNOPSIS
        Start REDIS in a Docker Container (for Demo)   

    .DESCRIPTION
        See above
    
    .INPUTS
        none

    .OUTPUTS
        Sucess or failure 
#>

function Get-DockerRunning {

	[bool]$DockerAlive = $false

	try {
		$null = Get-Process 'com.docker.proxy'
		$DockerAlive = $true;
	} catch {
		$DockerAlive = $false;
	}

	return $DockerAlive
}


#
# Main
#

Set-StrictMode -Version 2.0
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls -bor [Net.SecurityProtocolType]::Tls11 -bor [Net.SecurityProtocolType]::Tls12
$ScriptUtc = (get-date).ToUniversalTime()
Push-Location $PSScriptRoot
[string]$scriptName = $MyInvocation.MyCommand.Name

[bool]$da = Get-DockerRunning

if(! $da) {
	Write-Error "docker must be running 1st"
	return 1
}

[int]$REDISPORT=6379
[string]$REDISNAME="d-redis"
$null = (docker stop "${REDISNAME}") 2> $null
$null = (docker rm "${REDISNAME}") 2> $null
docker run --name "${REDISNAME}" -d -p "${REDISPORT}:${REDISPORT}" redis