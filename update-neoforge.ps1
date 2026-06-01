# ==================================================
# NeoForge Updater Example
# ==================================================

$ManifestUrl = "https://raw.githubusercontent.com/Kryst-31/Minecraft-Create-Industries-Modpack/main/manifest.json"

$LocalVersionFile = ".\versions.json"
$InstallerDirectory = ".\temp"

Write-Host "Checking for updates..."

# Download manifest
try {
    $Manifest = Invoke-WebRequest -Uri $ManifestUrl -UseBasicParsing | ConvertFrom-Json
}
catch {
    Write-Error "Failed to download manifest."
    exit 1
}

# Read local version
if (Test-Path $LocalVersionFile) {
    $Local = Get-Content $LocalVersionFile | ConvertFrom-Json
}
else {
    $Local = @{
        neoforgeVersion = "none"
    }
}

$CurrentVersion = $Local.neoforgeVersion
$RequiredVersion = $Manifest.neoforgeVersion

Write-Host "Installed NeoForge: $CurrentVersion"
Write-Host "Required NeoForge:  $RequiredVersion"

if ($CurrentVersion -eq $RequiredVersion) {
    Write-Host "NeoForge is up to date."
    exit 0
}

Write-Host "Updating NeoForge..."

# Create temp directory
New-Item `
    -ItemType Directory `
    -Force `
    -Path $InstallerDirectory | Out-Null

# NeoForge installer URL
$InstallerUrl =
    "https://maven.neoforged.net/releases/net/neoforged/neoforge/$RequiredVersion/neoforge-$RequiredVersion-installer.jar"

$InstallerFile =
    Join-Path $InstallerDirectory "neoforge-installer.jar"

Write-Host "Downloading installer..."
Invoke-WebRequest `
    -Uri $InstallerUrl `
    -OutFile $InstallerFile

# Verify Java exists
try {
    $null = java -version 2>&1
}
catch {
    Write-Error "Java not found in PATH."
    exit 1
}

Write-Host "Running installer..."

# Install client profile
java -jar $InstallerFile --install-client

if ($LASTEXITCODE -ne 0) {
    Write-Error "NeoForge installer failed."
    exit 1
}

# Save installed version
@{
    neoforgeVersion = $RequiredVersion
} | ConvertTo-Json | Set-Content $LocalVersionFile

Write-Host "NeoForge updated successfully."
