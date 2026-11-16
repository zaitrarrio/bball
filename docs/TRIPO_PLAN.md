# Tripo plan — web v2 (prepared 2026-11-15; EXECUTED 2026-11-16)

Decisions already made: **Three.js** (stay; v1 is Three.js, GLB is
engine-agnostic), **animations yes** — Tripo rig + preset retarget.

## CORRECTIONS from execution (2026-11-16, verified live)

- **P2 text-to-3D with texture costs 110 credits ($1.10), NOT 20.**
  The 10/20-credit figures are H-series (v3.x) pricing. Actual
  per-task `credits_consumed` returned by the API: 110.0.
- **Rig v1.0-20240301 (the default) has REAL basketball presets:**
  `preset:biped:basketball_shot`, `preset:biped:crossover_dribble`,
  `preset:biped:dribble` — in the official 90+ preset list. The
  earlier "preset:shoot is generic, shot stays procedural" caveat
  is superseded: Tripo retargets genuine basketball moves on a
  v1.0 rig. Rig with `model: v1.0-20240301`, `spec: tripo`
  (NOT mixamo — mixamo spec breaks Tripo retarget on v1.0).
- **Actual v3 endpoints:** `POST /v3/generation/text-to-model`,
  `POST /v3/animations/rig-check`, `POST /v3/animations/rig`,
  `POST /v3/animations/retarget`, `GET /v3/tasks/{task_id}`,
  `GET /v3/account/balance`. Rig body field is `model`
  (rig model version), not `model_version`.
- **First authenticated call timed out** (60 s) on the egress-proxy
  CONNECT while the new host awaited approval; the retry worked
  in 0.6 s. If a first call to a new Tripo host times out, retry
  once before diagnosing further.

## Actual spend (2026-11-16, balance 2915 -> 2505)

| Step | Credits | USD |
|---|---|---|
| 2 x P2 text-to-3D textured (110 each) | 220 | $2.20 |
| 2 x prerigcheck | 0 | $0.00 |
| 2 x auto-rig v1.0 (25 each) | 50 | $0.50 |
| Retarget 7 clips/player (10 each) | 140 | $1.40 |
| **Total** | **410** | **$4.10** |

Clips per player (v1.0 rig, `preset:biped:*`): idle 15.37 s,
walk 2.37 s, run 1.27 s, dribble 9.3 s, crossover_dribble 9.8 s,
basketball_shot 6.6 s, jump 2.23 s. Dribble/crossover/shot clips
are long takes, not single moves — play sub-ranges in Three.js.
Both GLBs: 41-joint skeleton, Tripo-native bone names
(L_ToeBase, L_Foot, L_Calf, L_Thigh...), skinned mesh.
Attacker base mesh: 5,776 verts / 15,315 indices (~5k tris),
1 material, 3 PBR textures.

## Files (~/workspace/basketball-sim-1v1/assets/tripo/)

- attacker-p2.glb (3.2 MB, unrigged), attacker-anim-1.glb
  (3.8 MB: idle/walk/run/dribble/crossover_dribble),
  attacker-anim-2.glb (3.4 MB: basketball_shot/jump)
- defender-p2.glb (2.7 MB), defender-anim-1.glb (3.4 MB),
  defender-anim-2.glb (3.0 MB) — same clip sets
- Task ids: attacker gen d7e10fa6, rig 3aec61dc, retarget
  1440fec5 + 27b6ccb0; defender gen 4d6cc576, rig 31ddbc01,
  retarget e390ab51 + 8a709726.
- Helper: ~/workspace/custom-connectors/custom.tripo/bin/tripo.py
  (balance, text-to-model, task, rig-check, rig, retarget,
  download, wait). Connector doc:
  ~/workspace/custom-connectors/custom.tripo/CONNECTOR.md.

## Tripo facts verified 2026-11-15 (official docs + search)

- API is async tasks: submit, get `task_id`, poll `GET /task/{id}`
  or webhook, download immediately — **download URLs expire after
  5 minutes**.
- Auth: `Authorization: Bearer <api_key>`.
- v3 base: `https://openapi.tripo3d.ai` — `POST /v3/generation/text-to-model`.
  v2 base (`https://api.tripo3d.ai/v2/openapi`) also documented.
- 1 credit = $0.01. Text-to-3D: 10 credits no texture, 20 with
  standard texture. Image-to-3D: 20–30.
