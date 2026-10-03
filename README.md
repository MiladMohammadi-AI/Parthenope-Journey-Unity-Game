# Parthenope Journey

**Developer:** Milad Mohammadi  
**Academic Supervisor:** Prof. Maurizio De Nino  
**Engine:** Unity 6 (6000.4.2f1)  
**Language:** C#

## Overview

**Parthenope Journey** is a 3D third-person academic game developed in Unity. The project turns a university environment into an interactive progression experience: the player enters the building, chooses a professional path, explores the environment, completes academic stages and quizzes, and progresses toward the final room.

The project was developed as an academic work under the supervision of **Prof. Maurizio De Nino**.

## Core Systems

- **Player Control** — third-person movement, camera-relative navigation, gravity and collision handling.
- **Interaction System** — reception interaction, profession selection and environment triggers.
- **Game Progression** — stage completion, unlocking logic and access to the final room.
- **Quiz System** — profession-based multiple-choice questions, timer, feedback and scoring.
- **Door System** — entrance and stage doors with trigger-based interaction and audio feedback.
- **Physics & Collision** — Character Controller movement and pushable Rigidbody objects.
- **Audio & Environment** — footsteps, door audio, ambient sound, lighting, materials and skybox environment.
- **Procedural Character Animation** — C#-driven walking motion without relying on external animation clips for the procedural movement system.

## Main C# Scripts

| Script | Responsibility |
| --- | --- |
| `PlayerMovement.cs` | Player input, movement and gravity |
| `SimpleCameraFollow.cs` | Third-person camera follow and rotation |
| `ReceptionInteraction.cs` | Reception trigger and profession selection |
| `CharacterSwitcher.cs` | Character switching after profession selection |
| `GameProgressManager.cs` | Academic stage and progression state |
| `StageQuizRoom.cs` | Quiz logic, questions, timer, score and completion |
| `DoubleEntranceDoor.cs` | Main entrance interaction |
| `StageSlidingDoor.cs` | Stage door locking and unlocking |
| `SlidingDoorAudio.cs` | Door sound feedback |
| `PlayerPushObjects.cs` | Physics interaction with pushable objects |
| `PlayerFootstepAudio.cs` | Footstep audio |
| `OutdoorAmbientZone.cs` | Outdoor ambient audio behavior |
| `FinalRoomController.cs` | Final sequence and game completion |
| `MiladProceduralAnimator.cs` | Procedural character walking animation |

## Technical Concepts

The implementation uses Unity concepts including `CharacterController`, `Transform`, `Vector3`, `Quaternion`, `Time.deltaTime`, `Update`, `LateUpdate`, trigger colliders, rigidbodies, `OnControllerColliderHit`, `AudioSource`, prefabs, materials, textures and script-to-script communication.

## Repository Structure

```text
Assets/          Game scenes, scripts, prefabs, characters, materials, audio and other assets
Packages/        Unity package manifest and lock file
ProjectSettings/ Unity project configuration
```

Unity-generated folders such as `Library`, `Temp`, `Logs`, `Obj` and `UserSettings` are intentionally excluded through `.gitignore`.

## Opening the Project

1. Clone or download this repository.
2. Open Unity Hub.
3. Select **Add project from disk** and choose the repository folder.
4. Use **Unity 6000.4.2f1** or a compatible Unity 6 installation.
5. Allow Unity to restore packages and regenerate local cache files.

## Academic Context

This repository documents the technical implementation of the project, including gameplay architecture, interaction logic, progression, quiz mechanics, physics, environment systems and procedural character animation.

### Acknowledgment

Special thanks to **Prof. Maurizio De Nino** for his academic supervision and guidance during the development of this project.

## Author

**Milad Mohammadi**  
2026
