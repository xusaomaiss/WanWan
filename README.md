# Wanwan Drop Blaster

一个面向 Android 的 Unity 2D 竖屏雷电风格射击游戏：玩家驾驶战机在竖屏空域内自由移动，自动开火击落敌机，收集金币、升级火力、吃取炸弹道具清屏，并挑战多阶段 Boss。

## 已实现内容

- `Menu / Game / GameOver` 三个场景
- 竖屏运行与单指拖动全屏自由移动
- 自动连发子弹，支持 `fireLevel 1~4` 火力等级
- 普通敌机、强化敌机、精英敌机、敌方导弹与多阶段 Boss
- 敌机击毁后掉落金币，金币会自动吸附玩家并转化为分数
- 每 100 金币生成一个场上炸弹道具，玩家触碰后立即清屏
- 火力包循环显示不同武器，拾取后升级火力并改变弹幕形态
- 航母起飞开场、关卡阶段推进、Boss 警报、胜利/失败结算
- 命中、爆炸、基地受击反馈
- 基于存活时间、关卡和循环数的难度提升
- 分数、生命、结算、最高分记录
- Unity EditMode 测试覆盖核心纯逻辑

详细玩法说明见 [docs/gameplay-features.md](docs/gameplay-features.md)。

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
- `docs/gameplay-features.md`
  - 当前玩法系统、数值配置与验证说明

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
  - `GameplayRewardStateTests`
  - `FireLevelStateTests`
  - `EnemySpawnBudgetTests`
  - `StageClearTargetTests`
- PlayMode 手测：
  - 战机可以在屏幕内任意移动但不会越界
  - 击毁敌机会掉落 4 个金币，金币靠近玩家后自动吸附
  - 金币收集后顶部金币和分数会更新
  - 每 100 金币生成炸弹图标，吃到后立即清屏
  - 火力包会让子弹从单发逐步升级到多排扩散
  - Boss 警报、Boss 血条、胜利结算和失败结算正常

## 当前环境限制

当前仓库的自动化脚本会优先查找 Unity `6000.3.7f1`、`6000.0.0f1`、`2022.3.62f1` 等常见安装路径。如果 Unity 不在默认路径，请设置 `UNITY_BIN` 后再运行测试或构建命令。
