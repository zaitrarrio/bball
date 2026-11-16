# Design — Basketball Sim 1v1 Prototype

## Goal

A 1v1 basketball game that looks like a real player, not a state
machine playing clips. The player should be able to dribble, change
direction, cross over, shoot, and be contested — and the defender
should react to the ball, not to a script.

Success for this prototype is narrow and checkable:

- Dribble looks attached to the hand (no ball sliding / teleporting).
- A crossover changes the dribble hand and the defender reacts.
- A jump shot has a readable gather, release, and follow-through,
  and the ball follows a real trajectory to the rim.
- A contested shot is measurably harder than an open one.
- AI-vs-AI can play a full game to 11 without getting stuck.

## Architecture

```
Input / AI intent ──► PlayerController ──► IMotionController ──► pose
                        │                      ▲
                        │ intent               │ current pose + phases
                        ▼                      │
                   BallController ◄── hand IK / contact
                        │
                   GameRules1v1 (score, possession, clock)
```

Three layers, deliberately separated:

### 1. Game brain (deterministic, testable without animation)

`Game/GameRules1v1.cs` — score to 11, win by 2, check-ball after a
made basket, 12-second shot clock, possession changes on a make,
a defensive rebound, or a steal. No animation code in here.

`AI/DefenderAI.cs` and `AI/OffensiveAI.cs` — decisions only: where to
stand, when to contest, when to shoot, when to drive. They output
the same `GameIntent` struct a human player outputs, so human and AI
are interchangeable.

### 2. Ball (physical, separate)

`Ball/BallController.cs` — a rigid body with gravity, court bounce
(restitution ~0.82 on the floor), rim and backboard as colliders,
and a shot solver that picks a launch velocity for a target entry
angle. States: `Dribbled`, `Held`, `InFlight`, `Loose`, `Dead`.
The ball is never parented to a hand. While dribbling, the hand is
driven *to* the ball by IK on the down-beat; while held, the ball is
constrained to the shooting pocket, not glued to a palm.

This is the single most important lesson from AI4Animation /
DeepDribble: if the ball is baked into the clip, every sharp turn
or contest looks wrong, because the ball follows the clip instead
of physics.

### 3. Motion (pluggable)

`Motion/IMotionController.cs`:

```csharp
PoseDelta Step(PoseState current, GameIntent intent, float dt);
```

Implementations, in order of increasing quality / cost:

1. **Procedural (this prototype, shipped now).** Locomotion blend
   from speed + direction, a dribble oscillator tied to a local
   phase per arm, a shot phase (gather → release → follow-through).
   Good enough to test the game loop, the ball, and the AI.
   It will look procedural — that is honest, and it is the baseline
   the neural controller has to beat.
2. **Motion matching.** A database of clips (CMU / AMASS / our own),
   queried by current pose + intent + local phases. Fast, proven in
   shipped games, and the fallback if a neural model is too slow.
3. **Codebook Matching (AI4AnimationPy, SIGGRAPH 2024).** A neural
   controller trained on basketball mocap. Export to ONNX, run in
   Unity via Sentis / ONNX Runtime. This is the target for v1.
4. **MotionBricks (NVIDIA, SIGGRAPH 2026).** In-betweening backbone
   for transitions between key poses. Use it for the hard transitions
   (crossover → drive, contest → recovery), not as the whole
   controller, until its Unity bridge exists.

Diffusion / text-to-motion models (HY-Motion, VideoMDM, MDM) are
**offline only**: generate and augment the clip library, then
distill into (2) or (3). Running a 1B-parameter diffusion model
live per player per frame is not a plan.

## Local phases

`Motion/MotionPhase.cs` — one phase per limb group (left leg, right
leg, left arm, right arm, torso). A phase is a 0–1 cycle position
plus a frequency. Dribbling is a phase on the dribbling arm; running
is a phase on the legs. Because they are independent, a player can
dribble while changing direction without the upper and lower body
snapping to a shared clip boundary. This is the core idea of
Starke et al. 2020, and it is what makes the basketball demo feel
different from a locomotion demo with a ball glued on.

## Data plan (the real bottleneck)

| Source | What it gives | Licence / gap |
|---|---|---|
| CMU Graphics Lab mocap | Free locomotion + some ball handling, retargetable | Quality varies; basketball-specific clips are thin |
| AMASS / SMPL | Large unified mocap corpus | Research licence; needs retargeting to our rig |
| AI4Animation repo | Code + demo, **no basketball mocap** | Research/education only; do not ship |
| MotionBricks pretrained | 350k+ clips, in-betweening | Code Apache 2.0; motion-data licence restricted — check before commercial use |
| VideoMDM (NBA video) | Learn 3D motion from broadcast 2D video | Research (June 2026); needs a pipeline, not a download |
| Own capture | Best quality, full control | Cost / time; a phone + Move AI / Rokoko is the cheap path |

Recommended order: CMU + AMASS to get motion matching working,
then VideoMDM-style augmentation from NBA video for basketball-
specific moves, then own capture for the signature moves (crossover,
step-back, fadeaway) if the game goes further.

## Controls (1v1, half-court)

- WASD / left stick — move (intent = direction + speed)
- Space — shoot (hold to gather, release to shoot; timing matters)
- Shift — crossover (swaps dribble hand, bursts the other way)
- Defender (AI): closes out on the ball, contests on shot gather,
  reaches on a predictable dribble rhythm — punished if it guesses
  wrong on a crossover.

## Out of scope for this prototype

5v5, full court, play-calling, seasons / stats, authored characters,
crowd, commentary, online play, mobile controls. All of those sit
on top of the 1v1 loop; none of them fix a bad 1v1 loop.

## Verification (honest)

- This VM has **no Unity, no .NET SDK, no GPU**. The C# in
  `unity/Assets/Scripts/` is written to Unity conventions and
  reviewed by reading, but it is **not compile-verified**.
- Next human step: open `unity/` in Unity 2022.3.11f1 on a real
  machine, press Play, and report the first compile errors.
  That is the P0.
- The web slice (separate artifact) is the playable feel-check
  that *can* be verified here, in a browser.
