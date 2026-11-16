# Offline motion pipeline — notes, not code that runs yet

This folder is for the training / augmentation side. The Unity
runtime never calls into here directly; it consumes ONNX models
exported from here.

## Recommended path

1. **AI4AnimationPy** (facebookresearch/ai4animationpy) — the
   Starkes' Python successor to the Unity AI4Animation. Use it for
   mocap processing, Codebook Matching training, and inference
   experiments. It runs on NumPy + PyTorch, no Unity needed for R&D.
2. **Motion matching first** — build a clip database from CMU mocap
   + AMASS, retarget to our rig, and get a working controller
   before training anything neural.
3. **Augment offline** — HY-Motion 1.0 (text-to-motion) for new
   moves described in words ("crossover dribble, step back, fadeaway
   jumper"), VideoMDM for learning 3D motion from NBA broadcast
   video. Both are offline generators; distill their output into
   the clip database / codebook, don't run them live.
4. **Export** — trained Codebook Matching / MotionBricks model to
   ONNX, run in Unity via Sentis (or ONNX Runtime). The Unity side
   only sees `IMotionController.Step()` — see
   `unity/Assets/Scripts/Motion/IMotionController.cs`.

## Data — check licences before use

- CMU mocap: free for research, check redistribution terms.
- AMASS: research licence.
- AI4Animation: research / education only, basketball mocap not
  included anyway.
- MotionBricks: code Apache 2.0, motion-data licence restricted.
- NBA video: broadcast footage is copyrighted; using it as
  training input via VideoMDM is research, not a shipped asset.

## Hardware note

Training needs a GPU. This VM has none. Use RunPod (already
connected — see MEMORY.md, pod lesson: cloudType ALL, 1 GPU,
40GB volume, 40GB container disk, 2 vCPU, 15GB RAM) for training
runs, or a local machine with a CUDA GPU.
