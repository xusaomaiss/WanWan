# 玩法功能说明

本文档描述当前 Unity 版 `Wanwan Drop Blaster` 的雷电风格核心玩法，方便后续功能迭代、测试和调参。项目定位是单人单机 Android 游戏，所有设置、进度、榜名和排行榜只保存在本机 `PlayerPrefs`；不规划联网、账号、云存档、在线排行榜或多人联机功能。

## 核心循环

玩家从航母起飞后进入竖屏战斗空域，通过触摸或鼠标拖动控制战机在屏幕内自由移动。战机会自动射击，玩家需要躲避敌机和敌方子弹，击落敌机获得金币与火力包，并在 Boss 阶段完成关卡。

当前主要循环是：

1. 躲避敌机、敌弹和 Boss 弹幕。
2. 自动射击击毁敌机。
3. 收集敌机掉落的金币。
4. 金币累计后生成炸弹道具。
5. 飞向炸弹道具触发清屏。
6. 吃火力包提升火力等级。
7. 收集能量胶囊推进 Power Meter，并在合适时机激活升级。
8. 推进关卡阶段并击败 Boss。
9. 通关时结算 Boss 分、通关奖励、剩余炸弹/护盾奖励和最高倍率奖励。

## 移动系统

- 玩家战机支持 X/Y 双轴移动，不再固定在屏幕底部。
- 输入方式：
  - 移动端：单指触摸位置。
  - 编辑器/桌面：鼠标按住拖动位置。
- 战机位置会被限制在 `LeftBound / RightBound / BottomBound / TopBound` 内，并预留机体边距，避免越出屏幕。
- 横向边界使用屏幕可见范围作为基础，只保留机体半宽内缩，真机上可飞到最左和最右边缘附近。

相关代码：

- `Assets/Scripts/Runtime/PlayerController.cs`
- `Assets/Scripts/Bootstrap/GameBootstrap.cs`

## 开场与开始菜单

游戏开局使用 AI 生成的关键画面 `launch_weather_intro_ai` 作为 `4.2` 秒起飞天气层引导：

- 战机从航母甲板加速进入云层。
- 背景滚动速度、喷焰粒子、速度线和白光过渡同步增强。
- 起飞过程叠加 AI 生成的序列帧战机动画 `launch_takeoff_frame_00~07`，让喷焰从待机推进到高速离舰。
- 玩家可在短暂保护时间后点击或触摸跳过。

开始菜单使用 AI 生成的 `menu_storm_title_ai` 作为风暴云层主视觉，并将标题、主按钮、排行榜、设置、保存和退出入口重新分层，突出“开始出击”。

菜单侧当前已落地：

- 标题页：圆形“保存 / 开始 / 退出”按钮，左上排行榜入口，右上设置入口，底部显示最高分。
- 战机选择：绿色战机与蓝色战机，分别对应散射/激光主武器取向和不同速度/火力星级。
- 难度选择：简单、普通、困难，影响敌弹、敌机、Boss 与评分档位。
- 设置页：总声音、音乐音量、音效音量、按键透明度、默认战机、默认难度、操控灵敏度、震动、伤害数字、特效质量和恢复默认。
- 本地状态：最高分、按难度分类的排行榜、三字母榜名、当前关卡/循环、默认战机、默认难度和设置项通过 `PlayerPrefs` 保存。

相关代码：

- `Assets/Scripts/Runtime/CarrierLaunchIntroController.cs`
- `Assets/Scripts/Runtime/CarrierLaunchIntroConfig.cs`
- `Assets/Scripts/Bootstrap/MenuBootstrap.cs`
- `Assets/Scripts/Runtime/SessionState.cs`
- `Assets/Scripts/Runtime/RuntimeSpriteFactory.cs`

## 节奏系统（Wave System）

战斗中会叠加一套轻量节奏循环，让敌机压力不再持续平铺，而是按 `Calm → Pressure → Burst → Reward` 循环变化。默认总周期约 `19` 秒：

