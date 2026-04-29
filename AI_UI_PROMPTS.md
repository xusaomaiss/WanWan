# WANWAN 空投爆破 — AI 图片素材生成提示词

使用 AI 图片工具（Midjourney / DALL-E / Stable Diffusion）生成的 UI 素材统一导入 `Assets/Resources/UI/` 目录。

---

## 一、标题画面素材

### 1.1 主标题背景
- **用途**：替换当前风暴背景，作为标题画面全屏背景
- **分辨率**：1080×1920（竖屏）
- **提示词**：
  > Sci-fi game menu background, dark cyberpunk atmosphere, storm clouds with lightning, fighter jets silhouettes in distance, deep blue and purple color scheme, 9:16 portrait ratio, game UI background, dramatic lighting, detailed clouds, neon accents, no text
- **输出文件**：`UI/title_bg.png`
- **Unity 设置**：Sprite(2D and UI), 全屏拉伸

### 1.2 发光标题文字特效层
- **用途**：覆盖在文字"WANWAN"后面的发光光晕，创建文字发光效果
- **分辨率**：512×128
- **提示词**：
  > Horizontal glowing light effect for game title text, cyberpunk neon glow in electric blue and yellow, bright center fading to edges, transparent PNG, dark background, suitable for text backlight
- **输出文件**：`UI/title_glow.png`
- **Unity 设置**：Sprite, 放在文字下方作为发光层

### 1.3 科技网格覆盖层
- **用途**：替换当前的扫描线效果
- **分辨率**：1080×1920
- **提示词**：
  > Sci-fi HUD grid overlay, transparent tech grid pattern with subtle neon blue dots at intersections, dark background with very low opacity grid lines, 9:16 portrait, cyberpunk UI overlay, subtle hex pattern
- **输出文件**：`UI/grid_overlay.png`
- **Unity 设置**：Sprite, 透明度 0.1-0.15

### 1.4 战机选中/预览发光框
- **用途**：标题画面战机预览的外发光框
- **分辨率**：256×256
- **提示词**：
  > Sci-fi fighter selection frame, glowing border in electric blue, angular tech corners, transparent center, game UI element, 1:1 square, neon edge glow effect, dark background
- **输出文件**：`UI/fighter_frame.png`
- **Unity 设置**：Sprite, 放在战机图片下层

### 1.5 圆形按钮外框
- **用途**：替换当前程序化生成的圆形按钮外观（开始/保存/退出）
- **分辨率**：320×320（大按钮）/ 180×180（小按钮）
- **提示词**：
  > Sci-fi circular game UI button, outer glowing ring in cyan blue, inner ring subtle gradient, dark center, 1:1 square, transparent PNG, neon edge glow, cyberpunk style game button, suitable for text overlay
- **输出文件**：`UI/btn_circle_large.png`（大）, `UI/btn_circle_small.png`（小）
- **Unity 设置**：Sprite, 按钮的 Image.sprite，文字放在按钮上方

### 1.6 图标按钮底框
- **用途**：排行榜和设置按钮的底框
- **分辨率**：128×128
- **提示词**：
  > Small sci-fi icon button frame, rounded square with neon glow border, dark fill, 1:1 square, transparent PNG, cyberpunk UI element, technology icon border
- **输出文件**：`UI/icon_button_bg.png`
- **Unity 设置**：Sprite

---

## 二、面板与边框素材

### 2.1 霓虹面板（横）
- **用途**：通用横向面板，用于暂停卡片、选机卡片、设置面板
- **分辨率**：540×320（基础切片尺寸）
- **提示词**：
  > Sci-fi UI panel frame, dark blue gradient fill, glowing cyan blue border (3px), subtle inner shadow, transparent center, horizontal rectangle, cyberpunk game UI, neon border, corner tech accents, suitable for 9-slicing
- **输出文件**：`UI/panel_horizontal.png`
- **Unity 设置**：Sprite Mode = Multiple, 9-slicing (border 20px each side)

### 2.2 霓虹面板（竖）
- **用途**：右侧关卡进度条底座
- **分辨率**：120×540
- **提示词**：
  > Vertical sci-fi UI panel, tall narrow rectangle, dark navy gradient fill, glowing yellow border, futuristic game UI element, transparent center for content, vertical neon frame
- **输出文件**：`UI/panel_vertical.png`
- **Unity 设置**：Sprite Mode = Multiple, 9-slicing (border 15px each side)

### 2.3 科技角标
- **用途**：面板四角装饰用
- **分辨率**：64×64
- **提示词**：
  > Sci-fi corner bracket decoration, L-shaped glowing line in cyan blue, tech angle bracket, game UI corner accent, 64x64 transparent PNG, cyberpunk style, sharp angled lines
- **输出文件**：`UI/corner_bracket.png`

