# Procedural Dungeon Generation System

<img width="1408" height="481" alt="dungeon example" src="https://github.com/user-attachments/assets/63720359-76f6-4a3c-a579-12fbbc3f86f0" />

---

# Demo - Dungeon Scene
Press Space to generate an Area map and enter to Populate it.
https://macetini.github.io/ProceduralGeneration/Builds/DungeonScene/

# Demo - Node Scene
Press Space to place a node in a room.
https://macetini.github.io/ProceduralGeneration/Builds/NodeScene/

## Overview

This repository contains a flexible procedural dungeon generation system for Unity, implemented in C#. It enables developers to create diverse, replayable dungeons for games and experiments.

---

## Getting Started

### Prerequisites

- **Unity**: Version 2018.4 or newer
- **C#**: Version 7.3 or newer

### Installation

1. **Clone the Repository**
   - `git clone https://github.com/macetini/ProceduralGeneration.git`
2. **Import the Package**
   - Import the `DungeonGeneration` folder into your Unity project.

---

## Usage

1. **Create a New Scene**
   - Open Unity and start a new scene.
2. **Add the Dungeon Generator**
   - Attach the `DungeonGenerator` script to any GameObject.
3. **Configure Parameters**
   - Set generation parameters in the Inspector to customize dungeon behavior.
4. **Generate the Dungeon**
   - Call the `GenerateDungeon` method to build your dungeon.

---

## API Reference

### DungeonGenerator

- **`GenerateDungeon`**: Generates a new dungeon layout using current settings.

### NodeGenerator

- **`GenerateNodeGraph`**: Builds a node graph structure for dungeon layout.

### ZonesGenerator

- **`GenerateZones`**: Creates distinct zones within the dungeon, each with unique features.

---

## Features

- **Procedural Dungeon Creation**: Diverse layouts every run
- **Open Paths & Connections**: Dynamic linking between dungeon elements
- **Highly Customizable**: Tune parameters for unique results
- **Node-Based Graph Generation**: Complex, branching structures
- **Zone-Based Generation**: Distinct areas with specific characteristics

---

## Configuration Options

### Node Generation (`NodeGenerator`)

- **`nodeCount`**: Number of nodes in the dungeon graph
- **`connectionChance`**: Probability of linking nodes
- **`nodeSize`**: Size of each node

### Zone Generation (`ZonesGenerator`)

- **`zoneSize`**: Size of each zone
- **`zoneShape`**: Shape of zone (rectangular, circular, etc.)
- **`zoneContents`**: Contents (enemies, treasure, obstacles, etc.)

---

## Example Use Cases

- Themed or styled dungeons for every playthrough
- Roguelike games with unique layouts on each run
- Custom algorithms building on the provided system
- Complex multi-path dungeons via node graphs
- Distinct challenge/reward areas with zone generation

---

## Troubleshooting

- **Errors or Issues**
  - Check the Unity console for error messages.
- **Dungeon Not Generating**
  - Adjust generation parameters, or review the `DungeonGenerator` script for problems.
- **Node Generation Problems**
  - Experiment with node count, connection chance, or node size.
- **Zone Generation Problems**
  - Try different zone sizes, shapes, or contents.

---

## Contributing

Pull requests, suggestions, and bug reports are welcome! Please open an issue or submit a PR to help improve the system.

---

## License

Distributed under the MIT License. See [`LICENSE`](LICENSE) for details.