| 节奏阶段 | 时长 | 体验特点 |
| --- | ---: | --- |
| `Calm` | `5s` | 低刷怪率，偏普通下落敌机，给玩家整理位置。 |
| `Pressure` | `6s` | 中等刷怪，更多俯冲与侧翼切入，少量强化/精英压力。 |
| `Burst` | `5s` | 高刷怪率，编队类型全开，强化/精英概率提高，并显示 `DANGER` 提示。 |
| `Reward` | `3s` | 敌机压力显著下降，显示 `BONUS` 提示，金币掉落提升并更容易看到火力包。 |

Reward 阶段敌机金币掉落按默认值的 `1.5x` 计算，并会在每次 Reward 进入后保证至少出现一次额外火力包机会。

相关代码：

- `Assets/Scripts/Runtime/Wave/WaveDirector.cs`
- `Assets/Scripts/Runtime/GameManager.cs`
- `Assets/Scripts/Runtime/BlockSpawner.cs`

## 金币与分数系统

敌机被击毁后会掉落金币：

- 每架敌机掉落 `4` 个金币。
- 每个金币价值 `5` 分。
- 金币在玩家进入吸附范围后自动飞向玩家。
- 金币进入收集范围后立即计入金币数和分数。

默认配置位于 `GameplayRewardConfig.Default`：

| 配置 | 默认值 | 说明 |
| --- | ---: | --- |
| `CoinsPerEnemy` | `4` | 每架敌机掉落金币数 |
| `ScorePerCoin` | `5` | 每个金币分值 |
| `CoinsLostPerEscapedEnemy` | `4` | 每架逃脱敌机扣除金币数 |
| `CoinsPerBomb` | `100` | 生成一个炸弹道具所需金币 |
| `MaxBombsPerStage` | `3` | 单关最多通过金币获得的炸弹数 |
| `CoinMagnetRadiusWorld` | `2.5` | 金币开始吸附的世界距离，约等于 150px 体验 |
| `CoinCollectRadiusWorld` | `0.5` | 金币自动收集距离，约等于 30px 体验 |

相关代码：

- `Assets/Scripts/Runtime/GameplayRewardConfig.cs`
- `Assets/Scripts/Runtime/GameplayRewardState.cs`
- `Assets/Scripts/Runtime/CoinController.cs`
- `Assets/Scripts/Runtime/BlockController.cs`

敌机从屏幕底部逃脱时不再触发基地失守，而是扣除 `4` 个金币；金币不足时扣到 `0`。该扣减只影响当前金币数量，不回滚已获得分数或已生成炸弹。

## 玩家 HP 系统

- 玩家每局初始 `10` 格 HP。
- 敌方子弹命中玩家扣 `1` 格 HP。
- 敌机或 Boss 碰撞玩家扣 `1` 格 HP。
- 受伤后进入短暂无敌闪烁窗口，避免连续碰撞或密集弹幕瞬间多段扣血。
- 护盾会优先吸收一次伤害，并触发更短的闪烁保护。
- 非致命受击会执行类街机死亡惩罚：当前武器重置为 1 级扇形弹，并在玩家附近掉落 1~2 个恢复火力包，方便玩家重新拾取补强。
- HP 归零后本局失败，失败文案为战机损毁类提示。

相关代码：

- `Assets/Scripts/Runtime/PlayerHealthState.cs`
- `Assets/Scripts/Runtime/PlayerInvulnerabilityState.cs`
- `Assets/Scripts/Runtime/Weapons/PlayerWeaponState.cs`
- `Assets/Scripts/Runtime/GameManager.cs`
- `Assets/Scripts/Runtime/EnemyFireballController.cs`

## 炸弹系统

炸弹不再通过按钮或空格释放，而是作为地图道具出现：

- 每累计 `100` 个金币，随机生成一个场上炸弹图标。
- 单关最多生成 `3` 个金币奖励炸弹。
- 玩家触碰炸弹图标后立即释放炸弹。
- 炸弹会清除敌方子弹和普通敌机。
- 被炸弹清除的敌机仍按击毁处理，会计入击落数并掉落金币。
- Boss 不会被直接秒杀，只受到固定炸弹伤害，避免破坏 Boss 战节奏。

