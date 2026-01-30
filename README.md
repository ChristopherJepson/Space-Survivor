# Space-Survivor: 2D Arcade Engine
### Unity 6 LTS | C# | Release Candidate v0.1.25

<div align="center">
  <img src="https://i.ibb.co/PGFYjn7g/Space-Survivor-0-1-25-Main-Menu.png" width="100%" height="auto" alt="Space Survivor Main Menu">
</div>

![Unity](https://img.shields.io/badge/Unity-6000.0.0+-black?style=flat&logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-blue)
![Status](https://img.shields.io/badge/Status-Gold%20Milestone-green)

> **Engineering Context:** This project serves as a comprehensive study of the **Unity Game Loop**, **UI/UX Architecture**, and **Component-Based Design**. It represents a fully shipped gameplay loop, demonstrating the ability to take a project from empty directory to compiled release.

---

## 🎮 Technical Features

### 1. Core Systems Architecture
Unlike prototype scripts, this project utilizes a structured architecture to manage game state and entity interactions.
- **Game Manager Singleton:** Implements a central `GameManager` to handle global state (Score, Lives, Level Progression) across scene transitions without data loss.
- **Event-Driven UI:** Decouples gameplay logic from UI updates using C# Events/Delegates, ensuring that the HUD updates reactively rather than polling every frame.
- **Object Pooling:** Implements a pooling system for projectiles and asteroids to optimize memory allocation and prevent Garbage Collection spikes during intense gameplay.

### 2. Gameplay Mechanics Implementation
- **Physics-Based Movement:** Utilizes Unity's `Rigidbody2D` physics engine for player propulsion, drag, and collision detection, ensuring smooth, predictable movement.
- **Dynamic Difficulty Scaling:** Features a progression algorithm that increases spawn rates and enemy velocity over time, keeping the engagement curve consistent.
- **Input System:** Mapped standard WASD/Space controls with potential for extension to the new Unity Input System.

### 3. UI/UX & Feedback Loops
- **Visual Feedback:** Integrated particle effects and sprite animations for collisions and power-ups to provide immediate user feedback (Game Feel).
- **Persistent Data:** Implements local save systems to track High Scores, demonstrating familiarity with data persistence (PlayerPrefs/JSON).

---

## 🤖 Methodology: Accelerated Engineering
This project was executed using an **AI-Augmented Workflow** to rapidly master the Unity Engine.
- **Syntax Acceleration:** Utilized LLMs to generate standard boilerplate code (e.g., UI Managers), allowing focus to remain on high-level logic and game feel.
- **Milestone Driven:** The development followed a strict milestone schedule, successfully delivering "Version 0.1.25" as the first Gold Candidate on time.

---

## 🕹️ Controls & Mechanics

| Key | Action | Technical Note |
| :--- | :--- | :--- |
| **W** | Fire Laser | Instantiates projectile from Pool |
| **Space** | Burst Engine | Applies `AddForce` impulse |
| **A / D** | Rotate | Modifies `Transform.Rotation` |

**Objective:** Survive the asteroid field. Difficulty ramps based on `Time.deltaTime`.

---

## 📦 Releases & Downloads
*Current Milestone: v0.1.25 (Windows 64-bit)*

| Version | Status | Notes |
| :--- | :--- | :--- |
| **[v0.1.25](https://github.com/ChristopherJepson/Space-Survivor/releases)** | **Stable** | UI Overhaul, Scoreboard Persistence, Final Audio |
| [v0.0.22](https://github.com/ChristopherJepson/Space-Survivor/releases) | Beta | Difficulty Ramping introduced |
| [v0.0.09](https://github.com/ChristopherJepson/Space-Survivor/releases) | Alpha | Core Movement & Shooting mechanics |

---

## 👤 Author
**Christopher Jepson**
*Technical Artist & Software Engineer*
[LinkedIn](https://www.linkedin.com/in/christopher-jepson-310a84308) | [Email](mailto:christopher.j.jepson@gmail.com)
