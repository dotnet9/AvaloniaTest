# 更新日志

V2.0.2（2026-05-02）

- 😄[新增]-新增 `publish_music_win-x64.bat`，用于单独发布 `Avalonia.MusicStore` 的 `win-x64` 版本，便于针对 AOT 场景快速验证
- 🔤[优化]-继续精简发布配置，移除多余的 `.pubxml` 配置项，并补充公共发布片段与项目级发布片段，降低多平台发布维护成本
- 🔤[优化]-修复 `Avalonia.MusicStore` 在 `win-x64` 的 `AOT + 裁剪` 发布产物中执行专辑搜索时的 `System.Text.Json` 运行时异常，将反射式序列化切换为源码生成上下文

V2.0.1（2026-05-01）

- 😄[新增]-新增根目录更新日志文件，用于持续记录仓库升级、结构调整和依赖变更
- 😄[新增]-新增根目录 `AvaloniaTest.slnx`、`Directory.Build.props`、`Directory.Packages.props` 和 `global.json`，统一解决方案入口、公共构建参数、中央包版本管理与 SDK 版本
- 😄[新增]-恢复并保留 ReactiveUI 升级方案，引入 `ReactiveUI 23.2.1` 与 `ReactiveUI.Avalonia 12.0.1`，完成两个示例项目在 Avalonia 12 下的适配
- 🔤[优化]-将两个示例项目统一升级到 `.NET 11` 和 `Avalonia 12.0.2`，同步修正 Avalonia 12 的主题配置、绑定写法和调试工具接入方式
- 🔤[优化]-重构 `Avalonia.MusicStore` 的专辑搜索流程，移除旧版 `iTunesSearch` 依赖，改为直接调用 Apple Search API 以提升兼容性
- 🔤[优化]-更新中英文界面文案与仓库说明，补充仓库用途、目录结构和常用命令，并完成 `dotnet build .\AvaloniaTest.slnx` 构建验证
- 🔤[优化]-将 `win-x64` 与 `linux-x64` 的单文件、裁剪和 AOT 发布参数下沉到各项目 `PublishProfiles` 的 `.pubxml` 文件中，批处理脚本仅保留 profile 调度与失败回退逻辑
- 🔤[优化]-为两个示例项目新增独立的 `Properties/Trimming/TrimmerRoots.xml` 裁剪保留配置，显式将当前程序集排除在裁剪范围外，降低 Avalonia 反射绑定与 ReactiveUI 表达式访问在裁剪发布下的风险
- 🔤[优化]-收敛发布配置为每个平台仅保留一个 `.pubxml`，其中 `win-x64` 使用 `AOT + 裁剪 + 单文件`，`linux-x64` 使用 `单文件 + 裁剪`，并同步移除批处理脚本中的多档回退逻辑
- 🔤[优化]-补充共享发布 `.pubxml` 片段，抽离公共发布属性与项目级裁剪项，降低四个平台 profile 的重复配置成本
- 🔤[优化]-将 `Avalonia.MusicStore` 中 `System.Text.Json` 的反射式序列化切换为源码生成上下文，修复 `AOT + 裁剪` 发布后搜索专辑时的运行时异常