相关代码：

- `Assets/Scripts/Runtime/BombPickupController.cs`
- `Assets/Scripts/Runtime/GameManager.cs`
- `Assets/Scripts/Runtime/EffectsController.cs`

## 火力与子弹系统

玩家拥有 `fireLevel`，范围为 `1~4`，默认 `1`。拾取火力包后提升一级，最高不超过 `4`。

火力系统由“主武器 + 2 个模块槽”组成。主武器决定基础弹幕，模块会叠加射速、贯穿、追踪、波形和护卫侧弹等效果；槽满后吃到新模块会替换最旧模块。

| 武器 | 显示名 | 间隔 | 速度 | 特性 |
| --- | --- | ---: | ---: | --- |
| `Spread` | 扇形弹 | `0.13s` | `26.5` | 默认雷电风格扩散弹，适合清理杂兵。 |
| `Laser` | 激光弹 | `0.12s` | `27.5` | 高速贯穿弹，基础伤害更高。 |
| `Burst` | 爆裂弹 | `0.18s` | `24.0` | 命中后产生小范围爆炸。 |
| `Plasma` | 等离子弹 | `0.20s` | `23.5` | 高伤害能量弹，爆炸半径更大。 |

模块包包括：

- `RapidFire`：缩短射击间隔并提高弹速。
- `Pierce`：让弹体增加贯穿次数。
- `Homing`：让部分弹体追踪最近敌机、地面目标或 Boss。
- `Wave`：让部分弹体产生横向波形运动。
- `Guard`：在两侧增加护卫侧弹，提升敌人多时的覆盖面。

火力包按 9 类可玩包轮播显示：`Scatter / RapidFire / Pierce / Laser / Homing / Burst / Wave / Plasma / Guard`。拾取主武器包会切换主武器，重复拾取当前主武器会提升 `fireLevel`；拾取模块包会进入模块槽。Reward 阶段更容易看到火力包，Pressure/Burst 阶段也有额外掉落概率。

相关代码：

- `Assets/Scripts/Runtime/FireLevelState.cs`
- `Assets/Scripts/Runtime/PlayerFirePattern.cs`
- `Assets/Scripts/Runtime/PowerupCycle.cs`
- `Assets/Scripts/Runtime/AmmoPackController.cs`
- `Assets/Scripts/Runtime/WeaponShotPresentation.cs`
- `Assets/Scripts/Runtime/Weapons/WeaponConfig.cs`
- `Assets/Scripts/Runtime/Weapons/WeaponModuleType.cs`
- `Assets/Scripts/Runtime/Weapons/WeaponShotPattern.cs`

## Power Meter 能量胶囊系统

敌机或奖励节奏会投放能量胶囊。玩家拾取胶囊后，底部 Power Meter 从左到右推进并高亮一个升级槽：

| 槽位 | 效果 |
| --- | --- |
| `SPEED` | 提升玩家速度等级，最高 `3` 级。 |
| `MISSILE` | 切换/升级爆裂弹火力。 |
| `DOUBLE` | 切回扇形弹并提升火力等级。 |
| `LASER` | 切换/升级激光弹。 |
| `OPTION` | 提升当前火力等级，表现为子机火力。 |
| `SHIELD` | 增加护盾层数，最多 `3` 层。 |

HUD 底部显示 `SPEED MISSILE DOUBLE LASER OPTION SHIELD`，当前高亮槽位会以 `>槽位<` 标记。只要存在高亮槽位，“充能”按钮会变为“升级”；点击后立即应用当前槽位效果，并将能量槽重置为未高亮状态。

相关代码：

- `Assets/Scripts/Runtime/PowerMeterState.cs`
- `Assets/Scripts/Runtime/PowerMeterUpgrade.cs`
- `Assets/Scripts/Runtime/GameManager.cs`
- `Assets/Scripts/UI/UIController.cs`

