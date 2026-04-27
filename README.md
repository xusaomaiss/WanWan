# Wanwan Drop Blaster

一个面向 Android 的 Unity 2D 竖屏雷电风格射击游戏：玩家驾驶战机从航母起飞，在 8 个主题空域中自由移动、自动开火、收集金币与火力道具，依靠炸弹清屏、能量胶囊升级和连击倍率击败多阶段 Boss。本项目定位为单人单机游戏，所有进度、设置和排行榜都保存在本机 `PlayerPrefs`，不做联网、账号、云存档、在线排行榜或多人联机功能。

详细玩法说明见 [docs/gameplay-features.md](docs/gameplay-features.md)。

## 已实现内容

- `Menu / Game / GameOver` 三个 Unity 场景，竖屏运行，Android 包名 `com.mark.wanwan.dropblaster`，版本 `1.0.1` / `versionCode 2`。
- 单指触摸或鼠标拖动控制战机在全屏 X/Y 范围内移动，自动连发子弹。
- 绿色/蓝色战机选择、简单/普通/困难难度选择、按难度分类的本地排行榜、三字母榜名、设置、暂停菜单、结算页和本地自动保存。
- 8 个雷电风格关卡主题：乡村、城市、海岸线、荒漠遗迹、赤褐荒地、浮空大陆、空间站、外星基地；通关后进入下一关并支持循环难度成长。
- 阶段化关卡脚本：`Preparation / Assault / Pressure / Elite / Boss / Clear`，不同关卡使用不同编队风格和 Boss 弹幕风格。
- Wave 节奏循环：`Calm -> Pressure -> Burst -> Reward`，动态调节刷怪压力、奖励航线和金币掉落倍率。
- 四种常驻武器：扇形弹、激光弹、追踪弹、爆裂弹；火力等级 `1~4`，同武器升级、不同武器切换。
- 能量胶囊 / Power Meter：`SPEED / MISSILE / DOUBLE / LASER / OPTION / SHIELD` 高亮推进，点击底部“升级”按钮激活并重置能量槽。
- 敌机击毁后掉落金币，金币自动吸附玩家并转化为分数；每 100 金币生成一个场上炸弹道具，触碰后立即清屏。
- 漏掉敌机扣 4 个金币，金币不足时扣到 0，不再触发基地失守。
- 玩家拥有 10 格 HP；敌弹命中或敌机/Boss 碰撞各扣 1 格，护盾可抵消伤害，受击后短暂无敌，非致命受击会重置火力并掉落 1~2 个恢复火力包，HP 归零失败。
- 普通敌机、强化敌机、精英敌机、敌方导弹、多阶段 Boss、Combo 连击倍率、Boss 阶段奖励和胜利/失败结算。
- AI 资源驱动的菜单风暴主视觉、4.2 秒航母起飞天气层、8 帧起飞战机序列、8 张关卡背景、24 张近景地面贴片、12 帧爆炸序列、HUD 装饰、敌机/Boss/金币/子弹/爆炸资源。
- 运行时性能预算：目标 60 FPS，关闭垂直同步、抗锯齿和阴影，Android 纹理与内存预算由 `PerformanceBudget` 约束，并通过“特效质量：完整/省电”控制近景贴片、粒子、爆炸级联和震动强度。
- Unity EditMode 测试覆盖奖励经济、武器、连击、关卡、Wave、Power Meter、性能预算、菜单、视觉特效预算和资源路径等核心逻辑。

## 项目结构

- `Assets/Scenes`
  - `Menu.unity`、`Game.unity`、`GameOver.unity`
- `Assets/Scripts/Bootstrap`
  - 场景启动入口、相机/背景/玩家/UI 运行时装配
- `Assets/Scripts/Runtime`
  - 游戏流程、关卡生成、奖励经济、Boss、子弹、道具、性能预算和运行时精灵工厂
- `Assets/Scripts/Runtime/Weapons`
  - 四种玩家武器的配置、状态和弹幕模式
- `Assets/Scripts/Runtime/Wave`
  - `Calm / Pressure / Burst / Reward` 节奏循环
- `Assets/Scripts/Runtime/Combo`
  - 连击数、倍率和最高连击统计
- `Assets/Scripts/UI`
  - 运行时 UI 构建、主题、HUD、菜单和按钮反馈
- `Assets/Resources/RaidenArt`
  - AI/程序化像素资源：关卡背景、近景地面贴片、战机、敌机、Boss、金币、子弹、爆炸序列、HUD 装饰、菜单和起飞过场资源
- `Assets/Editor/BuildAutomation.cs`
  - Unity 批处理测试、Android Debug/Release 构建和 Android PlayerSettings 配置
- `Assets/Tests/EditMode`
  - 纯逻辑层和资源路径测试
