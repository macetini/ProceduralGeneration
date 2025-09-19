# Procedural Dungeon Generation System

<img width="1408" height="481" alt="dungeon example" src="https://github.com/user-attachments/assets/63720359-76f6-4a3c-a579-12fbbc3f86f0" />

---

## 🚀 Live Demos

- **Dungeon Scene**  
  Press <kbd>Space</kbd> to generate an area map, <kbd>Enter</kbd> to populate it.  
  [Try it online!](https://macetini.github.io/ProceduralGeneration/Builds/DungeonScene/)

- **Node Scene**  
  Press <kbd>Space</kbd> to place a node in a room.  
  [Try it online!](https://macetini.github.io/ProceduralGeneration/Builds/NodeScene/)

---

## Overview

A flexible, modular procedural dungeon generation system for Unity (C#).  
Create diverse, replayable dungeons for games, prototypes, or experiments—perfect for roguelikes or custom level design.

---

## Getting Started

### Prerequisites

- **Unity**: 2018.4 or newer
- **C#**: 7.3 or newer

### Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/macetini/ProceduralGeneration.git
   ```
2. **Import into Unity**
   - Drag the `DungeonGeneration` folder into your project’s `Assets`.

---

## Usage

1. **Create a New Scene**  
   Open Unity and start a new scene.

2. **Add the Dungeon Generator**  
   Attach the `DungeonGenerator` script to any GameObject.

3. **Configure Parameters**  
   Use the Inspector to customize generation parameters.

4. **Generate a Dungeon**  
   Call the `GenerateDungeon` method—either in code or via the Inspector.

---

## API Reference

### DungeonGenerator

- `GenerateDungeon()`: Generates a new dungeon layout using current settings.

### NodeGenerator

- `GenerateNodeGraph()`: Builds a node graph structure for dungeon layout.

### ZonesGenerator

- `GenerateZones()`: Creates distinct zones within the dungeon, each with unique features.

---

## Features

- **Procedural Dungeon Creation**: Unique layouts for every run
- **Dynamic Paths & Connections**: Flexible linking between dungeon elements
- **Highly Customizable**: Tweak parameters for endless variety
- **Node-Based Graphs**: Branching, complex structures
- **Zone-Based Generation**: Distinct areas with configurable content

---

## Configuration

### Node Generation (`NodeGenerator`)

- `nodeCount`: Number of nodes in the graph
- `connectionChance`: Probability of linking nodes
- `nodeSize`: Size of each node

### Zone Generation (`ZonesGenerator`)

- `zoneSize`: Size of each zone
- `zoneShape`: Shape (rectangular, circular, etc.)
- `zoneContents`: What fills the zone (enemies, treasure, obstacles, etc.)

---

## Example Use Cases

- Themed dungeons for every playthrough
- Roguelike games with unique layouts on each run
- Custom algorithms building on the core system
- Complex multi-path dungeons via node graphs
- Distinct challenge/reward areas using zones

---

## Troubleshooting

- **Errors or Issues**:  
  Check the Unity console for error messages.

- **Dungeon Not Generating**:  
  Adjust generation parameters or review the `DungeonGenerator` script.

- **Node Generation Problems**:  
  Experiment with node count, connection chance, or node size.

- **Zone Generation Problems**:  
  Try different zone sizes, shapes, or contents.

---

## Contributing

Pull requests, suggestions, and bug reports are welcome!  
Please open an issue or submit a PR to help improve the system.

---

## License

Distributed under the MIT License.  
See [`LICENSE`](LICENSE) for details.
