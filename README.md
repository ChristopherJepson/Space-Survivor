# Space-Survivor: 2D Arcade Engine & CI/CD Pipeline
### Unity 6 LTS | C# | Python | GitHub Actions | Release Candidate v0.1.25

<div align="center">
  <img src="https://i.ibb.co/PGFYjn7g/Space-Survivor-0-1-25-Main-Menu.png" 
  width="100%" height="auto" alt="Space Survivor Main Menu">
</div>

![Unity](https://img.shields.io/badge/Unity-6000.0.0+-black?style=flat&logo=unity)
![Language](https://img.shields.io/badge/Language-C%23-blue)
![Pipeline](https://img.shields.io/badge/Pipeline-GitHub%20Actions-2088FF?style=flat&logo=githubactions)
![Status](https://img.shields.io/badge/Status-Gold%20Milestone-green)

> **Engineering Context:** This project is two things simultaneously — a fully 
> shipped Unity 2D arcade game and a working CI/CD pipeline. The game exists 
> to give the pipeline something real to protect. Every commit passes through 
> custom quality gates before it reaches the build. The pipeline is the product.

---

## 🔧 Pipeline Architecture

### 1. Shift-Left Quality Automation

This project implements a "shift-left" CI/CD strategy — catching issues at 
commit time rather than at build time.

- **Custom Python Linter** (`custom_linter.py`): A bespoke static analysis 
  tool that intercepts deprecated Unity APIs, misconfigured component 
  references, and structural anti-patterns before they reach the repository. 
  Written in Python, integrated directly into the pre-commit hook chain.

- **Git Pre-Commit Framework** (`.pre-commit-config.yaml`): Configured 
  pre-commit hooks that execute the custom linter and enforce code quality 
  standards on every commit — preventing build-breaking changes from entering 
  the repository. No commit bypasses the quality gate.

- **GitHub Actions Workflows**: Automated CI pipeline triggering on push and 
  pull request events, validating build integrity and enforcing quality checks 
  across the development lifecycle.

### 2. Versioned Release Management

The project follows a structured release pipeline from prototype to production:

| Version | Stage | Gate Criteria |
| :--- | :--- | :--- |
| v0.0.9 | Alpha | Core movement and shooting mechanics verified |
| v0.0.22 | Beta | Difficulty scaling algorithm validated |
| **v0.1.25** | **Gold** | UI overhaul, scoreboard persistence, final audio — full release candidate |

Each release is a compiled Windows 64-bit artifact produced from a clean 
build of the main branch after passing all pipeline quality gates.

---

## 🎮 Game Systems Architecture

The game itself is the test environment for the pipeline. The engineering 
decisions below reflect the same performance and architecture principles 
that apply at the build pipeline layer.

### Core Systems

- **Game Manager Singleton:** Central `GameManager` handles global state 
  (Score, Lives, Level Progression) across scene transitions without data loss.
- **Event-Driven UI:** Decouples gameplay logic from UI updates using C# 
  Events/Delegates — reactive updates rather than per-frame polling.
- **Object Pooling:** Pooling system for projectiles and asteroids eliminates 
  Garbage Collection spikes during intensive gameplay — same memory 
  management discipline that applies to build farm resource allocation.

### Gameplay & Physics

- **Physics-Based Movement:** `Rigidbody2D` for player propulsion, drag, and 
  collision detection.
- **Dynamic Difficulty Scaling:** Progression algorithm increases spawn rates 
  and enemy velocity over time via `Time.deltaTime`.
- **Persistent Data:** Local save system for High Score tracking 
  (PlayerPrefs/JSON).

---

## 🤖 Methodology: AI-Augmented Development Pipeline

This project was developed using a documented AI-augmented workflow — 
treating AI tooling as a pipeline component with known capabilities 
and limitations.

- **Agentic Code Orchestration:** Claude Code CLI used for multi-file 
  code orchestration and verified deliverable generation — with 
  deliberate separation from Cline's PLAN-mode analysis layer.
- **Syntax Acceleration:** LLMs used to generate standard boilerplate 
  (UI Managers, event wiring), keeping architectural focus on system 
  design and pipeline integration rather than syntactical implementation.
- **Milestone-Driven Delivery:** Strict milestone schedule enforced 
  through the pipeline — Alpha, Beta, and Gold releases each represent 
  a verified, gated build artifact.

---

## 🕹️ Controls

| Key | Action | Technical Note |
| :--- | :--- | :--- |
| **W** | Fire Laser | Instantiates projectile from Pool |
| **Space** | Burst Engine | Applies `AddForce` impulse |
| **A / D** | Rotate | Modifies `Transform.Rotation` |

---

## 📦 Releases & Downloads

*Current Milestone: v0.1.25 (Windows 64-bit)*

| Version | Status | Notes |
| :--- | :--- | :--- |
| **[v0.1.25](https://github.com/ChristopherJepson/Space-Survivor/releases/download/v0.1.25/Space-Survivor.0.1.25.zip)** | **Gold** | UI Overhaul, Scoreboard Persistence, Final Audio |
| [v0.0.22](https://github.com/ChristopherJepson/Space-Survivor/releases/download/v0.0.22/Space-Survivor.0.0.22.zip) | Beta | Difficulty Ramping introduced |
| [v0.0.09](https://github.com/ChristopherJepson/Space-Survivor/releases/download/v0.0.9/Space-Survivor.0.0.9.zip) | Alpha | Core Movement & Shooting mechanics |

---

## 👤 Author

**Christopher Jepson**  
*Build & Tools Engineer*  
[LinkedIn](https://www.linkedin.com/in/christopher-jepson-310a84308) | 
[Email](mailto:christopher.j.jepson@gmail.com)
