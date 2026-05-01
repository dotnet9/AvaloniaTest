# AvaloniaTest

这是一个用于 Avalonia UI 示例项目升级与兼容性验证的仓库。
当前包含两个桌面示例：

- `Todo`：简单待办事项示例
- `Avalonia.MusicStore`：音乐商店示例

## 本次调整

- 解决方案入口调整为根目录的 `AvaloniaTest.slnx`
- 项目源码统一保留在 `src/` 目录下
- 公共构建配置集中到 `Directory.Build.props`
- NuGet 包版本集中到 `Directory.Packages.props`
- 目标框架升级为 `.NET 11`
- Avalonia 主包统一升级到 `12.0.2`
- 移除了阻碍 Avalonia 12 升级的旧依赖，并替换为兼容实现

## 仓库结构

```text
AvaloniaTest/
|- AvaloniaTest.slnx
|- Directory.Build.props
|- Directory.Packages.props
|- global.json
`- src/
   |- Avalonia.MusicStore/
   `- Todo/
```

## 环境要求

- `.NET SDK 11.0.100-preview.3` 或兼容的 .NET 11 预览版 SDK
- Windows 开发环境

## 常用命令

```powershell
dotnet restore .\AvaloniaTest.slnx
dotnet build .\AvaloniaTest.slnx
dotnet run --project .\src\Todo\Todo.csproj
dotnet run --project .\src\Avalonia.MusicStore\Avalonia.MusicStore.csproj
```

## 发布说明

```powershell
.\publish_all.bat
.\publish.bat "src\Todo" "win-x64"
```

- 发布参数统一放在各项目的 `Properties/PublishProfiles/*.pubxml` 中维护
- 裁剪相关参数统一放在各项目的 `Properties/PublishProfiles/*.pubxml` 中，裁剪保留描述文件通过 profile 引用 `Properties/Trimming/TrimmerRoots.xml`
- 共享发布参数通过导入公共 `.pubxml` 片段复用，平台 profile 只保留目标框架、RID、输出目录和少量差异项
- 每个平台只保留一个发布配置文件
- `win-x64` 固定使用 `AOT + 裁剪 + 单文件`
- `linux-x64` 固定使用 `单文件 + 裁剪`

## 仓库用途

这个仓库主要用于：

- 验证 Avalonia 升级后的兼容性
- 让小型示例项目持续跟进新版 .NET SDK
- 作为后续 Avalonia 实验和示例扩展的基础
