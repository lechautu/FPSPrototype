# FPS Prototype – Dead Target–style (Unity / URP)

## Overview
This is a **Dead Target–style FPS prototype** built with Unity (URP), focusing on **core shooter gameplay**, **enemy swarm handling**, and **performance-oriented architecture for mobile-scale games**.

The goal of this prototype is **not content or polish**, but to demonstrate:
- FPS combat fundamentals
- Scalable enemy handling
- Performance-conscious design decisions

---

## Core Gameplay Loop
Spawn zombie wave
- Player aim & shoot (stationary FPS)
- Enemy swarm approaches
- Reload / manage ammo
- Clear wave → next wave (scaled)
- Stationary FPS (mobile-friendly)
- Hitscan shooting
- Headshot multiplier
- Wave-based progression with scaling

---

## Shooter Mechanics
- **FPS Camera**
  - Pitch/Yaw with clamping
  - Recoil system (kick + return)
- **Weapon System**
  - Hitscan raycast (`RaycastNonAlloc`)
  - Fire rate, reload, ammo state
  - Viewmodel recoil separated from camera recoil
- **Hit Feedback**
  - Hit marker UI
  - Blood VFX (Particle System)
  - Impact decals (URP Decal Projector)
    - Z-fighting & corner stretch handled

---

## Enemy System (Swarm-Oriented)
- Zombie AI states: Walk / Attack / Die
- Large enemy counts (40–80+)
- **No per-enemy `Update()`**
- Batch ticking (chunk-based updates)
- Simple movement (no NavMesh)
- Lightweight stagger/knockback on hit

---

## Performance Considerations
- Object pooling for:
  - Enemies
  - VFX
  - Impact decals
- Deferred enemy recycling (budgeted per frame)
  - Prevents frame spikes when many enemies die simultaneously
- Animator cost isolation (toggleable)
- URP configured with minimal overhead
- Debug HUD:
  - FPS
  - Alive enemy count
  - Memory usage

---

## Wave & Scaling System
- Wave-based spawning
- Enemy count increases per wave
- HP and movement speed scale progressively
- Designed to stress-test swarm performance rather than difficulty balance

---

## UI
- Ammo count display
- Reload timer with progress bar
- Minimal UI focused on gameplay feedback

---

## Technical Focus
This prototype intentionally emphasizes:
- FPS combat feel
- Enemy swarm scalability
- Runtime performance stability
- Clean separation of systems (input, combat, AI, VFX)

And intentionally **does not include**:
- Multiplayer
- Monetization
- Level design / large maps
- Story or progression systems
- Art or UI polish

---

## Known Limitations
- Single enemy type (zombie)
- Stationary player (no movement)
- Prototype-level visuals and animations
- Not optimized for low-end mobile devices yet

These limitations are **by design**, to keep the prototype focused on core shooter mechanics and performance.

---

## Target Platform
- Primary: PC (Editor / Standalone) for profiling and iteration
- Design considerations aligned with mobile FPS constraints

---

## Purpose
This prototype serves as a **technical showcase** of FPS gameplay understanding and performance-oriented Unity development, rather than a full game.
Video link: https://drive.google.com/file/d/1GWNVwjpVO58qvQQynSRdc9506CQwVOU_/view?usp=sharing
