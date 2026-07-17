# SlotBase-10xtriple

Unity slot game. Core gameplay lives in `Assets/Scripts/Functionality/SlotBehaviour.cs`.

## Scene and Unity object changes

Do not modify `Assets/Scenes/*.unity`, prefabs, `.asset` files, `.meta` files, or any other
Unity-serialized object. All scene, hierarchy, and inspector work is done by the developer in
the editor. Restrict changes to C# under `Assets/Scripts/`.

Reading the scene YAML to understand the hierarchy or look up authored values is fine and
often necessary — just never write to it.

When code needs geometry or tuning values, expose them as `[SerializeField]` fields for the
developer to fill in. Never hardcode a value that duplicates scene data: the old
`private float topSlotImageY = 3393.2f` was a non-serialized copy of the reels' authored
`anchoredPosition.y`, so the scene and the code could drift apart silently.

## Reel motion

Reels are driven by constant-speed sweeps, not by icon recycling. `InitializeReelSpin` sweeps
a column down to `BottomY`, then loops `TopY -> BottomY` on `LoopType.Restart`;
`StopReelSpin` snaps to `TopY` and lands on `RestY`. Durations are always derived from a
speed (`reelSpeed`, `specialReelSpeed`) via `DurationFor`, so every move runs at the same
visual speed regardless of distance.

Two invariants matter here:

- **Never use `LoopType.Incremental`.** It makes the column's `localPosition.y` drift without
  bound, which previously had to be cancelled by a recycler mutating every icon's local Y.
  When the init/stop pairing broke, the two drifts stopped cancelling and the column rendered
  as an empty gap. `Restart` keeps the position bounded and stateless.
- **Nothing mutates icon positions at runtime.** The column transform moves; its children
  never do. This is what makes a missed stop harmless.

`TopY - BottomY` must be an exact multiple of the spin band's icon pitch, or the `Restart`
snap is visible. The reel mask is `SlotMain`, the *parent* of every reel.