### 2.4 小角标装饰
- **用途**：HUD 顶栏角标
- **分辨率**：48×48
- **提示词**：
  > Small L-shaped corner bracket, thinner glowing line in electric blue, minimal tech corner decoration, game HUD element, transparent PNG, 48x48
- **输出文件**：`UI/corner_small.png`

---

## 三、HUD 素材

### 3.1 顶部 HUD 栏
- **用途**：游戏画面上方的信息栏背景
- **分辨率**：1080×192（实际显示 1080×192）
- **提示词**：
  > Sci-fi game HUD top bar, horizontal strip with tech decoration, dark navy to dark blue gradient background, glowing cyan bottom edge line, subtle corner accents at top-left and top-right, futuristic heads-up display bar, 9:1.6 ratio, cyberpunk style
- **输出文件**：`UI/hud_top_bar.png`
- **Unity 设置**：Sprite, 拉伸模式 Tiled 或 Stretched

### 3.2 底部 HUD 栏
- **用途**：游戏画面下方的信息栏背景
- **分辨率**：1080×200
- **提示词**：
  > Sci-fi game HUD bottom bar, horizontal strip, dark gradient background, glowing green top edge line (matching military green theme), tech corner accents at bottom-left and bottom-right, weapon system panel style, futuristic game UI, 9:1.6 ratio
- **输出文件**：`UI/hud_bottom_bar.png`
- **Unity 设置**：Sprite, 拉伸

### 3.3 Boss 血条面板
- **用途**：Boss 战时的血量条背景
- **分辨率**：900×80
- **提示词**：
  > Sci-fi boss health bar frame, wide horizontal bar, dark red-black gradient background, glowing red border, tech decoration on both ends, danger warning style, game HUD element, transparent center for health fill
- **输出文件**：`UI/boss_bar_frame.png`
- **Unity 设置**：Sprite, 9-slicing

### 3.4 玩家血条框架
- **用途**：玩家血量条背景
- **分辨率**：400×60
- **提示词**：
  > Sci-fi player health bar frame, horizontal bar, dark background, thin glowing border in cyan, subtle tech decoration, game HUD element, transparent center for fill
- **输出文件**：`UI/health_bar_frame.png`
- **Unity 设置**：Sprite, 9-slicing

### 3.5 能量槽分段
- **用途**：底部能量槽 6 个分段
- **分辨率**：80×120
- **提示词**：
  > Sci-fi energy meter segment, vertical rectangular slot, dark background with glowing yellow border when active, dim border when inactive, game UI power meter element, futuristic style, single slot
- **输出文件**：`UI/power_slot_active.png`, `UI/power_slot_inactive.png`
- **Unity 设置**：Sprite

### 3.6 关卡进度条外壳
- **用途**：右侧关卡进度条的底座
- **分辨率**：60×480
- **提示词**：
  > Vertical sci-fi progress bar track, narrow tall rectangle, dark navy background with subtle blue border, tick marks at 25% 50% 75% positions, futuristic thermometer style, game stage progress indicator
- **输出文件**：`UI/progress_track.png`
- **Unity 设置**：Sprite, 9-slicing

### 3.7 连击弹出框
- **用途**：连击倍率显示背景
- **分辨率**：200×120
- **提示词**：
  > Sci-fi combo multiplier badge, angular hexagonal shape, glowing red border for high combo, dark background, game HUD element, attention-grabbing style, transparent center
- **输出文件**：`UI/combo_badge.png`

### 3.8 暂停面板
- **用途**：暂停菜单的卡片背景
- **分辨率**：600×600
- **提示词**：
  > Sci-fi pause menu card panel, dark gradient fill with electric blue neon border, tech corner decorations, cyberpunk game UI, centered square with transparent content area
- **输出文件**：`UI/pause_panel.png`
- **Unity 设置**：Sprite, 9-slicing

---

## 四、选机/难度界面素材

### 4.1 选机卡片
- **用途**：选机界面中的战机信息卡片
- **分辨率**：900×280
- **提示词**：
  > Sci-fi ship selection card, horizontal panel, dark blue gradient fill, glowing border in ship accent color (cyan), tech corner accents, game UI element for fighter selection, spacious layout for stats text
- **输出文件**：`UI/ship_card.png`
- **Unity 设置**：Sprite, 9-slicing

### 4.2 难度卡片
- **用途**：难度选择界面卡片
- **分辨率**：900×160
- **提示词**：
  > Sci-fi difficulty selection card, horizontal panel, dark background, colored glow border (green for easy, blue for normal, red for hard), difficulty-themed tech accents, game menu element
- **输出文件**：`UI/difficulty_card.png`
- **Unity 设置**：Sprite, 9-slicing

### 4.3 武器图标（6种）
- **用途**：替换当前程序化生成的武器图标
- **分辨率**：128×128 each
- **提示词（通用模板）**：
  > Isometric sci-fi weapon icon on dark background, [weapon specific], glowing in [color], transparent PNG, game UI icon, 128x128, futuristic weapon design
