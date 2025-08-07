# Procedural Dungeon Generation System

<img width="1408" height="481" alt="d2" src="https://github.com/user-attachments/assets/63720359-76f6-4a3c-a579-12fbbc3f86f0" />

## Overview

This project is a procedural dungeon generation system built in Unity using C#. It generates dungeons by creating and connecting elements, such as rooms and corridors, using a combination of randomization and algorithms.

## Features

* Procedural generation of dungeons with varying characteristics
* Support for open paths and connections between elements
* Customizable generation parameters for diverse dungeon creation

## Requirements

* Unity 2018.4 or later
* C# 7.3 or later

## Usage

1. Clone the repository into your Unity project directory.
2. Import the `DungeonGeneration` package into your Unity project.
3. Create a new scene and add a `DungeonGenerator` script to a GameObject.
4. Configure the generation parameters in the Inspector to suit your needs.
5. Call the `GenerateDungeon` method to create a new dungeon.

## API Documentation

* `DungeonGenerator`: The main script responsible for generating dungeons.
	+ `GenerateDungeon`: Generates a new dungeon based on the configured parameters.
	+ `GetAllTwoWayOpenElementsShuffled`: Returns an array of all two-way open elements in a shuffled order.
* `DRandom`: A custom random number generator used for procedural generation.

## Contributing

Contributions are welcome! If you'd like to help improve the project, please fork the repository and submit a pull request with your changes.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
