# Carrier Launch and Kill-Clear Implementation Plan

1. Add `StageClearTarget` and tests for low/medium/high thresholds.
2. Add launch gating and kill-count victory state to `GameManager`.
3. Add carrier deck and explosion sprites to `RuntimeSpriteFactory`.
4. Animate player takeoff in `GameBootstrap`.
5. Replace enemy destruction burst with image-based explosion in `EffectsController`.
6. Update HUD/overlay text for kills and `游戏胜利`.
7. Run validation, Unity EditMode tests, Android smoke, commit, and push.
