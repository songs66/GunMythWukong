# GunMythWukong｜枪神话·悟空


## 📌 项目简介

![Unity](https://img.shields.io/badge/Unity%2FTuanjie-2022.3.62t9-black?logo=unity)
![C#](https://img.shields.io/badge/Language-C%23-239120?logo=csharp&logoColor=white)
![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Status](https://img.shields.io/badge/Status-Playable%20Prototype-orange)
![License](https://img.shields.io/badge/License-MIT%20%28Code%20Only%29-green)

**GunMythWukong｜枪神话·悟空** 是一个使用 **Unity 3D + C#** 开发的小型 FPS 枪战游戏项目。

游戏以“悟空角色 + 现代枪械 + 废弃工业场景 + 敌人刷新战斗”为核心玩法，玩家需要在限定战斗场景中操控角色移动、瞄准、射击、击败不断刷新的敌人，并在生命值耗尽前完成击杀目标。

这个项目最初用于虚拟现实 / 三维交互场景开发课程实践，重点不只是“做出一个能玩的小游戏”，而是完整串联 Unity 3D 游戏开发中常见的核心模块：

* 玩家第一 / 第三人称控制
* 摄像机视角切换
* 枪械射击与命中检测
* 敌人 AI 追踪与攻击
* 敌人对象池刷新
* 血量、伤害、死亡与胜负判定
* 十字准星与命中反馈
* 游戏暂停、重开、退出与鼠标灵敏度调节
* Windows 可执行版本导出

---

## 🎮 游戏玩法

玩家进入废弃工业战场后，会经历短暂倒计时。倒计时结束后，敌人会从场景中的多个刷新点不断出现。

你的任务是：

> 在生命值归零之前，击败指定数量的敌人。

当前默认任务目标：

```text
击败 50 个敌人
```

游戏过程中你需要合理利用移动、奔跑、跳跃、视角切换和射击操作，持续躲避敌人近战攻击，并尽可能高效地完成击杀目标。

---

## ✨ 核心功能

### 1. 第一 / 第三人称双视角控制

项目实现了第一人称与第三人称摄像机切换，玩家可以根据战斗情况自由选择更适合的视角。

* 第一人称：更适合沉浸式瞄准射击
* 第三人称：更适合观察角色与周围敌人位置
* 支持运行时按键切换视角
* 切换视角时自动处理角色模型显示 / 隐藏

---

### 2. 玩家移动系统

玩家控制器支持基础 FPS / TPS 游戏中常见的移动逻辑：

* `W / A / S / D` 移动
* 鼠标控制视角
* `Shift` 奔跑
* `Space` 跳跃
* 重力下落
* 鼠标灵敏度可调节

---

### 3. 枪械射击系统

项目实现了一套基础但完整的枪械射击逻辑：

* 鼠标左键射击
* 射击间隔控制
* 从屏幕中心发射瞄准射线
* 从枪口位置生成真实射击方向
* 敌人命中检测
* 伤害结算
* 枪口火光
* 子弹轨迹
* 射击音效

该设计解决了 FPS / TPS 游戏中常见的一个问题：

> 玩家瞄准点来自摄像机中心，但子弹视觉效果应该从枪口发出。

因此项目中采用了“摄像机中心确定目标点，枪口朝目标点发射”的方式，让射击手感和视觉表现更加自然。

---

### 4. 敌人 AI 系统

敌人具备基础战斗行为：

* 自动寻找玩家
* 在侦测范围内追踪玩家
* 接近玩家后进行攻击
* 攻击间隔控制
* 对玩家造成伤害
* 死亡后触发击杀统计

虽然当前 AI 仍属于轻量级实现，但已经具备一个小型 FPS 游戏所需的核心敌人行为闭环。

---

### 5. 敌人刷新与对象池

项目使用敌人刷新管理器控制敌人生成逻辑：

* 多刷新点生成
* 限制场上最大存活敌人数量
* 定时刷新敌人
* 使用对象池复用敌人对象
* 避免频繁 Instantiate / Destroy 带来的性能浪费

这让游戏在持续战斗过程中更加稳定，也更符合真实游戏项目中的基础优化思路。

---

### 6. 游戏流程管理

项目实现了完整的游戏状态流转：

```text
Countdown → Playing → Paused → Success / Failed
```

对应功能包括：

* 开局倒计时
* 游戏中 HUD 显示
* ESC 暂停菜单
* 继续游戏
* 重新开始
* 退出游戏
* 鼠标灵敏度调节
* 击杀目标达成后胜利
* 玩家生命值归零后失败

---

### 7. UI 与战斗反馈

项目包含基础但实用的 UI 反馈系统：

* 屏幕中心十字准星
* 命中敌人时显示命中反馈
* 玩家受伤时屏幕伤害提示
* 血量显示
* 击杀进度显示
* 当前存活敌人数量显示
* 暂停菜单
* 胜利 / 失败结算界面

这些反馈让玩家能够明确知道当前游戏状态，而不是只看到“能移动、能开枪”的空壳场景。

---

## 🕹️ 操作说明

| 操作              | 功能                 |
| --------------- | ------------------ |
| `W / A / S / D` | 玩家移动               |
| 鼠标移动            | 控制视角               |
| 鼠标左键            | 射击                 |
| `Shift`         | 奔跑                 |
| `Space`         | 跳跃                 |
| `V`             | 第一 / 第三人称视角切换      |
| `ESC`           | 暂停 / 打开菜单          |
| 暂停菜单滑条          | 调整鼠标灵敏度            |
| 暂停菜单按钮          | 继续游戏 / 重新开始 / 退出游戏 |

---

## 🧱 技术栈

| 类型    | 技术                          |
| ----- | --------------------------- |
| 游戏引擎  | Unity 2022.3 LTS / 团结引擎兼容版本 |
| 编程语言  | C#                          |
| 渲染与场景 | Unity 3D                    |
| UI    | Unity UI / IMGUI            |
| 角色控制  | CharacterController         |
| 碰撞检测  | Physics Raycast             |
| 敌人行为  | 自定义轻量 AI                    |
| 资源管理  | Prefab + Object Pool        |
| 目标平台  | Windows PC                  |

---

## 🏗️ 项目结构

```text
GunMythWukong/
├── Assets/
│   ├── _Game/
│   │   ├── Animations/              # 游戏动画资源
│   │   ├── Enemies/                 # 敌人相关资源
│   │   ├── Scenes/                  # 游戏主场景
│   │   ├── Scripts/                 # 核心 C# 脚本
│   │   │   ├── Combat/              # 战斗、血量、武器、伤害接口
│   │   │   ├── Enemies/             # 敌人 AI、死亡效果、刷新管理
│   │   │   ├── Game/                # 游戏流程管理
│   │   │   ├── Player/              # 玩家控制器
│   │   │   └── UI/                  # 十字准星、受伤反馈等 UI
│   │   └── UI/                      # 游戏界面资源
│   │
│   ├── Char_Wukong/                 # 第三方悟空角色资源
│   ├── Low Poly Weapons VOL.1/      # 第三方低多边形武器资源
│   ├── RPG_FPS_game_assets_industrial/
│   │                                  # 第三方工业场景资源
│   └── Sci-FI_Trooper_Man_v.3/      # 第三方科幻敌人角色资源
│
├── Packages/                        # Unity 包依赖
├── ProjectSettings/                 # Unity 项目设置
├── .gitignore
├── .vsconfig
├── LICENSE
└── README.md
```

---

## 🧠 核心脚本说明

| 模块    | 脚本                            | 说明                          |
| ----- | ----------------------------- | --------------------------- |
| 玩家控制  | `DualViewPlayerController.cs` | 玩家移动、鼠标视角、跳跃、奔跑、第一 / 第三人称切换 |
| 武器系统  | `PlayerWeaponController.cs`   | 开火、射线检测、伤害结算、枪口特效、弹道效果      |
| 血量系统  | `PlayerHealth.cs`             | 玩家生命值、受伤、死亡                 |
| 敌人血量  | `EnemyHealth.cs`              | 敌人受伤、死亡、击杀通知                |
| 伤害接口  | `IDamageable.cs`              | 统一受伤接口，降低模块耦合               |
| 可射击目标 | `ShootableTarget.cs`          | 用于测试或扩展可被射击的对象              |
| 敌人 AI | `EnemyAI.cs`                  | 敌人侦测、追踪、攻击玩家                |
| 敌人刷新  | `EnemySpawnManager.cs`        | 敌人对象池、刷新点、最大存活数量控制          |
| 游戏流程  | `GameFlowManager.cs`          | 倒计时、暂停、胜负判定、HUD、重开与退出       |
| 准星反馈  | `CrosshairUI.cs`              | 屏幕准星与命中反馈                   |
| 受伤反馈  | `DamageFlashUI.cs`            | 玩家受伤时的屏幕反馈                  |

---

## 🚀 快速开始

### 1. 克隆仓库

```bash
git clone https://github.com/songs66/GunMythWukong.git
```

### 2. 使用 Unity 打开项目

推荐使用以下版本或相近版本打开：

```text
Unity 2022.3 LTS
```

步骤：

1. 打开 Unity Hub
2. 点击 `Add / 添加`
3. 选择本仓库根目录 `GunMythWukong`
4. 使用 Unity 2022.3 LTS 或兼容版本打开
5. 打开 `Assets/_Game/Scenes/` 下的主场景
6. 点击 Play 运行游戏

---

## 📦 构建 Windows 可执行版本

在 Unity 中执行：

```text
File → Build Settings → PC, Mac & Linux Standalone → Windows → Build
```

推荐设置：

| 配置项          | 推荐值                     |
| ------------ | ----------------------- |
| Platform     | Windows                 |
| Architecture | x86_64                  |
| Build Type   | Release                 |
| Compression  | Default                 |
| Scene        | 添加 `_Game/Scenes` 下的主场景 |

构建完成后，可以将生成的 `.exe` 文件和同目录下的数据文件夹一起打包为 `.zip`，用于发布给其他玩家体验。

---

## 🖼️ 项目预览

建议在发布 GitHub 项目前补充以下素材，让 README 更像一个完整游戏项目：

```text
docs/images/cover.png          # 项目封面图
docs/images/gameplay.gif       # 10~20 秒游戏演示 GIF
docs/images/combat.png         # 战斗截图
docs/images/pause-menu.png     # 暂停菜单截图
docs/images/result.png         # 胜利 / 失败界面截图
```

添加后可以取消下面这些注释：

---

## 🧩 当前版本完成度

* [x] 玩家移动
* [x] 鼠标视角控制
* [x] 第一 / 第三人称切换
* [x] 枪械射击
* [x] 命中检测
* [x] 敌人受伤与死亡
* [x] 敌人 AI 追踪与攻击
* [x] 敌人对象池刷新
* [x] 玩家血量系统
* [x] 击杀目标判定
* [x] 游戏胜利 / 失败状态
* [x] 暂停菜单
* [x] 重新开始与退出
* [x] 鼠标灵敏度调节
* [x] 十字准星命中反馈
* [ ] 多武器切换
* [ ] 换弹系统
* [ ] 敌人远程攻击
* [ ] 更完整的主菜单
* [ ] 音量设置
* [ ] 小地图
* [ ] 关卡选择
* [ ] WebGL 在线试玩版本

---

## 🛣️ Roadmap

后续可以继续扩展以下方向：

### Gameplay

* 增加多种武器：手枪、步枪、霰弹枪、狙击枪
* 增加弹夹、换弹、后坐力、散射
* 增加拾取物：血包、弹药、临时增益
* 增加不同敌人类型：近战兵、远程兵、精英怪、Boss
* 增加波次系统：每一波敌人数量和强度递增

### UI / UX

* 增加主菜单
* 增加设置界面
* 增加音量调节
* 增加游戏说明页
* 增加结算评分
* 增加更精致的准星动画和受击反馈

### Engineering

* 将 IMGUI 迁移到 Unity UI Toolkit 或 Canvas UI
* 使用更清晰的状态机管理游戏流程
* 使用 ScriptableObject 管理武器和敌人配置
* 拆分玩家输入、移动、射击、血量模块
* 增加对象池泛型封装
* 增加配置文件和关卡数据管理

### Release

* 发布 Windows 可玩版本
* 增加 GitHub Releases
* 添加 gameplay GIF
* 添加 itch.io 页面
* 添加 WebGL 在线试玩版本

---

## 🙏 第三方资源说明

本项目使用了一些免费第三方资源用于课程学习、游戏原型开发和非商业展示。感谢所有资源作者的开放与分享。

> 注意：免费资源不等于没有版权。第三方资源仍然遵循其原始作者或平台的授权协议。
> 本仓库的 MIT License 仅覆盖本人原创代码，不覆盖第三方模型、贴图、动画、音效、字体等资源。

| 资源                                          | 用途        | 来源 / 授权                                             |
| ------------------------------------------- | --------- | --------------------------------------------------- |
| Char_Wukong                                 | 玩家悟空角色模型  | Unity Asset Store / Standard Unity Asset Store EULA |
| Low Poly Weapons VOL.1                      | 枪械 / 武器模型 | Unity Asset Store / Standard Unity Asset Store EULA |
| PBR RPG/FPS Game Assets Industrial Set v1.0 | 废弃工业场景资源  | Unity Asset Store / Standard Unity Asset Store EULA |
| FREE Sci-Fi Trooper Man v3                  | 敌人科幻士兵模型  | Unity Asset Store / Standard Unity Asset Store EULA |

如果你是资源作者，并认为本项目的使用方式存在问题，请通过 Issue 联系我，我会立即修改署名、补充授权信息或移除相关资源。

---

## 📥 Download

Windows 64位可运行版本已发布在 [Releases](https://github.com/songs66/GunMythWukong/releases) 页面.

---

## 📄 License

本项目中由作者编写的原创代码使用 **MIT License** 开源。

需要特别说明：

```text
MIT License 仅适用于本项目中的原创代码。
第三方资源，包括但不限于角色模型、武器模型、场景模型、贴图、动画、音效、字体等，
不属于 MIT License 授权范围，仍然遵循其原始资源页面、作者或平台的授权协议。
```

详情请查看仓库中的 `LICENSE` 文件。

---

## 👨‍💻 Author

**SongZihao / songs66**

* GitHub: [songs66](https://github.com/songs66)
* Project: [GunMythWukong](https://github.com/songs66/GunMythWukong)

---

## ⭐ Star

如果这个项目对你有帮助，欢迎点一个 Star。

这个项目会继续作为 Unity 3D 游戏开发学习、FPS 原型开发、课程设计展示和个人作品集项目持续迭代。

```text
May the myth begin with a gun.
```
