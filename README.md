# Tracks of Divinity

**Genre:** Tower Defense  
**Engine:** Unity  
**Development Timeframe:** 3 weeks  
**Production Status:** Early production (Day 3)

---

## Overview

**Tracks of Divinity** is a tower defense game where the player actively constructs the enemy path during gameplay.  
At the end of each wave, the player must select and place a new track tile, shaping future enemy movement and strategic possibilities.

The game emphasizes long-term planning, adaptive strategy, and replayability through modular path construction.

---

## Core Gameplay

- Enemies follow a player-built track toward the endpoint
- Towers can be placed on valid zones along the track
- Killing enemies grants money used to buy towers
- Enemies that reach the end reduce player HP
- The game ends when HP reaches zero

At the end of each wave:
- The player chooses **one of three track tiles**
- Tiles are randomly selected from predefined presets
- The chosen tile permanently extends the track

---

## Track Tiles & Environments

Each track tile preset defines:
- Enemy path layout
- Tower placement options
- Environment type

Different environments provide gameplay modifiers.

- **Elemental Environments:** Provide specific advantages or effects
- **Plains:** Neutral environment  
  - Lower chance of path splits  
  - Higher chance of longer tracks  
  - No direct bonuses

---

## Setting

The world of **Aetherfall** has lost its gods.  
With no divine order left, primordial elemental spirits are reclaiming the land, forcing humanity into its last stronghold.

The player assumes the role of a newly appointed god, aiding humanity by shaping the paths the invading monsters must take and reclaiming the world piece by piece.

---

## Team & Roles

- **Sound Design, UI, Boss Functionality Concept**  
  Bote Bouma

- **Programming Lead**  
  Illia Vasylenko

- **Tile & Tower 3D Assets**  
  Thomas Feldbrugge

- **Monsters & Bosses 3D Assets**  
  Martin Petkov

- **Balancing**  
  Yanislav Yanev

- **Production & Coordination**  
  Kaloyan Dimov  
  (Communication, file structure, pseudocode, task delegation, progress tracking, code logic support)

---

## Current Development Status

- Partial 3D assets available
- Some sound effects present in the repository
- Gameplay scripts developed separately (not yet merged)
- Unity project not yet assembled

This repository currently serves as a shared production space during early development.

---

## Planned Next Steps

- Set up main Unity project
- Integrate existing assets and scripts
- Implement core systems:
  - Track placement
  - Enemy waves
  - Towers and combat
- Begin balancing and iteration
