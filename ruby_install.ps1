$rubies = @(
    @{
        "version"      = "Ruby 2.6.9-1"
        "install_path" = "C:\Ruby26"
        "download_url" = "https://github.com/oneclick/rubyinstaller2/releases/download/RubyInstaller-2.6.9-1/rubyinstaller-2.6.9-1-x86.exe"
        "devkit_url"   = ""
        "devkit_paths" = @()
        "bundlerV2"    = $true
    }    
    @{
        "version"      = "Ruby 2.6.9-1 (x64)"
        "install_path" = "C:\Ruby26-x64"
        "download_url" = "https://github.com/oneclick/rubyinstaller2/releases/download/RubyInstaller-2.6.9-1/rubyinstaller-2.6.9-1-x64.exe"
        "devkit_url"   = ""
        "devkit_paths" = @()
        "bundlerV2"    = $true
    }
    @{
        "version"      = "Ruby 2.7.8-1"
        "install_path" = "C:\Ruby27"
        "download_url" = "https://github.com/oneclick/rubyinstaller2/releases/download/RubyInstaller-2.7.8-1/rubyinstaller-2.7.8-1-x86.exe"
        "devkit_url"   = ""
        "devkit_paths" = @()
        "bundlerV2"    = $true
    }    
    @{
        "version"      = "Ruby 2.7.8-1 (x64)"
        "install_path" = "C:\Ruby27-x64"
        "download_url" = "https://github.com/oneclick/rubyinstaller2/releases/download/RubyInstaller-2.7.8-1/rubyinstaller-2.7.8-1-x64.exe"
        "devkit_url"   = ""
        "devkit_paths" = @()
        "bundlerV2"    = $true
    }        
)

function UpdateRubyPath($rubyPath) {
    $env:path = ($env:path -split ';' | Where-Object { -not $_.contains('\Ruby') }) -join ';'
    $env:path = "$rubyPath;$env:path"
}

function Install-Ruby($ruby) {
    Write-Host "Installing $($ruby.version)" -ForegroundColor Cyan

    # uninstall existing
    $rubyUninstallPath = "$ruby.install_path\unins000.exe"
    if ([IO.File]::Exists($rubyUninstallPath)) {
        Write-Host "  Uninstalling previous Ruby 2.4..." -ForegroundColor Gray
        "`"$rubyUninstallPath`" /silent" | out-file "$env:temp\uninstall-ruby.cmd" -Encoding ASCII
        & "$env:temp\uninstall-ruby.cmd"
        del "$env:temp\uninstall-ruby.cmd"
        Start-Sleep -s 5
    }

    if (Test-Path $ruby.install_path) {
        Write-Host "  Deleting $($ruby.install_path)" -ForegroundColor Gray
        Remove-Item $ruby.install_path -Force -Recurse
    }

    $exePath = "$($env:TEMP)\rubyinstaller.exe"

    Write-Host "  Downloading $($ruby.version) from $($ruby.download_url)" -ForegroundColor Gray
    (New-Object Net.WebClient).DownloadFile($ruby.download_url, $exePath)

    Write-Host "Installing..." -ForegroundColor Gray
    cmd /c start /wait $exePath /verysilent /allusers /dir="$($ruby.install_path.replace('\', '/'))" /tasks="noassocfiles,nomodpath,noridkinstall"
    del $exePath
    Write-Host "Installed" -ForegroundColor Green

    # setup Ruby
    UpdateRubyPath "$($ruby.install_path)\bin"
    Write-Host "ruby --version" -ForegroundColor Gray
    cmd /c ruby --version

    Write-Host "gem --version" -ForegroundColor Gray
    cmd /c gem --version

    # list installed gems
    Write-Host "gem list --local" -ForegroundColor Gray
    cmd /c gem list --local

    # delete temp path
    if ($tempPath) {
        Write-Host "  Cleaning up..." -ForegroundColor Gray
        Remove-Item $tempPath -Force -Recurse
    }

    Write-Host "  Done!" -ForegroundColor Green
}

for ($i = 0; $i -lt $rubies.Count; $i++) {
  Install-Ruby $rubies[$i]
}