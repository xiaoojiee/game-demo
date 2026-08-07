# demo2 — Unity 2D 动作游戏

## 项目概述

这是 demo1 的延续项目。demo1 已建立了一套完整的 Unity 2D 动作游戏框架，包括：
- 玩家状态机系统
- 怪物 AI 状态机系统（史莱姆）
- 颜料/染色系统（核心特色玩法）
- Boss 战斗系统
- 弹幕/子弹系统
- 伤害与击退系统
- 摄像机系统

demo2 在此基础上继续开发。详见下方各系统说明。

<!-- UNITY CODE ASSIST INSTRUCTIONS START -->
- Project name: demo2
- Unity version: Unity 2022.3.62f3c1
<!-- UNITY CODE ASSIST INSTRUCTIONS END -->

## 核心架构

### 1. 玩家状态机系统

**文件结构**：`Assets/JiaoBeng/Player/`

```
PlayerStateMachine.cs   — 主状态机（MonoBehaviour），挂载在玩家 GameObject 上
PlayerState.cs          — 抽象基类，所有状态继承此类
StateType.cs            — 状态类型枚举 + ActionPriority 优先级枚举
```

**状态列表及优先级**（数字越大越高）：
| 状态 | 优先级 | 说明 |
|------|--------|------|
| Idle | 10 | 待机 |
| Run | 20 | 地面移动 |
| Fall | 30 | 下落（!isGrounded && vel.y < 0） |
| Jump | 40 | 地面起跳 |
| DoubleJump | 42 | 二段跳 |
| Sprint | 45 | 闪避/冲刺（左Shift），有冷却 |
| Attack | 50 | 普通攻击（鼠标左键），有冷却 |
| SprintAttack | 55 | 突刺攻击（E键），3s冷却，消耗颜料 |

**关键设计模式**：
- `InputBuffer` 结构体：输入缓冲系统，允许玩家提前按键 `bufferDuration` 秒仍可触发
- **优先级驱动**：每帧调用 `DetermineBestState()`，遍历所有可能状态，选中优先级最高的切换
- **JumpCut**：松键截断跳跃上升力（`jumpCutMultiplier = 0.5`）
- **冷却计时**：`_attackCooldownTimer`, `_sprintCooldownTimer`, `_sprintAttackCooldownTimer`
- **地面检测**：`BoxCollider2D.IsTouchingLayers(groundLayer)`
- **空气冲次数**：`airSprintCount` 限制空中只能冲刺1次
- **二段跳次数**：`jumpCount` + `JumpTimes = 2`

**输入按键**：
- 水平移动：`Horizontal` 轴（A/D 或左右箭头）
- 跳跃：`Jump`（空格）
- 攻击：鼠标左键（`Mouse0`）
- 冲刺：左Shift
- 突刺攻击：E键

**美术资源引用**：
- `SprintTrail` — 冲刺拖尾特效
- `doubleJumpPaintObject` — 二段跳颜料爆发
- `attackHitBoxes[]` — 普攻碰撞体数组
- `sprintAttackHitBox` — 突刺碰撞体

### 2. 颜料/染色系统（Paint System）

**文件结构**：`Assets/JiaoBeng/Paint/`

这是项目的核心特色系统。玩家攻击和技能会在 Tilemap 地面上留下颜料。

**核心类**：
| 文件 | 说明 |
|------|------|
| `PaintManager.cs` | 单例管理器，控制所有颜料生成和 Tilemap 染色 |
| `PaintDrop.cs` | 颜料滴实体，物理飞行+碰撞后溅射 |
| `PaintMeter.cs` | 玩家颜料计量条 |
| `PlayerColor.cs` | 玩家当前颜色 |
| `PlayerSlashColorBinder.cs` | 绑定玩家颜色到攻击颜料 |
| `PaintSpawner.cs` | 颜料生成器组件，挂载在攻击碰撞体上 |
| `AttackPaintSpawner.cs` | 攻击颜料生成器 |
| `SlashPaintEffect.cs` | 斩击颜料特效 |
| `PaintLauncherColorBinder.cs` | 发射器颜色绑定 |

**形状系统**（`Shape/`）：
| 类 | 说明 |
|------|------|
| `PaintShapeBase` | 颜料形状抽象基类 |
| `Shape_Circle` | 圆形 |
| `Shape_Cone` | 锥形 |
| `Shape_Line` | 线形 |
| `Shape_QuarterCircle` | 四分之一圆 |

**染色算法**（`PaintManager`）：
- **椭圆扩散**：以落点为中心，`aspectRatio` 控制椭圆宽高比
- **指数衰减**：`intensity = baseIntensity * decayRatio^distance`
- **强度下限**：`intensityFloor` 卡住最小值，避免边缘太淡
- **颜色叠加**：`Color.Lerp(currentColor, newColor, intensity)` 逐格叠加
- **Tilemap 染色**：`tilemap.SetColor(cell, finalColor)`

**扩散参数**（`PaintSpreadSettings`）：
```csharp
baseIntensity  — 中心强度（建议保持1）
wallIntensity  — 墙面轨迹强度
diffusionRadius — 最大扩散半径（格子数）
decayRatio     — 每格衰减比例（默认0.7）
aspectRatio    — 椭圆宽高比（1=正圆）
```

**颜料消耗**：
- `PaintMeter.HasEnough(dropCount, chance)` — 检查是否有足够颜料
- `limitByPaintMeter` — 怪物设为 false（无限制），玩家设为 true
- 攻击前会校验 `CanAfford(hitBox)`，不够则不触发

### 3. 怪物 AI 系统

**文件结构**：`Assets/JiaoBeng/Monster/Slime/1/`

