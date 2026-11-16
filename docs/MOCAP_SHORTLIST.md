# CMU mocap shortlist — basketball sim 1v1

Prepared 2026-11-15. Source: CMU Graphics Lab Motion Capture Database
(http://mocap.cs.cmu.edu), subjects list and full trial listing checked
2026-11-15. This corrects two assumptions in docs/DESIGN.md.

## Correction 1: basketball coverage is NOT thin

CMU has two dedicated basketball subjects, 48 trials total, and they
map almost one-to-one onto the prototype's six-move library.

### Subject #6 — "dribble, shoot basketball" (15 trials, 120 Hz)

| Trial | Motion | Maps to |
|---|---|---|
| 2–5 | forward dribble | dribble locomotion |
| 6–7 | backward dribble | retreat / reset |
| 8–9 | sideways dribble | lateral dribble |
| 10 | forward dribble, 90-degree left turns | change of direction |
| 11 | forward dribble, 90-degree right turns | change of direction |
| 12 | forward dribble, 90-degree right turns, crossover dribble | crossover |
| 13 | low, fast freestyle dribble, dribble through legs | between-legs |
| 14 | crossover dribble, shoot | crossover → shot chain |
| 15 | dribble, shoot | gather → release |

### Subject #102 — "Basketball" (33 trials, 120 Hz)

| Trials | Motion | Maps to |
|---|---|---|
| 1–4 | Right/LeftWideTurn, SuperTightLeft/Right | tight turns |
| 5–8 | RunningStraight, RunningWideRight/Left, RunningTighterRight | drive locomotion |
| 11–12, 15 | OffensiveMoveSpinLeft (x2), SpinRight | spin move |
| 13–14 | OffensiveMoveGoRight / GoLeft | first-step drive |
| 16–17 | WalkEvasiveLeft / Right | hesitation / evasive step |
| 18–19 | FeintLeftMoveRight, FeintRightMoveLeft | hesitation / fake |
| 20–21 | FakeShotBreakRight / Left | pump fake → drive |
| 22–28 | DefensiveStraight (no stop / with stop), DefensiveRight/LeftStopToStop, ZigZag, SideToSide (x2) | defender motion — the contest side |
| 29 | Pivoting | pivot / jab step |
| 30–32 | StraightDriveFromStop, RightDrive, LeftDrive | drive from triple threat |
| 33 | RightTightTurn | tight turn |

Also worth grabbing: Subject #15, trial 12 — "basketball: dribble,
lay-up shot, pass" (lay-up, the one finish neither subject above
names).

## Correction 2: the licence is better than "research only"

CMU's own site (front page and FAQs, checked 2026-11-15):

- "This dataset of motions is free for all uses."
- "You may include this data in commercially-sold products, but you
  may not resell this data directly, even in converted form."
- "The motion capture data may be copied, modified, or redistributed
  without permission."
- Attribution requested if results are published: "The data used in
  this project was obtained from mocap.cs.cmu.edu. The database was
  created with funding from NSF EIA-0196217."

So unlike AI4Animation (research/education only, no commercial use)
and AMASS (research licence), CMU clips can go into a shipped
product — they just can't be resold as data. DESIGN.md's "free for
research, check redistribution terms" line is now superseded for CMU.

## Gaps that remain

- **Step-back** is not named in any trial. Closest raw material:
  Subject #102 trials 20–21 (fake shot, break) and 18–19 (feints).
  A true step-back still needs HY-Motion/VideoMDM generation or own
  capture, as DESIGN.md already plans.
- **No ball in the data.** CMU captures the body only; the ball is
  still a separate simulated object, exactly as the scaffold's
  BallController already treats it. Hand-to-ball contact stays an IK
  problem, not a data problem.
- **Retargeting still needed.** Files are ASF/AMC (+ C3D) at 120 Hz
  per trial; the CMU Tools page lists asf/amc viewers and amc2bvh
  conversion. Nothing here is Unity-ready out of the box.

## How to get it

- Browse: http://mocap.cs.cmu.edu/subjects.php
  (Subject #6, Subject #102) or the full listing
  http://mocap.cs.cmu.edu/search.php?maincat&
- Per-trial downloads (ASF/AMC/C3D) from each trial's page, e.g.
  Subject #6 trial 12:
  http://Mocap.cs.cmu.edu/search.php?subjectnumber=6&trinum=12
- The FAQ asks not to crawl the database; it offers zipped
  subject bundles instead (subjects 5–9 in one zip, which covers
  Subject #6). See http://mocap.cs.cmu.edu/faqs.php.
- Suggested first pull: all of Subject #6 (15 trials) + Subject #102
  trials 11–21 and 22–28 (offensive moves + defender set) + Subject
  #15 trial 12 (lay-up). That is the motion-matching seed database.

## Next step (unchanged order)

1. Play the web slice; note dribble timing and defender contest timing.
2. Open unity/ in Unity 2022.3.11f1, fix first compile errors, press Play.
3. Pull the CMU trials above, convert ASF/AMC → BVH, retarget to the
   rig, build the motion-matching controller on that data.
4. Only then train Codebook Matching in AI4AnimationPy and export ONNX.
