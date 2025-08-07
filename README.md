# Procedural Dungeon Generation System

<img width="1408" height="481" alt="d2" src="https://github.com/user-attachments/assets/63720359-76f6-4a3c-a579-12fbbc3f86f0" />

<br /><br />
**Getting Started**

This project is a procedural dungeon generation system built in Unity using C#. To get started, follow these steps:

### Prerequisites

* Unity 2018.4 or later
* C# 7.3 or later

### Installation

1. Clone the repository into your Unity project directory.
2. Import the `DungeonGeneration` package into your Unity project.

### Usage

1. Create a new scene and add a `DungeonGenerator` script to a GameObject.
2. Configure the generation parameters in the Inspector to suit your needs.
3. Call the `GenerateDungeon` method to create a new dungeon.

**API Documentation**
-----------------

### DungeonGenerator
* `GenerateDungeon`: Generates a new dungeon based on the configured parameters.

### NodeGenerator
* `GenerateNodeGraph`: Generates a graph of nodes that can be used to create a dungeon.

### ZonesGenerator
* `GenerateZones`: Generates a set of zones within the dungeon, each with its own unique characteristics.

**Features**
------------

* Procedural generation of dungeons with varying characteristics
* Support for open paths and connections between elements
* Customizable generation parameters for diverse dungeon creation
* Node-based graph generation for creating complex dungeon structures
* Zone-based generation for creating distinct areas within the dungeon

**Node Generation**
-----------------

The `NodeGenerator` script can be used to generate a graph of nodes that can be used to create a dungeon. The graph can be customized using the following parameters:

* `nodeCount`: The number of nodes to generate in the graph.
* `connectionChance`: The chance that two nodes will be connected.
* `nodeSize`: The size of each node in the graph.

**Zone Generation**
-----------------

The `ZonesGenerator` script can be used to generate a set of zones within the dungeon. Each zone can have its own unique characteristics, such as:

* `zoneSize`: The size of the zone.
* `zoneShape`: The shape of the zone (e.g. rectangular, circular).
* `zoneContents`: The contents of the zone (e.g. enemies, treasure, obstacles).

**Example Use Cases**
--------------------

* Generate a dungeon with a specific theme or style
* Create a procedurally generated game with a unique dungeon each time the player starts a new game
* Use the dungeon generation algorithm as a starting point for your own custom generation system
* Use the node generation algorithm to create a complex dungeon structure with multiple paths and connections.
* Use the zone generation algorithm to create distinct areas within the dungeon, each with its own challenges and rewards.

**Troubleshooting**
-----------------

* If you encounter any issues or errors, please check the console output for error messages.
* If you're having trouble getting the dungeon to generate, try adjusting the generation parameters or checking the `DungeonGenerator` script for errors.
* If you're having trouble with node generation, try adjusting the node count, connection chance, or node size parameters.
* If you're having trouble with zone generation, try adjusting the zone size, shape, or contents parameters.
