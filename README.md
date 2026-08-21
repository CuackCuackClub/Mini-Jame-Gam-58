# Duckula: Bloodbound

> A vampire duck, a cursed sword, and a race against time.

**Duckula: Bloodbound** is a 2D action-platformer developed for [**Mini Jame Gam #58**](https://itch.io/jam/mini-jame-gam-58).

The game is built around the jam's themes of **Time** and **Swords**. You play as a vampire duck cursed to wield a sword that feeds on his own blood. Your blood is both your **health** and your **remaining time**: it continuously drains as you explore the castle, forcing you to keep moving and fighting.

Defeating enemies restores blood, turning combat into a necessity for survival rather than something you can simply avoid.

## Core Concept

Your blood is constantly running out.

To survive, you must:

* Explore the castle before your blood runs dry.
* Fight enemies using your cursed sword.
* Defeat enemies to recover blood.
* Decide when spending blood on powerful abilities is worth the risk.
* Find and preserve Blood Vials for emergency situations.
* Reach the end before the curse consumes you.

The core gameplay loop is:

**Explore → Lose Blood → Fight → Kill → Recover Blood → Keep Moving**

## Gameplay

### Cursed Blood

Blood acts as the player's health and as a representation of time.

It continuously decreases during the game, creating constant pressure to progress.

Taking damage also reduces blood.

If your blood reaches zero and you have no way to recover, the run ends.

### Cursed Sword

The sword is the player's main weapon and a central part of the curse.

Combat is designed around close-range action, positioning, timing and resource management.

### Blood Recovery

Enemies are a vital resource.

Defeating them restores part of the player's blood, rewarding aggressive but calculated play.

Avoiding every encounter may save you from taking damage, but it also means losing opportunities to recover blood.

### Blood Abilities

Special abilities can provide powerful advantages at the cost of additional blood.

Using them creates a risk/reward decision:

**Spend blood to finish a fight faster, or conserve it and take a safer approach.**

### Blood Vials

The player can carry up to **3 Blood Vials**.

Blood Vials act as an emergency survival resource and can prevent the curse from ending a run prematurely.

## Progression

If development time allows, the game may include lightweight progression through sword or ability upgrades.

Possible upgrades include:

* Increased sword damage.
* Improved blood recovery.
* Reduced ability costs.
* New blood-based abilities.
* Movement improvements.

The priority of the jam version is a complete and polished core experience rather than a large progression system.

## Development

The game is being developed with:

* **Unity 6**
* **Unity Editor 6000.3.22f1**
* **C#**
* **Git & GitHub**

### Required Unity Version

Use:

```text
6000.3.22f1
```

Using the same Unity version across the team is strongly recommended to avoid unnecessary scene, package and project settings changes.

## Running the Project

1. Clone the repository:

```bash
git clone https://github.com/CuackCuackClub/Mini-Jame-Gam-58.git
```

2. Open **Unity Hub**.

3. Add the cloned repository as an existing project.

4. Open it using **Unity 6000.3.22f1**.

5. Open the main development scene from:

```text
Assets/Scenes/
```

6. Enter Play Mode.

## Controls

Controls will be documented here once the final input scheme is locked.

## Project Structure

The repository follows a standard Unity project layout:

```text
Assets/
Packages/
ProjectSettings/
```

Game-specific assets, scripts, prefabs, scenes, audio and UI are kept inside `Assets/`.

## Development Workflow

Development is managed through GitHub Issues and the **Mini Jame Gam 58 — Jam Board** project.

Work should be developed on short-lived branches and merged into `main` through Pull Requests.

Example branch names:

```text
feat/2-player-movement
feat/3-sword-combat
feat/4-blood-health
fix/enemy-death-reward
```

Commit messages should remain concise and descriptive.

Examples:

```text
feat(player): implement horizontal movement
feat(combat): add sword hit detection
feat(gameplay): add continuous blood drain
fix(enemy): prevent duplicate blood reward
```

## Game Jam

Created for:

**Mini Jame Gam #58**

Jam page:
https://itch.io/jam/mini-jame-gam-58

### Theme

**Time + Swords**

The project interprets the theme mechanically:

* **Time** is represented by the player's continuously draining blood.
* **Swords** are represented by the cursed weapon at the center of combat and the game's resource-management systems.

## Status

**In development for Mini Jame Gam #58.**

The immediate goal is to deliver a complete playable experience first, then use the remaining jam time for additional content, visual polish, audio and optional mechanics.

## Team

Developed by **CuackCuackClub**.

## License

See the repository's [LICENSE](LICENSE) file for licensing information.

