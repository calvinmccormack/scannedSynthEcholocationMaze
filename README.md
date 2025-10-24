# ScanMaze: Improving Maze Navigation through Scanned-Synthesis Echolocation

Author: Calvin McCormack
Affiliation: Center for Computer Research in Music and Acoustics (CCRMA), Stanford University

--------------------------------------------------------------------

## Overview
ScanMaze is a Unity project that investigates how auditory feedback can train spatial navigation. The game pairs a moving visual scan line with scanned-synthesis audio so that, over repeated play, the brain learns to rely on sound rather than sight. The visual slice progressively narrows and can be disabled; players then navigate primarily by listening.

--------------------------------------------------------------------

## Gameplay
- Goal: Collect as many jewels as possible in 5 minutes while avoiding spikes.
- Scan: Press the scan button to sweep a horizontal slice across the maze. Objects intersecting the slice emit distinct sounds.
- Sonic design:
  - Jewels → smooth, harmonic scanned-synthesis tones.
  - Spikes → rough, abrasive scanned-synthesis tones.
- Difficulty: The blue visual slice becomes thinner over time and can be turned off to encourage auditory-first navigation.

--------------------------------------------------------------------

## Controls (Game Controller)
- Left Stick: Move
- Right Stick: Look; rotate camera
- R1 (or equivalent): Trigger scan

Tested with DualSense and Xbox-style controllers; any standard HID gamepad should work.

--------------------------------------------------------------------

## Installation and Run

Prerequisites
- Unity 2022.3 LTS or newer in the 2022 LTS line
- macOS or Windows
- A USB or Bluetooth game controller, but can also be reassigned to keyboard and mouse

Steps
1) Clone:
   git clone https://github.com/<username>/ScanMaze.git
   cd ScanMaze

2) Open in Unity:
   Open Unity Hub → Open → select the project folder.

3) Load scene:
   Open Assets/Scenes/SampleScene.unity

4) Run:
   Connect a controller and press Play in the Unity Editor.

5) Build (optional):
   File → Build Settings → choose your target platform → Build and Run.

--------------------------------------------------------------------

## VR / XR Support
ScanMaze can be built and run in VR. Controller and headset mappings must be configured in Unity for the specific device.

- Recommended stack: Unity XR Plugin Management with OpenXR; XR Interaction Toolkit for input and rig setup.
- PC VR (OpenXR/SteamVR/WMR): enable the corresponding OpenXR plugin features; map controller actions to the project’s input.
- Standalone Android VR; for example, Quest: supported in principle; however, Chunity (ChucK + Unity) on Android-based VR/XR is work in progress and an active area of development, performance varies by device, and controller input will need to be configured.

--------------------------------------------------------------------

## Technical Notes
- Engine: Unity 2022.x
- Audio: Chunity (ChucK + Unity) for real-time scanned-synthesis
- Input: Unity Input System; standard HID controllers
- Session length: default 5 minutes; configurable in the game manager code

--------------------------------------------------------------------

## Repository Layout
Only the folders needed for source control are included.

Assets/            # Scenes, scripts, audio logic, materials
Packages/          # Package manifest and lock
ProjectSettings/   # Unity project configuration

Generated folders are excluded via .gitignore:
Library/
Temp/
Logs/
UserSettings/
Build/
Builds/

--------------------------------------------------------------------

## Concept and Background
The project explores whether repeated exposure to informative auditory scans can strengthen reliance on sound for spatial mapping. It draws on prior work in human echolocation, auditory scene analysis, and scanned synthesis (Bill Verplank & Max Mathews). Intended applications include accessibility, training tools, and artistic practice.

Scanned Synthesis paper: https://ccrma.stanford.edu/~verplank/S2S/ScannedSynthesis.PDF

--------------------------------------------------------------------

## Acknowledgments
Developed at CCRMA, Stanford University in MUSIC 257 with guidance from Prof. Poppy Crum

--------------------------------------------------------------------

## License
Educational and research use permitted with attribution. Please credit the author and CCRMA in derived works.