## 敌机、关卡与 Boss

关卡由 `BlockSpawner` 运行时生成，主要阶段包括：

- Preparation：准备/开场。
- Assault：基础编队进攻。
- Pressure：密集压制。
- Elite：精英敌机。
- Boss：Boss 战。
- Clear：通关。

为了支撑金币和炸弹循环，`EnemySpawnBudget` 会按奖励经济提高编队刷怪数量，确保一关有足够敌机让玩家看到金币吸附、火力升级和炸弹道具循环。

当前关卡目录由 `StageCatalog` 提供：

| 关卡 | 名称 | Boss | 战斗风格 | Boss 风格 |
| ---: | --- | --- | --- | --- |
| `1` | 乡村 | 乡村防卫旗舰 | 均衡突袭 | 标准旗舰 |
| `2` | 城市 | 城市防卫旗舰 | 侧翼包抄 | 交叉火线 |
| `3` | 海岸线 | 海峡拦截舰 | 密集蜂群 | 弹幕压制 |
| `4` | 荒漠遗迹 | 遗迹炮击舰 | 远程狙击 | 针状点射 |
| `5` | 赤褐荒地 | 峡谷压制舰 | 重装压制 | 重锤推进 |
| `6` | 浮空大陆 | 浮岛截击舰 | 高速截击 | 漂移机动 |
| `7` | 空间站 | 轨道封锁舰 | 螺旋封锁 | 轨道环绕 |
| `8` | 外星基地 | Cranassian核心 | 最终核心 | 核心决战 |

每个关卡绑定独立背景资源 `Assets/Resources/RaidenArt/Backgrounds/stage_XX_*.png`，并叠加 `Assets/Resources/RaidenArt/GroundDetails/stage_XX_detail_YY.png` 近景贴片，形成道路、建筑、海岸、遗迹、金属设施和外星基地细节的低空掠过感。背景速度倍率、近景贴片密度、关卡难度倍率、编队脚本和 Boss 弹幕风格共同形成差异。第 8 关通关后回到第 1 关并提升循环倍率。

Boss 使用多阶段血量和弹幕配置，低/中/高难度会影响 Boss 血量、敌弹速度、敌机生命和射击频率。

敌机美术分层：

- 普通敌机：沿用基础敌机资源，作为低威胁杂兵轮廓。
- 强化敌机：使用 AI 生成的重装拦截机资源 `tough_enemy_ai`。
- 精英敌机：使用 AI 生成的高速精英机资源 `elite_enemy_ai`。
- 地面坦克/炮台：随关卡背景向下滚动，能被玩家子弹、爆裂弹和炸弹清除，并会向玩家方向开火。
- Boss：使用 AI 生成的宽体旗舰资源 `boss_flagship_ai`。

地面目标会在 Pressure / Burst 等非奖励节奏中穿插出现，重装、最终、狙击和均衡风格关卡出现概率更高。坦克偏低血量、较慢射击；炮台血量和分值更高、射击更频繁。

相关代码：

- `Assets/Scripts/Runtime/BlockSpawner.cs`
- `Assets/Scripts/Runtime/GroundTargetController.cs`
- `Assets/Scripts/Runtime/GroundTargetProfile.cs`
- `Assets/Scripts/Runtime/EnemySpawnBudget.cs`
- `Assets/Scripts/Runtime/BossController.cs`
- `Assets/Scripts/Runtime/StageClearTarget.cs`
- `Assets/Scripts/Runtime/DifficultyProgression.cs`
- `Assets/Scripts/Runtime/RuntimeSpriteFactory.cs`

## HUD 与反馈

顶部 HUD 显示：

- 当前分数。
- 当前金币数量。
- 玩家 HP 血条。
- 炸弹图标数量。
- 难度、关卡进度、击落目标。
- 火力等级和当前武器状态。
- Power Meter 高亮槽位与“充能/升级”按钮。
- Pause 按钮、暂停菜单、Boss 警报和 Boss 血条。

主要反馈包括：

