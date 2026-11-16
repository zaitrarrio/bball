# Verification — what is real, what is stubbed (2026-11-15)

## Real (verified by reading / running)

- **Project files**: README, docs/DESIGN.md, python/README.md,
  11 C# scripts (574 lines total), ProjectSettings/ProjectVersion.txt
  (Unity 2022.3.11f1). File listing verified 2026-11-15.
- **Web slice** (artifact `basketball-sim-1v1-prototype`, v1, live):
  audit passed with no console errors; crossover + shoot driven
  live, made basket 2–0, 24-second clock, scoring to 11, six-move
  library, Phase Lab. Not probe-verifiable: held-key movement,
  Web Audio (needs a first tap).
- **AI4Animation reference inspection**: repo metadata via GitHub
  API (8,926 stars, 1,141 forks, ~5.4 GB, no SPDX licence),
  SIGGRAPH 2020 ReadMe read directly, root README read directly
  (licence line at line 400). See `reference/REFERENCE.md`.
- **Licence**: AI4Animation — research/education only, no
  commercial use or redistribution; mocap CC BY-NC 4.0.
  Basketball mocap data is not in the repo at all.

## Stubbed / not verified

- **Unity C# scaffold**: written to Unity conventions, reviewed
  by reading only. **Not compile-verified** — no Unity Editor,
  no .NET SDK, no GPU on this VM. P0: open `unity/` in Unity
  2022.3.11f1 on a real machine, press Play, report compile errors.
  Known placeholders: court/rim built from primitives, keyboard
  controls are placeholders.
- **Motion controller**: only the procedural baseline exists.
  Motion matching, Codebook Matching, and MotionBricks are
  interfaces / roadmap, not code.
- **Ball / IK**: ball physics and shot solver are written, but
  there is no IK solver wired in Unity yet (the web slice notes
  the same gap). Hand-to-ball contact is a target position, not
  a solved IK chain.
- **Defender / offensive AI**: decision logic written, not
  play-tested in Unity. AI-vs-AI full-game stability is unproven
  outside the web slice.
- **AI4Animation prebuilt demo**: download never verified
  (fetch timed out). Do not present the zip as working.
- **Data**: no mocap in hand. CMU / AMASS / VideoMDM / own
  capture are all still to be sourced, each with its own licence
  to check. This is the critical path to a neural controller.

## Next steps (in order)

1. Play the web slice; note dribble timing and defender
   contest timing.
2. Open `unity/` in Unity 2022.3.11f1, fix the first compile
   errors, press Play.
3. Source mocap (CMU first — free, no new capture needed).
4. Build the motion-matching controller on that data.
5. Only then train a Codebook Matching controller in
   AI4AnimationPy and export to ONNX.
