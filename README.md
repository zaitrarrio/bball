# Basketball Sim — 1v1 Prototype

High-quality 1v1 basketball simulation, built from the lessons of
AI4Animation (Sebastian Starke, SIGGRAPH 2020, Local Motion Phases)
and the latest real-time motion-generation models.

Scope for this prototype: **one-on-one, playable, first to 11, win by 2,
half-court**. No 5v5 yet — the 5v5 game brain is a separate layer that
only makes sense once the 1v1 motion actually looks and feels right.

## What is in this repo

- `unity/` — Unity C# project scaffold (Unity **2022.3.11f1**, the version
  AI4Animation's SIGGRAPH 2024 README recommends; URP). No Unity Editor
  is installed on the build VM, so nothing here is compile-verified in
  Unity yet — that is the explicit next step on a machine with Unity.
- `unity/Assets/Scripts/` — the game, split so the animation layer can
  be swapped without touching the game rules:
  - `Core/` — intent structs, bootstrap, court builder (primitives, for
    Editor testing with no authored art yet)
  - `Motion/` — `IMotionController` interface, local-phase tracking, and
    a procedural fallback controller. The neural controller (Codebook
    Matching / MotionBricks, via ONNX) plugs into the same interface.
  - `Ball/` — the ball as a separate simulated rigid body, never baked
    into an animation clip
  - `Player/` — human player controller (move, dribble, crossover, shoot)
  - `AI/` — 1v1 defender AI and a simple offensive AI for AI-vs-AI demo
  - `Game/` — 1v1 rules: score, possession / check-ball, shot clock
- `web/` — the playable browser slice (Three.js, self-contained
  single HTML file). Open `web/index.html` in a browser: 1v1,
  first to 11, 24-second clock, six-move library, defender AI,
  Phase Lab. v2 uses the real Tripo player models embedded in
  the page.
- `assets/tripo/` — the Tripo source assets: P2 player models
  (attacker red #7, defender blue #23), auto-rigged v1.0 with
  7 retargeted clips each (idle, walk, run, dribble,
  crossover_dribble, basketball_shot, jump). Generated via the
  Tripo API, 2026-11-16, 410 credits total. The `-merged.glb`
  files are the ones embedded in `web/index.html`. These are
  binary and are NOT pushed through the GitHub connector's
  snapshot push; see `docs/TRIPO_PLAN.md`.
- `reference/` — local only, NOT committed: sparse, read-only
  reference checkout of sebastianstarke/AI4Animation (root README
  only; the full repo is ~5.4 GB, so a full clone was abandoned —
  see `reference/REFERENCE.md`). Its basketball mocap data is **not**
  included in that repo and is not redistributed here. Licence:
  research / education only, not for commercial use or
  redistribution; mocap under CC BY-NC 4.0. Do not copy its code
  into this project.
- `docs/DESIGN.md` — architecture, the lessons applied, the model
  roadmap, and the data / licensing gaps.
- `python/` — notes for the offline motion pipeline (AI4AnimationPy
  training, HY-Motion / VideoMDM augmentation).

## Lessons applied (from AI4Animation)

1. **Local motion phases, not whole clips.** Each body part carries its
   own phase, so hand-ball contact is asynchronous from foot contact.
   `Motion/MotionPhase.cs` tracks a phase per limb group.
2. **A learned controller, not a state machine.** `IMotionController`
   is queried every frame from current pose + intent and returns the
   next pose. The procedural controller in this prototype is a
   stand-in; the neural controller replaces it, not the game code.
3. **The ball is physical.** `Ball/BallController.cs` simulates the
   ball with gravity, bounce, rim/backboard collision, and a shot
   trajectory solver. The hand meets the ball via IK; the ball is
   never parented to a hand animation.
4. **Data is the bottleneck.** There is no basketball mocap in the
   reference repo. See `docs/DESIGN.md` for the data plan (CMU mocap,
   AMASS, VideoMDM from NBA video, MotionBricks pretrained clips —
   each with its own licence to check).

## Latest models (Nov 2026)

- **Codebook Matching / AI4AnimationPy (SIGGRAPH 2024, Meta)** —
  the direct successor. Python framework, learns the motion manifold
  and sampling end-to-end. Best near-term path for a trained
  basketball controller.
- **NVIDIA MotionBricks (SIGGRAPH 2026)** — single 224M backbone,
  350k+ clips, ~2ms / generation reported. Preview, kinematic, no
  Unity plugin in the box. Best for the in-betweening layer once we
  have a bridge.
- **Score Matching (Sony, 2026)** — diffusion-based replacement for
  motion matching, aimed at sports games. Watch, don't build on yet.
- **HY-Motion 1.0 / VideoMDM** — offline only. Generate / augment
  the move library from text and NBA video, distill into the runtime
  controller. Never run these live at 60fps.

## How to run (Unity)

1. Install Unity **2022.3.11f1** (Hub).
2. Open `unity/` as a project.
3. Open `Assets/Scenes/Main.unity`, press Play.
4. Controls (placeholder, keyboard): WASD move, Space shoot / jump
   shot, Shift crossover, E pass — n/a in 1v1, kept for 2v2 later.
   Defender is AI; press T to toggle AI-vs-AI demo.

## Status

- [x] Reference clone started (see `reference/`) — RESOLVED 2026-11-15: full clone abandoned (repo is ~5.4 GB; 361 MB in 12 min, ~6%). Sparse checkout kept (root README only) + `reference/REFERENCE.md` inspection: 3-demo menu, basketball mocap NOT shared, licence research/education only + mocap CC BY-NC 4.0, local phases usable outside NNs.
- [x] Design doc + Unity C# scaffold
- [x] Procedural web slice (playable in browser, for feel-checking
  the loop before Unity) — v1 done 2026-11-14, verified live.
- [x] Web v2 with real Tripo models — done 2026-11-16: P2 players
  (red #7 / blue #23), v1.0 rig, 7 mocap clips each, embedded in
  `web/index.html`. Verified headlessly; live play-check pending.
- [ ] Unity compile + Editor Play verification (needs Unity on a
  real machine — **not** verified on this VM, which has no Unity,
  no GPU, and no .NET SDK)
- [ ] Trained neural controller (needs mocap data — see DESIGN.md)
- [ ] 5v5 game brain (out of scope for this prototype)

## Licence notes

This project's own code is new work. AI4Animation is research /
education only — do not ship its code or weights commercially.
Check the licence of every mocap dataset and every pretrained
model before using it in a release build.