- 金币吸附动画。
- AI 生成的金色信用币图标，替换旧的纯黄色圆形金币。
- 火力升级 banner。
- 敌机爆炸和碎片，爆炸图使用 `Assets/Resources/RaidenArt/Effects/Explosions/explosion_frame_00~11.png` 序列帧。
- 分数弹出文字。
- 炸弹全屏闪光、冲击波和完整模式下的额外清屏爆炸。
- Boss 警报、Boss 血条、入场震动和击败连锁爆炸。
- 能量胶囊提示、Power Meter 高亮和护盾图标。
- HUD 使用 `Assets/Resources/RaidenArt/HUD` 下的装饰资源强化顶部状态框、底部 Power Meter 和警示框。

特效质量：

- `完整`：默认模式，使用较多近景贴片、完整粒子数量、较强震动和更多 Boss 爆炸级联。
- `省电`：设置页可切换，降低近景贴片密度、粒子数量、爆炸级联数量和震动强度，HUD 信息不降级。

相关代码：

- `Assets/Scripts/UI/UIController.cs`
- `Assets/Scripts/Runtime/EffectsController.cs`
- `Assets/Scripts/Runtime/RuntimeSpriteFactory.cs`

## 武器选择系统

火力包现在代表四种常驻武器，并按 `Spread → Laser → Homing → Burst → Spread` 循环显示。玩家吃到不同武器包时会切换到该武器并保留当前 `fireLevel`；吃到相同武器包时提升 `fireLevel`，最高仍为 `4`。

四种武器：

- Spread / 扇形弹：默认雷电风格弹幕，等级越高散射角度和弹数越强，适合清小怪。
- Laser / 激光弹：高速贯穿弹，单发伤害较高，等级越高激光数量或伤害越强。
- Homing / 追踪弹：自动寻找最近敌机或 Boss，等级越高追踪弹数量越多。
- Burst / 爆裂弹：命中后造成小范围爆炸，等级越高弹数或爆炸半径越强。

HUD 底部显示当前武器和等级，例如 `Weapon: 激光弹 Lv.2`。火力包使用运行时生成的图标、颜色和字母标签区分类型。

相关代码：

- `Assets/Scripts/Runtime/Weapons/WeaponType.cs`
- `Assets/Scripts/Runtime/Weapons/PlayerWeaponState.cs`
- `Assets/Scripts/Runtime/Weapons/WeaponShotPattern.cs`
- `Assets/Scripts/Runtime/AmmoPackController.cs`
- `Assets/Scripts/Runtime/PlayerController.cs`

## Combo 连击倍率系统

玩家连续击杀普通敌机会累计 Combo。Combo 越高，普通击杀和 Boss 击破得分倍率越高；金币转化分数暂时不吃倍率，避免金币和炸弹经济膨胀。

倍率规则：

| Combo | 倍率 |
| ---: | ---: |
| `0~9` | `x1` |
| `10~24` | `x2` |
| `25~49` | `x3` |
| `50~99` | `x4` |
| `100+` | `x5` |

Combo 变化：

- 普通敌机死亡：`combo +1`。
- Boss 阶段阈值被打穿：`combo +5`。
- Boss 最终死亡：`combo +10`。
- 玩家受敌弹伤害、与敌机碰撞受伤、敌机漏出底部或玩家死亡：Combo 清零。
- 拾取金币、武器包、炸弹不会中断 Combo。

HUD 顶部显示 `Combo: 23  x2`。GameOver / Victory 结算页显示 `Max Combo` 和 `Max Multiplier`。

相关代码：

- `Assets/Scripts/Runtime/Combo/ComboState.cs`
- `Assets/Scripts/Runtime/GameManager.cs`
- `Assets/Scripts/Runtime/BlockController.cs`
- `Assets/Scripts/Runtime/BossController.cs`
- `Assets/Scripts/UI/UIController.cs`

## 通关奖励系统

击败 Boss 或达到关卡清场目标后，结算会额外加入通关奖励：

