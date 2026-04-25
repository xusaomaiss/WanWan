# Carrier Launch and Kill-Clear Design

## Summary

Add a short carrier takeoff intro, enemy kill-count clear rules, a three-second victory transition, and a more realistic explosion sprite for destroyed enemies.

## Confirmed Behavior

- On game start, the player fighter launches from a carrier deck.
- Combat starts one second after the fighter reaches the sky position.
- Enemy spawn, player input, and auto-fire remain disabled during the launch intro.
- Player kill count is tracked in the background.
- Low difficulty clears after 20 enemy kills, medium after 30, high after 40.
- Boss defeat still counts as victory, but ordinary enemy kill thresholds can also clear the stage.
- On victory, show `游戏胜利` for three seconds, then load the next level. Until multiple levels exist, the next level is the current `Game` scene reloaded.
- Destroyed enemies show a generated explosion image with a bright core, orange-red flame, and smoke edge, then fade/expand.

## Implementation Notes

- Keep all visuals runtime-generated in `RuntimeSpriteFactory`.
- Add a small `StageClearTarget` helper for testable kill thresholds.
- Add launch-state gating to `GameManager.IsPlaying`.
- Animate the player launch from `GameBootstrap` with a coroutine.
- Keep defeat flow unchanged except where victory timing changes.