- **6 种武器**：
  - `icon_scatter.png` — spread shotgun style, orange glow
  - `icon_rapid.png` — fast fire bullets, yellow glow
  - `icon_laser.png` — laser beam, cyan glow
  - `icon_homing.png` — guided missile, blue glow
  - `icon_burst.png` — explosive burst, red glow
  - `icon_plasma.png` — plasma orb, purple glow

---

## 五、设置/结算界面素材

### 5.1 设置面板
- **用途**：设置界面主体面板
- **分辨率**：900×1200
- **提示词**：
  > Large sci-fi settings menu panel, tall vertical panel, dark blue gradient fill, electric blue neon border, organized sections with subtle dividers, cyberpunk game UI, multiple rows for game settings
- **输出文件**：`UI/settings_panel.png`
- **Unity 设置**：Sprite, 9-slicing

### 5.2 滑块轨道
- **用途**：替换当前滑块外观
- **分辨率**：400×20
- **提示词**：
  > Sci-fi slider track, horizontal bar, dark background with cyan glow fill, futuristic game UI element, transparent PNG
- **输出文件**：`UI/slider_track.png`

### 5.3 滑块手柄
- **用途**：滑块的可拖动部分
- **分辨率**：40×60
- **提示词**：
  > Sci-fi slider handle, vertical diamond or hexagonal shape, glowing cyan center, game UI element, futuristic style, 40x60 transparent PNG
- **输出文件**：`UI/slider_handle.png`

### 5.4 结算面板
- **用途**：GameOver 界面的结果展示面板
- **分辨率**：900×500
- **提示词**：
  > Sci-fi result screen panel, wide horizontal panel, dark gradient fill, victory themed (cyan glow) or defeat themed (red glow), cyberpunk game UI, centered layout for score display, neon border
- **输出文件**：`UI/result_panel.png`
- **Unity 设置**：Sprite, 9-slicing

### 5.5 挂载商店卡片
- **用途**：结算界面中的挂载购买卡片
- **分辨率**：280×200
- **提示词**：
  > Sci-fi mount shop item card, small vertical card, dark background with colored accent border, game UI shop element, cyberpunk style, shows item name and price area
- **输出文件**：`UI/mount_card.png`
- **Unity 设置**：Sprite, 9-slicing

### 5.6 排行榜面板
- **用途**：本地排行榜的背景面板
- **分辨率**：900×700
- **提示词**：
  > Sci-fi leaderboard panel, tall panel, dark gradient fill, golden yellow glow border, tech decorations, trophy/leaderboard style, ranked list layout, neon glow
- **输出文件**：`UI/leaderboard_panel.png`
- **Unity 设置**：Sprite, 9-slicing

---

## 六、按钮素材

### 6.1 像素风格按钮（通用）
- **用途**：`CreatePixelButton` 使用的通用按钮
- **分辨率**：200×76（base）
- **提示词**：
  > Sci-fi game UI button, rectangular with tech border, dark fill with glowing outline, cyberpunk style, game menu button, suitable for text label inside, 200x76 base for 9-slicing
- **输出文件**：`UI/btn_pixel.png`
- **Unity 设置**：Sprite, 9-slicing (border 8px)

### 6.2 开关按钮
- **用途**：设置界面的开关（ON/OFF）
- **分辨率**：120×60
- **提示词**：
  > Sci-fi toggle switch, horizontal pill shape, left half ON (glowing green), right half OFF (dim), game UI toggle element, cyberpunk style, neon accent
- **输出文件**：`UI/toggle_on.png`, `UI/toggle_off.png`

### 6.3 暂停/继续按钮
- **用途**：暂停菜单中的操作按钮
- **分辨率**：400×70
- **提示词**：
  > Sci-fi menu action button, wide rectangle, dark fill with glowing border, cyberpunk game UI, text label button, 400x70
- **输出文件**：`UI/btn_action.png`
- **Unity 设置**：Sprite, 9-slicing

---

## 七、导入后的 Unity 配置说明

所有素材导入 Unity 后需设置：

1. **Texture Type** → Sprite (2D and UI)
2. **Sprite Mode** → Single（图标/独立元素）或 Multiple（需 9-slicing 的面板）
3. **Pixels Per Unit** → 128（保持与现有项目一致）
4. **Filter Mode** → Bilinear（发光模糊效果需要）
5. **Compression** → Normal（品质优先，UI 不适合压缩）
6. **Alpha Source** → Input Texture Alpha（保留透明通道）

对于需 9-slicing 的面板，在 Sprite Editor 中设置 Border：
- 面板类：上下左右各 20-30px
- 按钮类：上下左右各 8-15px
- 细边框类：上下左右各 8px