**史莱姆状态列表**：
| 状态 | 说明 |
|------|------|
| SlimeIdle | 待机 |
| SlimePatrol | 巡逻 |
| SlimeChase | 追击玩家 |
| SlimeAttack | 攻击（跳跃踩踏） |
| SlimeHurt | 受伤 |
| SlimeFall | 下落 |
| SlimeDie | 死亡 |

**状态机基类**：`SlimeState.cs`（抽象类，类似 `PlayerState`）
**状态机主体**：`SlimeStateMachine.cs`（MonoBehaviour）

**通用怪物组件**：
| 文件 | 说明 |
|------|------|
| `PaintClear.cs` | 怪物经过时清除颜料/使颜料褪色 |
| `DeathPaint.cs` | 死亡时爆发颜料 |
| `DeathShake.cs` | 死亡时震屏 |
| `Respawner.cs` | 怪物重生 |
| `ScaleByHealth.cs` | 体型随血量变化 |
| `RandomizeHealth.cs` | 随机血量 |

**动画要点**：使用 `Animator.Play("状态名")` 切换动画。曾遇到 `Animator.GotoState: State could not be found` 错误是因为动画状态名不匹配。

### 4. 伤害/战斗系统

**文件结构**：`Assets/JiaoBeng/hit/`

**核心文件**：
| 文件 | 说明 |
|------|------|
| `IDamage.cs` | 伤害接口 `void TakeDamage(Damage damage)` |
| `Healh.cs` | 血量组件（实现 IDamage） |
| `DamageSender.cs` | 伤害发送器，挂载在攻击碰撞体上 |
| `Knockback.cs` | 击退效果 |
| `HitFlash.cs` | 受击闪烁 |
| `HitStop.cs` | 命中顿帧/卡肉 |
| `DamagePaint.cs` | 受击时溅出颜料 |
| `Monster.cs` | 怪物基类（实现 IDamage） |

**Damage 结构体**：
```csharp
struct Damage {
    int damageAmount;        // 伤害值
    Vector2 knockbackForce;  // 击退力
    Transform damageSource;  // 伤害来源
    Color PaintColor;        // 携带的颜料颜色
}
```

### 5. Boss 系统

**文件结构**：`Assets/JiaoBeng/boss/`

| 文件 | 说明 |
|------|------|
| `BossCore.cs` | Boss 核心逻辑 |
| `Bullet.cs` | 子弹实体 |
| `BossBulletManager.cs` | 子弹管理器 |
| `BlockBullLauncher.cs` | 方块子弹发射器 |
| `Block.cs` | 方块障碍物 |
| `悬浮.cs` | 悬浮行为 |
| `检测.cs` | 检测逻辑 |

### 6. 弹幕/投射物系统

**文件结构**：`Assets/JiaoBeng/Proj/`

| 文件 | 说明 |
|------|------|
| `Projectile.cs` | 投射物实体 |
| `ProjectileLauncher.cs` | 投射物发射器 |
| `ExplosionDamage.cs` | 爆炸伤害 |
| `ExplosionHandle.cs` | 爆炸处理器 |
| `IExplosionShape.cs` | 爆炸形状接口 |
| `CircleExplosionShape.cs` | 圆形爆炸 |
| `RectangleExplosionShape.cs` | 矩形爆炸 |

### 7. 摄像机系统

| 文件 | 说明 |
|------|------|
| `CameraFollow.cs` | 平滑跟随玩家 |
| `CameraBack.cs` | 背景视差滚动（加速→减速3秒） |

### 8. UI 系统

| 文件 | 说明 |
|------|------|
| `HealthBarUI.cs` | 血条显示 |
| `ColorIndicator.cs` | 当前颜色指示器 |

### 9. 游戏管理

| 文件 | 说明 |
|------|------|
| `GamePause.cs` | 暂停（时间缩放为0但游戏状态不停） |
| `MenuManager.cs` | 菜单管理 |
| `Map.cs` | 地图管理 |

## 技术约定

### 代码风格
- **命名空间**：不使用 namespace，全局命名
- **命名规范**：PascalCase 类名/方法名，camelCase 参数
- **中文注释**：允许中文注释和中文变量名（如 `悬浮.cs`, `检测.cs`）
- **序列化**：字段使用 `[SerializeField]` 或 `public` 暴露给 Inspector

### 工具类
- **Singleton\<T\>**：泛型单例基类（`Assets/JiaoBeng/Paint/Singleton.cs`），DontDestroyOnLoad + 防重复
- **InputBuffer**：输入缓冲结构体，在 `PlayerStateMachine.cs` 中定义

### 关键场景配置
- **场景名**：Game
- **关键 Tag**：Player, Monster, GameController, Boss, Block, 柱子, WuKuai
- **关键 Layer**：Player, Monstar(Monster), DiMian(地面), Hit, Boss, MonsterHit, 柱子, Drop

### 常见 Bug 记录
1. **场景重载后引用丢失**：`PaintManager` 中使用 `FindObjectOfType<Tilemap>()` 重新查找，避免旧引用被销毁
2. **Animator 找不到状态**：检查 `Animator.Play()` 中的字符串是否与 Animator 控制器中的状态名完全一致
3. **Tilemap 染色不生效**：需要先调用 `tilemap.SetTileFlags(cell, TileFlags.None)` 清除锁定的颜色
4. **游戏退出后重启不能染色**：场景中 groundTilemap 引用丢失，需要在 `SpawnSplat` 方法开头重新 `FindObjectOfType`

### Git 仓库
- 远程地址：`https://github.com/xiaoojiee/game-demo.git`
- 分支：`main`
