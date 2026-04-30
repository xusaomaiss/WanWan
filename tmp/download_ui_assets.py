#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
WANWAN 空投爆破 — UI素材批量下载脚本
使用方法:
    1. 安装依赖: pip install requests pillow
    2. 运行: python download_ui_assets.py
"""

import os
import time
import requests
from urllib.parse import urlparse
from pathlib import Path

# ========== 配置 ==========
OUTPUT_DIR = "UI_Assets"  # 下载目录，可修改
TIMEOUT = 30             # 请求超时(秒)
DELAY = 1                # 每次下载间隔(秒)，避免请求过快
RETRY = 2                # 失败重试次数

# ========== 图片列表 (URL, 保存文件名) ==========
IMAGES = [
    ("https://images.stockcake.com/public/5/0/e/50e04dc4-f04f-4b49-9604-23b86980091c_large/neon-cyber-face-stockcake.jpg", "title_bg_storm.jpg"),
    ("https://images.stockcake.com/public/7/4/b/74b6009d-e5e6-4716-a85b-af8f5164d4ba/neon-fighter-jet-stockcake.jpg", "title_fighter_jet.jpg"),
    ("https://www.shutterstock.com/image-vector/laser-beam-effects-on-transparent-260nw-2693998265.jpg", "title_glow_effect.jpg"),
    ("https://static.vecteezy.com/system/resources/thumbnails/068/634/859/small_2x/cyber-futuristic-digital-hud-grid-with-shining-lights-and-elements-in-dark-blue-background-template-vector.jpg", "grid_overlay.jpg"),
    ("https://static.vecteezy.com/system/resources/previews/007/162/503/small/abstract-futuristic-background-of-blue-glowing-technology-sci-fi-frame-hud-ui-lower-third-button-bar-vector.jpg", "fighter_frame.png"),
    ("https://static.vecteezy.com/system/resources/previews/060/690/711/non_2x/futuristic-glowing-blue-ring-with-segmented-design-and-neon-accents-evoking-sense-of-advanced-technology-and-innovation-sleek-circular-form-suggests-sci-fi-theme-png.png", "btn_circle_large.png"),
    ("https://static.vecteezy.com/system/resources/thumbnails/043/509/996/small/vr-hud-futuristic-interface-square-grid-line-and-dot-head-up-display-similar-blue-pattern-digital-ui-screen-mesh-gui-digital-hi-tech-visor-backdrop-template-fui-sci-fi-dashboard-display-vector.jpg", "icon_button_bg.jpg"),
    ("https://www.shutterstock.com/image-vector/cyberpunk-hud-frames-panels-futuristic-260nw-2603857031.jpg", "panel_horizontal.jpg"),
    ("https://www.shutterstock.com/image-vector/set-abstract-blue-futuristic-technology-600nw-2702917065.jpg", "panel_vertical.jpg"),
    ("https://www.shutterstock.com/image-vector/digital-screen-technology-box-blue-260nw-2637970019.jpg", "corner_bracket.png"),
    ("https://www.shutterstock.com/image-vector/abstract-futuristic-background-blue-glowing-260nw-1837386709.jpg", "corner_small.png"),
    ("https://static.vecteezy.com/system/resources/previews/007/504/678/non_2x/abstract-futuristic-background-of-blue-glowing-technology-sci-fi-frame-hud-ui-lower-third-button-bar-vector.jpg", "hud_top_bar.jpg"),
    ("https://static.vecteezy.com/system/resources/previews/077/461/243/non_2x/futuristic-hud-banner-frame-with-high-tech-sci-fi-design-features-red-and-blue-geometric-borders-digital-interface-elements-and-a-white-blank-space-for-text-or-gaming-stream-overlays-vector.jpg", "hud_bottom_bar.jpg"),
    ("https://static.vecteezy.com/system/resources/previews/015/391/250/non_2x/abstract-futuristic-background-of-red-glowing-technology-sci-fi-frame-hud-ui-vector.jpg", "boss_bar_frame.jpg"),
    ("https://www.shutterstock.com/image-vector/abstract-yellow-black-futuristic-scifi-260nw-2747025993.jpg", "health_bar_frame.jpg"),
    ("https://static.vecteezy.com/system/resources/previews/068/588/237/non_2x/futuristic-black-and-yellow-ui-panel-with-orange-highlights-minimal-tech-interface-element-in-dark-mode-style-scifi-hud-frame-with-sleek-orange-border-vector.jpg", "power_slot_active.jpg"),
    ("https://c8.alamy.com/comp/2J8KK0F/futuristic-download-progress-bar-hud-blue-digital-elements-2J8KK0F.jpg", "progress_track.jpg"),
    ("https://static.vecteezy.com/system/resources/previews/015/734/629/non_2x/abstract-futuristic-background-of-red-glowing-technology-sci-fi-frame-hud-ui-vector.jpg", "combo_badge.jpg"),
    ("https://png.pngtree.com/thumb_back/fh260/background/20241210/pngtree-futuristic-blue-neon-frame-in-a-dark-setting-perfect-for-tech-image_16678084.jpg", "pause_panel.jpg"),
    ("https://www.shutterstock.com/image-vector/futuristic-tech-game-uiguifui-frames-260nw-2575116909.jpg", "ship_card.jpg"),
    ("https://www.shutterstock.com/image-vector/cyberpunk-futuristic-scifi-neon-hud-260nw-2682979223.jpg", "difficulty_card.jpg"),
    ("https://thumbs.dreamstime.com/b/orange-illuminated-futuristic-gun-dark-backdrop-d-digital-render-orange-illuminated-futuristic-gun-dark-backdrop-d-311257399.jpg", "icon_scatter.jpg"),
    ("https://www.shutterstock.com/image-vector/loading-bar-retro-futurism-scifi-260nw-2695573643.jpg", "icon_rapid.jpg"),
    ("https://www.shutterstock.com/image-vector/laser-beam-effects-on-transparent-260nw-2693998265.jpg", "icon_laser.jpg"),
    ("https://www.shutterstock.com/image-vector/hud-grid-tech-interface-electronic-260nw-2702945829.jpg", "icon_homing.jpg"),
    ("https://www.shutterstock.com/image-vector/transparent-neon-glass-frames-collection-260nw-2731203663.jpg", "icon_burst.jpg"),
    ("https://i.etsystatic.com/44403991/r/il/b84391/5090427362/il_fullxfull.5090427362_6xzf.jpg", "icon_plasma.jpg"),
    ("https://www.shutterstock.com/image-vector/transparent-neon-glass-frames-collection-260nw-2731203663.jpg", "settings_panel.jpg"),
    ("https://www.shutterstock.com/image-vector/loading-bar-retro-futurism-scifi-260nw-2695573643.jpg", "slider_track.jpg"),
    ("https://static.vecteezy.com/system/resources/previews/032/414/740/non_2x/hud-frames-futuristic-frame-gradient-futuristic-text-frame-scifi-frame-hud-gradient-hud-futuristic-frame-scifi-digital-screen-border-free-png.png", "slider_handle.png"),
    ("https://www.shutterstock.com/image-vector/cyberpunk-victory-defeat-screen-futuristic-260nw-2682979261.jpg", "result_panel.jpg"),
    ("https://www.shutterstock.com/image-vector/cyberpunk-futuristic-scifi-neon-hud-260nw-2682979223.jpg", "mount_card.jpg"),
    ("https://www.shutterstock.com/image-vector/futuristic-hud-interface-elements-neon-260nw-2464391947.jpg", "leaderboard_panel.jpg"),
    ("https://www.shutterstock.com/image-vector/futuristic-cyberpunk-game-ui-menu-260nw-2682979257.jpg", "btn_pixel.jpg"),
    ("https://static.vecteezy.com/system/resources/previews/071/230/043/non_2x/cyberpunk-hud-box-elements-with-futuristic-ui-frames-and-digital-interface-overlays-sci-fi-hologram-borders-and-glitch-effects-for-game-screens-or-vr-designs-modern-data-dashboards-vector.jpg", "toggle_on.jpg"),
    ("https://static.vecteezy.com/system/resources/previews/071/230/043/non_2x/cyberpunk-hud-box-elements-with-futuristic-ui-frames-and-digital-interface-overlays-sci-fi-hologram-borders-and-glitch-effects-for-game-screens-or-vr-designs-modern-data-dashboards-vector.jpg", "toggle_off.jpg"),
    ("https://static.vecteezy.com/system/resources/previews/007/504/678/non_2x/abstract-futuristic-background-of-blue-glowing-technology-sci-fi-frame-hud-ui-lower-third-button-bar-vector.jpg", "btn_action.jpg"),
]

# ========== 下载函数 ==========
def download_image(url, save_path, timeout=TIMEOUT, retries=RETRY):
    """下载单张图片，支持重试"""
    headers = {
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36'
    }
    
    for attempt in range(retries + 1):
        try:
            response = requests.get(url, headers=headers, timeout=timeout, stream=True)
            response.raise_for_status()
            
            with open(save_path, 'wb') as f:
                for chunk in response.iter_content(chunk_size=8192):
                    if chunk:
                        f.write(chunk)
            
            size = os.path.getsize(save_path)
            return True, size
            
        except Exception as e:
            if attempt < retries:
                time.sleep(1)
                continue
            return False, str(e)
    
    return False, "Unknown error"

def main():
    """主函数：批量下载所有素材"""
    # 创建输出目录
    output_path = Path(OUTPUT_DIR)
    output_path.mkdir(exist_ok=True)
    
    print(f"{'='*60}")
    print(f"  WANWAN UI素材批量下载工具")
    print(f"  输出目录: {output_path.absolute()}")
    print(f"  共 {len(IMAGES)} 个文件")
    print(f"{'='*60}\n")
    
    success_count = 0
    fail_count = 0
    failed_files = []
    
    for i, (url, filename) in enumerate(IMAGES, 1):
        save_path = output_path / filename
        print(f"[{i:02d}/{len(IMAGES)}] 下载: {filename}")
        
        success, result = download_image(url, str(save_path))
        
        if success:
            size_kb = result / 1024
            print(f"       ✓ 成功 ({size_kb:.1f} KB)")
            success_count += 1
        else:
            print(f"       ✗ 失败: {result}")
            fail_count += 1
            failed_files.append((filename, url))
        
        # 间隔延迟，避免请求过快被封
        if i < len(IMAGES):
            time.sleep(DELAY)
    
    # 汇总报告
    print(f"\n{'='*60}")
    print(f"  下载完成!")
    print(f"  成功: {success_count} / 失败: {fail_count}")
    print(f"{'='*60}")
    
    if failed_files:
        print(f"\n  失败列表 (可手动下载):")
        for name, url in failed_files:
            print(f"    - {name}")
            print(f"      {url}")
    
    print(f"\n  文件保存在: {output_path.absolute()}")

if __name__ == "__main__":
    main()
