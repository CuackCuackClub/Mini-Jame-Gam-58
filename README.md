# Duckula: Bloodbound

> A vampire duck, a cursed sword, and a race against time.

**Duckula: Bloodbound** is a 2D action-platformer / hack-and-slash developed by **CuackCuackClub** for [**Mini Jame Gam #58**](https://itch.io/jam/mini-jame-gam-58).

The game combines the jam themes of **Time** and **Swords** directly into its core mechanics.

You play as a vampire duck cursed to wield a sword that feeds on his own blood. Your Blood is both your **health** and one of your most important resources: it continuously drains while you explore, is lost when taking damage, and can also be deliberately spent to use powerful abilities.

The only way to keep the curse under control is to fight.

Defeating enemies restores Blood, turning combat into a resource-management decision rather than something you can simply avoid.

---

## Play the Game

The completed jam build is available on itch.io:

### [Play / Download Duckula: Bloodbound](https://noemifar.itch.io/duckula-bloodbound)

Repository:

### [CuackCuackClub/Mini-Jame-Gam-58](https://github.com/CuackCuackClub/Mini-Jame-Gam-58)

---

## Core Concept

Your Blood is constantly running out.

To survive, you must:

- Explore the castle before dawn.
- Fight enemies with your cursed sword.
- Defeat enemies to recover Blood.
- Spend Blood carefully on special abilities.
- Use checkpoints and Blood Vials to survive fatal mistakes.
- Navigate traps, pits and hostile encounters.
- Reach and defeat the Final Boss.
- Escape before the curse — or the sunrise — finishes you.

The core gameplay loop is:

**Explore → Lose Blood → Fight → Kill → Recover Blood → Keep Moving**

But Blood is not your only enemy.

The level also has a **five-minute night-to-day cycle**, creating a second time limit that continues progressing even after Blood Vial respawns.

---

# Gameplay

## Cursed Blood

Blood acts as the player's **health and combat resource**.

It continuously decreases during gameplay, creating constant pressure to keep progressing.

Blood is also lost by:

- Taking enemy damage.
- Using Blood-powered abilities.

Blood can be restored by:

- Defeating enemies.
- Reviving through the Blood Vial system.

Blood is always clamped to its maximum value and the HUD updates immediately whenever it changes.

If Blood reaches zero, the death system is triggered.

---

## Cursed Sword

The cursed sword is Duckula's main weapon and the central element of the game's combat.

Combat focuses on:

- Close-range positioning.
- Attack timing.
- Enemy attack patterns.
- Movement.
- Blood management.
- Knowing when to attack aggressively and when to preserve resources.

Enemies restore Blood when defeated, meaning combat is directly tied to survival.

---

## Blood Recovery

Enemies are not only obstacles — they are also resources.

Different enemy types restore different amounts of Blood when defeated.

This creates an important trade-off:

**Avoiding enemies reduces immediate risk, but also removes opportunities to recover Blood.**

Enemy Blood rewards are granted only once per legitimate death, preventing duplicate recovery while remaining compatible with room resets and respawns.

---

# Blood Abilities

Duckula can deliberately sacrifice Blood to use special combat abilities.

These abilities are powerful, but spending too much Blood can leave the player vulnerable.

Abilities cannot directly spend enough Blood to reduce the player to zero.

## Bloodstep

**Blood Cost: 5**

Bloodstep is a fast horizontal dash.

It provides:

- Rapid horizontal movement.
- A way to reposition during combat.
- Brief protection from contact damage.
- Collision-aware movement that respects level geometry.

Passive Blood drain continues during Bloodstep.

---

## Blood Crescent

**Blood Cost: 15**

Blood Crescent launches a ranged Blood projectile in the direction Duckula is facing.

By default, it:

- Deals **25 damage**.
- Travels horizontally.
- Can damage multiple enemies once each.
- Stops when hitting level geometry.
- Uses the same enemy damage and death systems as sword combat.

Enemies killed with Blood Crescent still restore Blood normally.

---

# Blood Vials

Duckula can carry up to **3 Blood Vials**.

Blood Vials act as extra lives and are integrated with the room and respawn systems.

When a recoverable death occurs and a vial is available:

1. One Blood Vial is consumed.
2. The player returns to the current room or checkpoint.
3. Blood is restored.
4. Player movement and physics are restored.
5. Enemies belonging to the current room reset.
6. Defeated enemies in that room can be fought again.

Enemies outside the active room are not affected.

Blood Vials are therefore not simply health potions — they are part of the game's death and encounter-reset system.

---

# Checkpoints & Death

The level contains checkpoints that establish safer respawn positions as the player progresses.

The player can die from several causes, including:

- Blood depletion.
- Enemy attacks.
- Falling into pits or lethal zones.
- Reaching daylight before completing the level.

Recoverable deaths can use Blood Vials.

When normal recovery is no longer possible, the game transitions into its final death / Game Over flow.

The Final Boss encounter also contains dedicated respawn handling and visual feedback.

---

# Race Against Dawn

In addition to the player's continuously draining Blood, the main level contains an independent **five-minute time limit**.

During these five minutes, the environment transitions dynamically from:

**Night → Dawn → Daylight**

The sky changes progressively throughout the level and can be seen through windows, ruined walls and exterior openings.

The timer:

- Runs independently from Blood.
- Continues after Blood Vial respawns.
- Does not reset when the player dies and revives.
- Creates an absolute limit on how long the player can remain in the level.

If daylight is reached before the player completes the level, the final death sequence is triggered.

This gives the game two overlapping forms of time pressure:

**Your Blood is running out — and so is the night.**

---

# Enemies

The final jam build includes multiple enemy archetypes with different movement, combat and Blood-reward characteristics.

## Melee Enemy

The standard grounded enemy.

Characteristics:

- Patrols the environment.
- Detects and chases the player.
- Attacks at close range.
- Medium movement speed.
- Medium durability.

Default statistics:

- **Health:** 50
- **Damage:** 10
- **Speed:** 2
- **Blood Reward:** 10

---

## Flying Enemy

A faster airborne enemy with dedicated movement behavior.

Characteristics:

- Hover patrol.
- Vertical movement.
- Player detection.
- Approach and retreat combat behavior.
- Higher mobility but lower durability.

Default statistics:

- **Health:** 30
- **Damage:** 5
- **Speed:** 4
- **Blood Reward:** 5

---

## Heavy Melee Enemy

A slower but significantly more dangerous melee enemy.

Characteristics:

- High health.
- Heavy attacks.
- Slower movement.
- High Blood reward.

Default statistics:

- **Health:** 150
- **Damage:** 25
- **Speed:** 1
- **Blood Reward:** 20

---

# Final Boss

The game culminates in a dedicated **Final Boss encounter**.

The boss has its own:

- Combat behavior.
- Attack logic.
- Damage and death flow.
- Animations.
- Music.
- Sound effects.
- Arena handling.
- Respawn behavior.

The boss restores **30 Blood** when defeated.

The exit cannot trigger victory while the boss is still alive.

After defeating the boss, the victory route becomes available and the player can complete the game.

---

# Game Over & Victory

## Game Over

A Game Over state can be triggered when the player reaches an unrecoverable death condition.

The player can then restart the game or return to the Main Menu.

## Victory

Victory is gated behind defeating the Final Boss.

After the boss is defeated, reaching the victory exit triggers:

- Victory validation.
- Victory visual effects.
- Fireworks.
- Final Victory UI.

The victory exit cannot be used to bypass the boss.

---

# Player Movement

The player movement system includes:

- Horizontal movement.
- Jumping.
- Coyote time.
- Jump buffering.
- Bloodstep dash.
- Collision-aware movement.
- Improved ground detection.

The player has animation states for:

- Idle.
- Walking.
- Jumping.
- Attacking.
- Hurt.
- Bloodstep / Dash.
- Defeated.

The camera follows the player smoothly while respecting the level boundaries.

---

# Controls

## Keyboard & Mouse

| Action | Controls |
| --- | --- |
| Move Left | `A` / `Left Arrow` |
| Move Right | `D` / `Right Arrow` |
| Jump | `Space` / `W` |
| Sword Attack | `Left Mouse Button` / `E` / `L` |
| Bloodstep | `Left Shift` / `Right Shift` / `J` |
| Blood Crescent | `Q` / `K` |

Some Blood abilities also include gamepad bindings through Unity's Input System.

---

# HUD

The gameplay HUD displays the player's most important survival information.

It includes:

- Current Blood.
- Continuous Blood drain feedback.
- Damage and healing updates.
- Blood Vial count.
- Support for **0–3 Blood Vials**.
- Fully depleted Blood state.

The HUD updates immediately when gameplay values change.

---

# Audio

Duckula: Bloodbound includes music and sound effects integrated across the final jam build.

## Music

Dedicated music is used for:

- Main Menu.
- Main gameplay level.
- Final Boss encounter.

## Sound Effects

Sound effects are integrated with:

- Player animations and actions.
- Enemy animations and actions.
- Combat.
- Boss encounter.
- Gameplay feedback.

## Audio Options

The Options menu provides independent volume controls for:

- **Music Volume**
- **SFX Volume**

---

# Visual Effects & Polish

The final build includes several gameplay and presentation effects, including:

- Dynamic night-to-day background.
- Checkpoint visual feedback.
- Respawn visual effects.
- Holy-cross respawn feedback during the Final Boss encounter.
- Combat feedback.
- Hurt animations.
- Death animations.
- Blood ability effects.
- Victory fireworks.

---

# Development

The game was developed with:

- **Unity 6**
- **Unity Editor 6000.3.22f1**
- **C#**
- **Unity Input System**
- **Git**
- **GitHub**

## Required Unity Version

Use:

```text
6000.3.22f1
```

Using the same Unity version is strongly recommended to avoid unnecessary changes to scenes, packages and project settings.

---

# Running the Project

1. Clone the repository:

```bash
git clone https://github.com/CuackCuackClub/Mini-Jame-Gam-58.git
```

2. Open **Unity Hub**.

3. Add the cloned repository as an existing project.

4. Open it using:

```text
Unity 6000.3.22f1
```

5. Open one of the game scenes.

The main gameplay scenes include:

```text
Assets/Level/Scenes/Lvl1.unity
Assets/Level/Scenes/Boss.unity
```

6. Enter Play Mode.

For the complete jam experience, start from the game's Main Menu flow.

---

# Project Structure

The repository follows the standard Unity project layout:

```text
Assets/
Packages/
ProjectSettings/
```

The project separates game content into areas for:

- Art.
- Animations.
- Audio.
- Code.
- Gameplay systems.
- Levels and scenes.
- Materials.
- Prefabs.
- UI.
- Visual effects.

Scripts use the project's `S_` naming convention where applicable.

Examples include systems for:

- Player Blood.
- Player death.
- Enemy behavior.
- Enemy damage.
- Blood Vials.
- Rooms and checkpoints.
- Gameplay HUD.
- Blood abilities.
- Dynamic time.
- Victory and Game Over.
- Audio management.

---

# Development Workflow

Development was managed through GitHub Issues and Pull Requests.

Features were developed on short-lived branches and merged into `main` after integration and testing.

Example branch names:

```text
feat/4-blood-health
feat/6-blood-recovery
feat/7-death-flow
feat/10-blood-vial-system
feat/12-blood-ability
feat/dynamic-time-background
fix/boss-respawn
```

Commit messages follow concise, descriptive conventions such as:

```text
feat(gameplay): implement blood health system
feat(gameplay): restore blood on enemy defeat
feat(combat): add blood-powered abilities
feat(level): add dynamic five-minute dawn cycle
fix(boss): restore final boss combat
```

---

# Game Jam

Created for:

**Mini Jame Gam #58**

Jam page:

https://itch.io/jam/mini-jame-gam-58

Game page:

https://noemifar.itch.io/duckula-bloodbound

## Theme

**Time + Swords**

Duckula: Bloodbound interprets both themes directly through gameplay.

### Time

Time is represented through two connected systems:

- Blood continuously drains while playing.
- The level transitions from night to daylight over five minutes.

The player is therefore fighting against both an immediate resource timer and an absolute level timer.

### Swords

The cursed sword is Duckula's primary weapon and the catalyst for the Blood-based survival mechanics.

Combat is not optional resource expenditure: defeating enemies is one of the player's primary methods of extending survival time.

---

# Status

**Jam build completed and published.**

Duckula: Bloodbound was created for **Mini Jame Gam #58** and is available on itch.io.

**Play / Download:**  
https://noemifar.itch.io/duckula-bloodbound

The final jam build includes:

- Complete player movement.
- Sword combat.
- Continuous Blood drain.
- Blood recovery from enemies.
- Bloodstep.
- Blood Crescent.
- Blood Vials.
- Checkpoints.
- Room-based enemy resets.
- Multiple enemy archetypes.
- Lethal environmental hazards.
- Five-minute night-to-day cycle.
- Dynamic background.
- Gameplay HUD.
- Music and sound effects.
- Music and SFX volume settings.
- Game Over flow.
- Final Boss.
- Boss-specific music and combat.
- Victory flow.
- Victory visual effects.

---

# Team

## CuackCuackClub

**CuackCuackClub** was founded by:

- **Meritxell Alguero Manrique / Mei** — Co-Founder, Lead Developer, Programmer
- **Iago Prieto Lamas / Koala** — Co-Founder, Programmer, Technical Writer

## Associated Artist

- **Noemi Farre Porta / MimiOnly** — Associated Artist (2026), Artist, Animator, UI & UX Designer

---

# Duckula: Bloodbound Credits

## Developed By

- **Meritxell Alguero Manrique / Mei** — Lead Developer, Programmer, Level Designer
- **Iago Prieto Lamas / Koala** — Programmer, Technical Writer, UI & UX Designer
- **Noemi Farre Porta / MimiOnly** — Artist, Animator, UI & UX Designer

---

## Third-Party Assets

The following third-party assets were used during the development of **Duckula: Bloodbound**.

### Unity Asset Store

- **BoldPixels Font** — YukiPixels
- **2D Simple UI Pack** — OArielG
- **Monsters_Creatures_Fantasy** — Luiz Melo
- **Monsters_Creatures_Fantasy 2** — Luiz Melo
- **Bringer Of Death (free)** — Clembod
- **Pixel Art Potion Pack - Animated** — karsiori
- **Pixel 2D Castle Tileset** — Szadi Art
- **Pixel Art RPG VFX Lite** — Pixogen
- **Asset FTTGR | Free Pixel Art Platform** — Superposition Principle

Unless otherwise stated by the original author or distributor, Unity Asset Store assets remain subject to their respective **Unity Asset Store license terms** and are not licensed under this project's MIT License.

---

## Audio & Sound Effects

- Sound effects obtained from [**Pixabay**](https://pixabay.com/) and used under the applicable **Pixabay Content License**.
- Music obtained from [**Pixabay**](https://pixabay.com/) and used under the applicable **Pixabay Content License**.

### Music Credits

- **"[Song Name]"** — [Artist / Author]
- **"[Song Name]"** — [Artist / Author]
- **"[Song Name]"** — [Artist / Author]

> Replace the placeholders above with the exact track titles and authors before the final credits are considered complete.

---

## Special Thanks

- **Jame Gam Community**
- Everyone who played, tested and supported the game during the jam.
- **Thank you for playing Duckula: Bloodbound!**

---

# License

The original source code created for **Duckula: Bloodbound** is licensed under the **MIT License**.

See the repository's [LICENSE](LICENSE) file for details.

## Third-Party Content

The MIT License applies only to original project code and content created by the **Duckula: Bloodbound** development team, unless otherwise stated.

Third-party assets are **not automatically licensed under the MIT License** and remain subject to their respective licenses and terms.

This project includes content distributed under licenses or terms including:

- **Unity Asset Store licensing terms**
- **Pixabay Content License**
- Other asset-specific licenses where applicable

Third-party assets, fonts, music, sound effects, artwork and other externally sourced content remain the property of their respective authors and publishers.

See the [Third-Party Assets](#third-party-assets) and [Audio--Sound-Effects](#audio--sound-effects) sections for attribution information.
````
