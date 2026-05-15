# build.ps1

# 1. 确保版本文件存在
if (-not (Test-Path "version.txt")) {
    "1.0.0" | Out-File "version.txt"
    Write-Host "Created initial version file"
}

# 2. 生成新版本
$currentVersion = Get-Content "version.txt"
$parts = $currentVersion -split '\.'
$patch = [int]($parts[2]) + 1

# 基础版本号
$baseVersion = "$($parts[0]).$($parts[1]).$patch"

# 获取当前日期时间
$currentDateTime = Get-Date -Format "yyyyMMddHHmm"

# 带时间戳的版本
$timestampedVersion = "$baseVersion-$currentDateTime"

# 保存基础版本号（不含时间戳）
$baseVersion | Out-File "version.txt"

# 3. 打印版本信息
Write-Host "Base version: $baseVersion"
Write-Host "Timestamped version: $timestampedVersion"

# 4. 构建 Docker 镜像并添加多个标签
$imageName = "yjj6731/tshotel-management-system-api"

# 构建命令
docker build `
    -t "$imageName`:$timestampedVersion" `
    -t "$imageName`:latest" .

# 5. 导出镜像到当前文件夹
# 创建导出目录（如果不存在）
$exportDir = "docker-images"
if (-not (Test-Path $exportDir)) {
    New-Item -ItemType Directory -Path $exportDir | Out-Null
}

# 导出带时间戳的版本
$exportFileName = "tshotel-management-system-api_$($timestampedVersion).tar"
Write-Host "Exporting image to $exportDir\$exportFileName"
docker save -o "$exportDir\$exportFileName" "$imageName`:$timestampedVersion"

# 计算导出文件大小
$fileSize = (Get-Item "$exportDir\$exportFileName").Length / 1MB
$fileSize = [Math]::Round($fileSize, 2)

# 5. 可选的推送命令
# docker push "$imageName`:$baseVersion"
# docker push "$imageName`:$timestampedVersion"
# docker push "$imageName`:latest"

Write-Host ""
Write-Host "Build completed:"
Write-Host " - Base version: $baseVersion"
Write-Host " - Timestamped version: $timestampedVersion"
Write-Host " - Latest"
Write-Host " - Exported to: $exportDir\$exportFileName ($fileSize MB)"

Write-Host "Build complete. Press any key to exit..."
[void][System.Console]::ReadKey($true)
exit