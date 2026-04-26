# 玩法功能说明

本文档描述当前 Unity 版 `Wanwan Drop Blaster` 的雷电风格核心玩法，方便后续功能迭代、测试和调参。

## 核心循环

玩家从航母起飞后进入竖屏战斗空域，通过触摸或鼠标拖动控制战机在屏幕内自由移动。战机会自动射击，玩家需要躲避敌机和敌方子弹，击落敌机获得金币与火力包，并在 Boss 阶段完成关卡。

当前主要循环是：

1. 躲避敌机、敌弹和 Boss 弹幕。
2. 自动射击击毁敌机。
3. 收集敌机掉落的金币。
4. 金币累计后生成炸弹道具。
5. 飞向炸弹道具触发清屏。
6. 吃火力包提升火力等级。
7. 推进关卡阶段并击败 Boss。

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
- HP 归零后本局失败，失败文案为战机损毁类提示。

相关代码：

- `Assets/Scripts/Runtime/PlayerHealthState.cs`
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

火力等级对应弹幕：

| 火力等级 | 子弹模式 |
| ---: | --- |
| `1` | 单发直线 |
| `2` | 左右双发 |
| `3` | 三发扇形 |
| `4` | 多排扩散，当前为 7 发组合 |

火力包仍保留红/蓝/紫武器身份，会在同类别内循环显示。拾取后激活当前显示的武器类型，并提升火力等级。不同武器可影响子弹视觉、穿透、追踪或波形运动。

相关代码：

- `Assets/Scripts/Runtime/FireLevelState.cs`
- `Assets/Scripts/Runtime/PlayerFirePattern.cs`
- `Assets/Scripts/Runtime/PowerupCycle.cs`
- `Assets/Scripts/Runtime/AmmoPackController.cs`
- `Assets/Scripts/Runtime/WeaponShotPresentation.cs`

## 敌机、关卡与 Boss

关卡由 `BlockSpawner` 运行时生成，主要阶段包括：

- Preparation：准备/开场。
- Assault：基础编队进攻。
- Pressure：密集压制。
- Elite：精英敌机。
- Boss：Boss 战。
- Clear：通关。

为了支撑金币和炸弹循环，`EnemySpawnBudget` 会按奖励经济提高编队刷怪数量，确保一关有足够敌机让玩家看到金币吸附、火力升级和炸弹道具循环。

Boss 使用多阶段血量和弹幕配置，低/中/高难度会影响 Boss 血量、敌弹速度、敌机生命和射击频率。

敌机美术分层：

- 普通敌机：沿用基础敌机资源，作为低威胁杂兵轮廓。
- 强化敌机：使用 AI 生成的重装拦截机资源 `tough_enemy_ai`。
- 精英敌机：使用 AI 生成的高速精英机资源 `elite_enemy_ai`。
- Boss：使用 AI 生成的宽体旗舰资源 `boss_flagship_ai`。

相关代码：

- `Assets/Scripts/Runtime/BlockSpawner.cs`
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

主要反馈包括：

- 金币吸附动画。
- AI 生成的金色信用币图标，替换旧的纯黄色圆形金币。
- 火力升级 banner。
- 敌机爆炸和碎片。
- 分数弹出文字。
- 炸弹全屏闪光和冲击波。
- Boss 警报与 Boss 血条。

相关代码：

- `Assets/Scripts/UI/UIController.cs`
- `Assets/Scripts/Runtime/EffectsController.cs`
- `Assets/Scripts/Runtime/RuntimeSpriteFactory.cs`

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
- `EnemySpawnBudgetTests`：敌机数量满足奖励经济。
- `StageClearTargetTests`：关卡击落目标。
- `PowerupCycleTests`：火力包循环池。
- `BossPhaseConfigTests`：Boss 多阶段配置。

PlayMode 手测建议：

1. 进入游戏后确认航母起飞、自由移动和自动射击正常。
2. 击毁敌机，确认每架敌机掉落 4 个金币。
3. 飞近金币，确认金币吸附并自动收集。
4. 累计 100 金币，确认地图出现炸弹图标。
5. 飞向炸弹，确认触碰后立即清屏并播放全屏特效。
6. 吃火力包，确认火力等级提升，子弹形态变化。
7. 漏掉敌机，确认只扣金币且不会出现基地失守。
8. 被敌弹或敌机碰撞，确认 HP 扣减，HP 归零才失败。
9. 推进到 Boss，确认 Boss 血条、警报、弹幕和通关结算正常。