- `docs/gameplay-features.md`
  - 当前玩法系统、数值配置与验证说明
- `Kimi_Agent_雷电手游UI方案`
  - 早期雷电参考方案与当前 Unity 落地状态说明

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

- 校验工程结构和关键资源：
  - `python3 scripts/validate_unity_project.py`
- 跑 Unity EditMode 测试：
  - `./scripts/run_editmode_tests.sh`
- 用 Unity Hub CLI 安装 Editor + Android 模块：
  - `./scripts/install_unity_editor.sh`
- 构建 Android Debug APK（只用于干净安装或没有同包名旧版本时的临时测试）：
  - `./scripts/build_android.sh`
- 构建 Android 签名 Release APK：
  - `./scripts/build_android_release.sh`
- 安装到已连接手机并做冒烟启动（默认安装 `Builds/Android/WanwanDropBlaster-release.apk`）：
  - `./scripts/android_smoke_test.sh`

## Android 真机更新

真机已有 `com.mark.wanwan.dropblaster` 时，更新包必须和设备上现有应用使用同一套签名。不要用 debug APK 覆盖 release 版本；Android 会因为同包名签名不一致拦截安装，而且为了保留本机进度和 `PlayerPrefs`，也不应先卸载再安装。

以后更新到真机时直接走 release 同签名流程：

```bash
./scripts/build_android_release.sh
./scripts/android_smoke_test.sh
```

`build_android_release.sh` 不会自动生成 release keystore，也不内置任何签名密码；必须显式设置 `WANWAN_ANDROID_KEYSTORE`、`WANWAN_ANDROID_KEYSTORE_PASS`、`WANWAN_ANDROID_KEYALIAS` 和 `WANWAN_ANDROID_KEYALIAS_PASS`，且 keystore 文件必须已存在。`android_smoke_test.sh` 默认安装 `Builds/Android/WanwanDropBlaster-release.apk`，因此会按同签名 release 包更新并保留设备数据。

如果 Unity 不在默认路径，先指定：

```bash
export UNITY_BIN="/Applications/Unity/Hub/Editor/<version>/Unity.app/Contents/MacOS/Unity"
```

Release 构建需要提前提供签名环境变量：

```bash
export WANWAN_ANDROID_KEYSTORE="/path/to/wanwan.keystore"
export WANWAN_ANDROID_KEYSTORE_PASS="..."
export WANWAN_ANDROID_KEYALIAS="..."
export WANWAN_ANDROID_KEYALIAS_PASS="..."
```

## 建议的 Unity 内验证

- EditMode 测试：
  - `GameplayRewardStateTests`
  - `FireLevelStateTests`
  - `WeaponPickupStateTests`
  - `PowerMeterStateTests`
  - `WaveDirectorTests`
  - `StageCatalogTests`
  - `ComboStateTests`
  - `PerformanceBudgetTests`
  - `MenuBootstrapTests`
  - `RaidenEnemySpriteTests`
- PlayMode 手测：
  - 标题页显示风暴主视觉，排行榜、设置、保存、开始、退出入口可点击。
  - 设置页可切换声音、音量、按键透明度、默认战机、默认难度、灵敏度、震动、伤害数字和特效质量。
  - 战机可以在屏幕内任意移动但不会越界，航母起飞过场可点击跳过。
  - 击毁敌机会掉落 4 个金币，金币靠近玩家后自动吸附并更新顶部金币和分数。
  - 每 100 金币生成炸弹图标，吃到后立即清屏并播放全屏特效。
  - 完整特效模式下可看到更密集的近景地面贴片、爆炸序列和 Boss 连锁爆炸；省电模式下贴片与粒子数量下降但 HUD 信息保持完整。
  - 火力包会在扇形弹、激光弹、追踪弹、爆裂弹之间循环；同武器提升火力等级。
  - 能量胶囊推进底部 Power Meter，高亮 `SPEED / MISSILE / DOUBLE / LASER / OPTION / SHIELD`，点击“升级”后生效并重置。
  - Wave 进入 Burst / Reward 时出现对应提示，Reward 阶段压力降低且奖励更密集。
  - 漏掉敌机只扣金币并中断 Combo，不出现基地失守。
  - 被敌弹或敌机碰撞会扣 HP 或消耗护盾，HP 归零才失败。
  - Boss 警报、Boss 血条、阶段 Combo 奖励、胜利结算和失败结算正常。

## 当前环境限制

当前仓库的自动化脚本会优先查找 Unity `6000.3.7f1`、`6000.0.0f1`、`2022.3.62f1` 等常见安装路径。如果 Unity 不在默认路径，请设置 `UNITY_BIN` 后再运行测试或构建命令。Android 真机安装可能受设备厂商安全确认影响，烟测脚本仍需要一台已授权的 adb 设备。
