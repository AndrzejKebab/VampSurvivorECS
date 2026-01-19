# Vampire Survivor ECS

A high-performance "Survivor-like" game built on **Unity 6** using the **Data-Oriented Technology Stack (DOTS)**.

This project demonstrates a ECS architecture handling thousands of active units. It leverages the **Unity Character Controller** package for robust kinematic movement and utilizes **ZLinq** and **Burst** for optimized data processing. The architecture strictly follows an **MVC pattern**, separating Input/AI "Brains" from the core Physics "Engine."

---

## 🚀 Key Features

### 🧠 Core Architecture
*   **Unified Character Controller:** Both Player and Enemies share the same `CharacterPhysicsUpdateSystem` and `CharacterProcessor`.
    *   **Player Brain:** `PlayerFixedStepControlSystem` reads Input System data.
    *   **AI Brain:** `AIControllerSystem` uses spatial physics queries to drive movement.
*   **Pure ECS Camera:** A fully custom **Camera** implemented in ECS that handles:
    *   Rotation smoothing & pitch clamping.
    *   **Obstruction Detection:** Sphere-casts against the physics world to prevent clipping through walls (`OrbitCameraLateUpdateJob`).
    *   Interpolation support for smooth rendering.

### ⚔️ Combat & Gameplay
*   **"Ghost" Death State:** Implements a mechanism where dead entities remain in memory (for Camera tracking and Ragdoll visuals) but are stripped of logic and physics:
    *   *See `AttackJob.cs`:* Removes `PhysicsCollider`, adds `IsDeadTag`.
*   **Jobified Spawning:** `EnemySpawnerSystem` uses `EntityCommandBuffer.ParallelWriter` to spawn enemies in a ring pattern without stalling the main thread.
*   **Spatial Targeting:** AI uses `AllHitsCollector` and `CalculateDistance` physics queries to efficiently find targets.

### ⚡ Performance
*   **Burst Compiled Jobs:** All heavy logic (Physics, Spawning, Targeting, Camera Math) is Burst compiled.
*   **Zero-GC Allocation:** Uses `NativeList` and `Allocator.Temp` within jobs to avoid Garbage Collection spikes.

---

## 🛠 Tech Stack & Dependencies

**Engine:** Unity 6.4 (6000.4.0b4 or newer)

**Core Packages:**
*   **Entities:** 6.4.0 (ECS)
*   **Physics:** 1.4.4
*   **Character Controller:** 1.4.1 (Kinematic movement)
*   **Input System:** 1.18.0
*   **Burst:** 1.8.27
*   **Mathematics:** 1.3.3

**Third-Party Packages:**
*   **ZLinq:** 1.5.4 (Zero Allocation LINQ)

**Additional Dependencies:**
* Project is using [Unity Roslyn Updater](https://github.com/DaZombieKiller/UnityRoslynUpdater) to enable C# 11 features.

---

## 🏗 System Architecture

The project decouples **Intent** (Control) from **Execution** (Physics).

### 1. Data Layer (Components)
*   `CharacterControlComponent`: Shared component containing `float3 MoveVector` and `bool Jump`.
*   `PlayerInputsComponent`: Raw data from the New Input System.
*   `CameraComponent`: Settings for the Camera (Distance, Obstruction Radius, etc.).

### 2. Logic Pipeline

1.  **Input Gathering:** `PlayerInputsSystem` runs early in the frame to capture Hardware Input.
2.  **Brain Execution (Fixed Step):**
    *   `PlayerFixedStepControlSystem`: Converts Input -> Character Control (Relative to Camera).
    *   `AIControllerSystem`: Converts Physics Query Results -> Character Control.
3.  **Physics Engine:** `CharacterPhysicsUpdateSystem` reads `CharacterControl` and applies velocity/collision using the `KinematicCharacterProcessor`.
4.  **Combat:** `AttackSystem` processes damage and handles state changes (Alive -> Dead).
5.  **Camera:** `CameraSimulationSystem` and `CameraLateUpdateSystem` calculate the final camera transform after physics have settled.

---

## 📂 Installation & Setup

1.  **Clone the repository.**
2.  **Open in Unity 6.4** (Ensure DOTS packages are downloaded).
3.  **Open the Project:**
4.  **Authoring Setup:**
    *   **Player:** Requires `PlayerAuthoring` and `CharacterAuthoring`.
    *   **Enemy:** Requires `AIControllerAuthoring` and `CharacterAuthoring`.
    *   **Camera:** Requires `CameraAuthoring` (Linked to Player in Editor).
5.  **Press Play.**

---

## 🎮 Controls

*   **Mouse:** Lock cursor (Press **Escape** to toggle lock via `LockMouse.cs`).
*   **Mouse Move:** Orbit Camera.
*   **Mouse Scroll:** Zoom Camera.
*   **W/A/S/D:** Move Character relative to Camera view.
*   **Space:** Jump.

---

## 💡 Code Snippets

### The "Ghost" Death Logic
When a player dies, we do not destroy the entity immediately. This prevents the Camera (which follows the entity) from crashing or freezing.

*From `AttackJob.cs`:*
```csharp
if (PlayerTagLookup.HasComponent(targetEntity))
{
    // Disable Logic
    Ecb.AddComponent<IsDeadTag>(targetEntity);
    // Disable Physics (Enemies walk through body)
    Ecb.RemoveComponent<PhysicsCollider>(targetEntity);
}
else
{
    // Enemies are destroyed instantly
    Ecb.DestroyEntity(targetEntity);
}
```

---