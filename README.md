# Wanwan Drop Blaster

一个面向 Android 的 Unity 2D 竖屏小游戏：玩家拖动底部炮台左右移动，自动发射子弹，击碎从顶部落下的可爱方块，守住底部基地并尽量拿高分。

## 已实现内容

- `Menu / Game / GameOver` 三个场景
- 竖屏运行与单指拖动横向移动
- 自动连发子弹
- 普通方块与强化方块
- 命中、爆炸、基地受击反馈
- 基于存活时间的难度提升
- 分数、生命、结算、最高分记录
- Unity EditMode 测试样例

## 项目结构

- `Assets/Scenes`
  - `Menu.unity`
  - `Game.unity`
  - `GameOver.unity`
- `Assets/Scripts/Bootstrap`
  - 各场景启动入口
- `Assets/Scripts/Runtime`
  - 游戏逻辑与运行时生成内容
- `Assets/Scripts/UI`
  - 运行时 UI 构建辅助
- `Assets/Tests/EditMode`
  - 纯逻辑层测试

## 在 Unity 中打开

1. 用 Unity Hub 打开当前目录。
2. 如果 Unity 提示升级项目，允许升级到本机安装版本。
3. 在 `Build Settings` 中确认场景顺序：
   - `Assets/Scenes/Menu.unity`
   - `Assets/Scenes/Game.unity`
   - `Assets/Scenes/GameOver.unity`
4. 切换平台到 `Android`。
5. 在 `Player Settings` 中确认 `Default Orientation = Portrait`。
6. 运行 `Menu` 场景开始测试。

## 自动化命令

- 校验工程结构：
  - `python3 scripts/validate_unity_project.py`
- 跑 Unity EditMode 测试：
  - `./scripts/run_editmode_tests.sh`
- 用 Unity Hub CLI 安装 Editor + Android 模块：
  - `./scripts/install_unity_editor.sh`
- 构建 Android Debug APK：
  - `./scripts/build_android.sh`
- 构建 Android 签名 Release APK：
  - `./scripts/build_android_release.sh`
- 安装到已连接手机并做冒烟启动：
  - `./scripts/android_smoke_test.sh`

如果 Unity 不在默认路径，先指定：

```bash
export UNITY_BIN="/Applications/Unity/Hub/Editor/<version>/Unity.app/Contents/MacOS/Unity"
```

## 建议的 Unity 内验证

- EditMode 测试：
  - `DifficultyProgressionTests`
  - `SessionStateTests`
- PlayMode 手测：
  - 炮台不会移出屏幕
  - 方块会随机掉落并逐步加速
  - 强化方块需要更多命中
  - 方块落到底部会扣生命
  - 生命归零后跳转到结算场景

## 当前环境限制

当前这台机器已经检测到一台已连接 Android 手机：

- 设备型号：`2211133C`
- Android 版本：`15`

当前环境仍然没有安装 Unity Editor、`dotnet` 或 Mono，所以我在这里还不能直接完成 Unity 编译与真机打包；但 `adb` 已可用，等 Unity 安装后就可以直接执行上面的构建与真机冒烟脚本。
