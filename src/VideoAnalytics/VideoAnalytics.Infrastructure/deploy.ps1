#Requires -Version 6

Param(
	[string] $resourceGroup,
	[string] $location,
	[string] $templateFile,
	[string] $parametersFile
)

# Validate mandatory parameters
if (-not $resourceGroup) {
	Write-Output "> Missing required -resourceGroup parameter."
	Break
}

if (-not $location) {
	Write-Output "> Missing required -location parameter."
	Break
}

if (-not $templateFile) {
	Write-Output "> Missing required -templateFile parameter."
	Break
}

if (-not $parametersFile) {
	Write-Output "> Missing required -parametersFile parameter."
	Break
}

Write-Output "Deploying resources with the following settings:"
Write-Output "> Resource group: $resourceGroup"
Write-Output "> Location: $location"
Write-Output "> Template file: $templateFile"
Write-Output "> Parameters file: $parametersFile"

# Get timestamp
$timestamp = Get-Date -f MMddyyyyHHmmss

# Create resource group
Write-Output "> Creating resource group ..."
(az group create --name $resourceGroup --location $location)

# Deploy Azure services
Write-Output "> Validating Azure deployment ..."
$validation = az deployment group validate `
	--resource-group $resourcegroup `
	--template-file "$($templateFile)" `
	--parameters "@$($parametersFile)" `
	--parameters name=$resourceGroup `
	--output json

if ($validation) {
	$validation = $validation | ConvertFrom-Json
	
	if (-not $validation.error) {
		Write-Output "> Deploying Azure services (this could take a while)..." 
		$deployment = az deployment group create `
			--name $timestamp `
			--resource-group $resourceGroup `
			--template-file "$($templateFile)" `
			--parameters "@$($parametersFile)" `
			--parameters name=$resourceGroup `
			--output json
		Write-Output "> Finished deploying Azure services." 
	}
	else {
		Write-Output "! Template is not valid with provided parameters."
		Write-Output "! Error: $($validation.error.message)"
		Write-Output "+ To delete this resource group, run 'az group delete -g $($resourceGroup) --no-wait'"
		Break
	}
}

# Get deployment outputs
Write-Output "> Getting deployment outputs." 
$outputs = (az deployment group show `
	--name $timestamp `
	--resource-group $resourceGroup `
	--query properties.outputs `
	--output json)
	
if ($outputs) {
	# Log and convert to JSON
    Write-Output "> Successfully got deployment outputs. Take a note of these settings and add them to your appsettings.json file." 
	
	$outputs = $outputs | ConvertFrom-Json -Depth 100
	$outputMap = @{}
	$outputs.PSObject.Properties | Foreach-Object { $outputMap[$_.Name] = $_.Value }

	foreach ($key in $outputMap.Keys) { 
		Write-Output "$($key): $($outputMap[$key].value)"
	}
}
else {
	Write-Output "Could not get deployment outputs."
}

Write-Output "> Finished!"