| 奖励 | 默认值 |
| --- | ---: |
| Boss 基础分 | `10000` |
| 通关基础奖励 | `100000` |
| 剩余炸弹奖励 | 每个 `5000` |
| 剩余护盾奖励 | 每层 `8000` |
| 最高倍率奖励 | 每倍 `2500` |

相关代码：

- `Assets/Scripts/Runtime/ScoreRewardConfig.cs`
- `Assets/Scripts/Runtime/GameManager.cs`
- `Assets/Scripts/Runtime/BossController.cs`

## 自动化验证

常用验证命令：

```bash
python3 scripts/validate_unity_project.py
./scripts/run_editmode_tests.sh
```

关键测试：

- `GameplayRewardStateTests`：金币、分数和炸弹兑换规则。
- `PlayerHealthStateTests`：玩家 HP 扣减和归零规则。
- `PlayerControllerBoundsTests`：玩家横向可达边界。
- `FireLevelStateTests`：火力等级和弹幕数量。
- `WeaponPickupStateTests`：武器切换、同武器升级和等级上限。
- `WeaponDisplayTests`：当前武器和等级显示文本。
- `ProjectileConfigTests`：四种武器各等级弹数。
- `ComboStateTests`：连击、倍率边界、中断和 Boss 奖励。
- `EnemySpawnBudgetTests`：敌机数量满足奖励经济。
- `StageClearTargetTests`：关卡击落目标。
- `PowerupCycleTests`：火力包循环池。
- `BossPhaseConfigTests`：Boss 多阶段配置。
- `PowerMeterStateTests`：能量胶囊槽位推进、激活重置和 HUD 文本。
- `WaveDirectorTests`：`Calm -> Pressure -> Burst -> Reward` 时长和顺序。
- `StageCatalogTests`：8 关主题、循环、背景路径、战斗风格和 Boss 风格。
- `GroundTargetProfileTests`：地面坦克/炮台血量、分值和射击间隔随难度成长。
- `PlayerInvulnerabilityStateTests`：受击无敌窗口、倒计时和闪烁透明度。
- `ScoreRewardConfigTests`：通关奖励、剩余资源奖励和负数输入保护。
- `PerformanceBudgetTests`：帧率、内存阈值和运行时性能配置。
- `MenuBootstrapTests`：标题按钮字号、启动 Logo 状态和菜单文案。
- `RaidenEnemySpriteTests`：RaidenArt 敌机/Boss/金币/背景资源路径。
- `RaidenVisualUpgradeTests`：近景贴片、爆炸序列、HUD 装饰、特效质量预算和 HUD 布局常量。

PlayMode 手测建议：

1. 进入游戏后确认航母起飞、自由移动和自动射击正常。
2. 击毁敌机，确认每架敌机掉落 4 个金币。
3. 飞近金币，确认金币吸附并自动收集。
4. 累计 100 金币，确认地图出现炸弹图标。
5. 飞向炸弹，确认触碰后立即清屏并播放全屏特效。
6. 吃火力包，确认武器在扇形弹、激光弹、追踪弹、爆裂弹之间切换；连续吃同武器确认等级提升。
7. 收集能量胶囊，确认 Power Meter 依次高亮 `SPEED / MISSILE / DOUBLE / LASER / OPTION / SHIELD`，点击“升级”后效果生效并重置。
8. 进入 Burst / Reward 节奏，确认危险提示、奖励提示、刷怪压力和金币掉落变化。
9. 在 Pressure / Burst 中确认地面坦克或炮台随背景进入战场、能开火、能被子弹和炸弹清除。
10. 连续击杀敌机，确认 Combo 和倍率提升，普通击杀分数随倍率增加。
11. 漏掉敌机，确认扣金币并中断 Combo，且不会出现基地失守。
12. 被敌弹或敌机碰撞，确认 HP 扣减或护盾消耗、短暂无敌闪烁、Combo 清零，HP 归零才失败。
13. 推进到 Boss，确认 Boss 血条、阶段 Combo 奖励、最终击破 Combo 奖励、Boss 多段爆炸和通关结算奖励正常。