- Rig chain: `animate_prerigcheck` (free) → `animate_rig`
  (25 credits, `out_format: glb`) → `animate_retarget`
  (10 credits per animation). Retarget takes the **rig task id**,
  not the generation task id.
- Retarget: exactly one of `animation` (single preset) or
  `animations` (array, max 5). Output GLB with skeleton + clips.
- Rig v3.0-20260909 biped presets (official docs): `preset:idle`,
  `preset:walk`, `preset:run`, `preset:dive`, `preset:climb`,
  `preset:jump`, `preset:slash`, `preset:shoot`, `preset:hurt`,
  `preset:fall`, `preset:turn`. Rig v1.0 has 90+ presets.
- **Caveat:** `preset:shoot` is in Tripo's generic preset library;
  whether it is a basketball jump shot is NOT established — most
  likely a generic shooting pose. Do not rely on it for the shot.
  The shot, crossover, and dribble stay procedural.
- Model version: `P2-20260801` for game-ready quads
  (48–50,000 tris / 25,000 quads); `v3.1-20260211` for detail.
- **Billing:** Tripo Studio subscription (user's Pro, $21/mo)
  does NOT include API credits. API is separate pay-as-you-go.

## Cost estimate (API route, 2 players) — SUPERSEDED, see actuals above

| Step | Credits | USD |
|---|---|---|
| 2 × text-to-3D with texture (P2) | 2 × 110 | $2.20 |
| 2 × prerigcheck | 0 | $0.00 |
| 2 × auto-rig | 2 × 25 | $0.50 |
| Retarget 7 clips/player (idle, walk, run, dribble, crossover_dribble, basketball_shot, jump) | 14 × 10 | $1.40 |
| **Total** | **410** | **$4.10** |

## Assets to generate

1. **Attacker** — text-to-3D, P2, textured. Prompt (draft):
   "Full-body 3D game character, athletic male basketball player,
   red jersey number 7, white shorts, basketball shoes, standing
   in neutral A-pose, arms slightly out, game-ready low-poly,
   PBR textures, clean topology, symmetrical, no props, no ball"
2. **Defender** — same prompt, blue jersey number 23. Same body
   proportions requested so rig/scale match; verify scale in
   Three.js after download and normalise to 1.9 m height.
3. **Do NOT generate via Tripo:** basketball (procedural sphere +
   texture), hoop/backboard, court (procedural, already built).

## Pipeline (API route, once key is added)

1. Add Tripo API key via secure vault (custom.tripo connector,
   hosts `openapi.tripo3d.ai`, `api.tripo3d.ai`, Bearer header).
   Buy API credits separately — Studio Pro credits do not apply.
2. Submit both text-to-model tasks (P2, texture on), poll every
   2 s, download GLB immediately on success to
   `~/workspace/basketball-sim-1v1/assets/tripo/`.
3. Prerigcheck → rig each (glb). Record task ids.
4. Retarget on each rig task id, two batches (max 5 per call):
   batch 1 `animations: [preset:biped:idle, preset:biped:walk,
   preset:biped:run, preset:biped:dribble,
   preset:biped:crossover_dribble]`; batch 2
   `[preset:biped:basketball_shot, preset:biped:jump]`.
   Download both GLBs per player.
5. Inspect each GLB in Three.js: skeleton present, clip names,
   scale, jersey colours distinct, no missing textures.
6. Rebuild web artifact v2 (`artifact.edit` on
   basketball-sim-1v1-prototype): replace primitive players with
   the two GLBs; Tripo clips drive locomotion/idle; local phases
   drive limbs for dribble/crossover/shot; ball stays a separate
   physics object; keep HUD, Phase Lab, scoring, defender AI.

## Studio route (if user picks it)

User generates the same two prompts in Tripo Studio, rigs there,
exports GLB, sends the files. Same inspection + v2 rebuild steps
apply from step 5. No API key or credits needed from the assistant.

## Open questions for generation time — RESOLVED 2026-11-16

- `preset:biped:basketball_shot` IS a basketball shot clip (6.6 s
  take on the v1.0 rig), alongside `preset:biped:dribble` and
  `preset:biped:crossover_dribble`. All three retargeted
  successfully; verify visually in v2 before trusting timing.
- Do the two Tripo players come out at the same scale? Check
  bounding boxes on load in Three.js and normalise to 1.9 m.
- Tripo rig bone names vs our phase code: Tripo-native names
  (L_/R_ prefixes, e.g. L_Thigh, L_Calf, L_Foot). Map bones on
  load, do not assume names.
