# Beavers-Job
Beaver Simulation Game
This repository contains a turn-based resource management game developed using Unity. Players control beavers, gather resources, build dams and lodges, and manage a dynamic environment to meet the game’s win conditions.

Features
Turn-Based Strategy: Manage beaver actions like moving, building, and gathering.
Dynamic Environment: Procedural tile-based terrain (trees, rivers) evolves over time.
Resource Management: Collect wood, stone, and food to build structures and maintain the colony.
Structure Building: Construct dams, lodges, and canals, each affecting gameplay and win conditions.

Installation
1. Clone repository.
2. Open in Unity and load the MainMenu scene.

Core Mechanics

1. Beaver Actions
Scripts: BeaverManager.cs, Beaver.cs
Functionality: Beavers can move, chop trees, collect resources, build structures (dams, lodges, canals), and repair damaged buildings.
How it works: Each beaver has a set number of movement points and action points per turn. The BeaverManager script handles player inputs, including selecting beavers and triggering their actions. The Beaver.cs script tracks individual beaver stats like movement points and handles action execution. Tile-based interactions (e.g., chopping trees or collecting resources) are managed through Tilemaps.
Movement: Movement costs are calculated based on tile types (e.g., water or land), with terrain modifications affecting pathfinding and movement points.
2. Resource Management
Scripts: ResourceManager.cs, UIManager.cs
Functionality: The game tracks resources like wood, stone, and food, which are required for building and maintaining structures.
How it works: The ResourceManager.cs script manages resource accumulation and depletion. Resources are collected by beavers performing actions (e.g., chopping trees for wood) and displayed in real-time using UIManager.cs. The system updates based on game events, such as beavers finishing a task or completing a turn.
3. Building System
Scripts: DamBuilder.cs, TileManager.cs, BeaverManager.cs
Functionality: Players can construct dams, lodges, and canals using the resources collected.
How it works: The DamBuilder.cs script handles building placement based on the player’s input and resource availability. The system checks valid tile positions on the Tilemap and updates the constructionTilemap to reflect new buildings. BeaverManager.cs tracks beaver actions related to building, ensuring the player has enough resources (checked via ResourceManager.cs) and managing structure placement.
4. Environment Interaction
Scripts: TileManager.cs, RiverFlow.cs, TreeData, TileData
Functionality: The environment is dynamic, with trees growing, resources spawning, and rivers flowing, affecting gameplay.
How it works: TileManager.cs controls the procedural generation and updates of tiles on the map. Trees grow and age based on a timer system, while resources like branches and stones appear randomly on available tiles. The RiverFlow.cs script simulates the river’s movement, which can interact with and damage player-built structures like dams.
5. Turn-Based System
Scripts: GameManager.cs, BeaverManager.cs
Functionality: The game progresses in turns, with each turn allowing the player to manage their resources, beavers, and environment.
How it works: The GameManager.cs script manages the overall game state, including the number of turns, win/lose conditions, and turn transitions. Each turn, the game updates the environment (e.g., trees growing, resources spawning) and resets beaver actions for the next round. Turn events like checking win conditions are triggered at the end of each turn, using parameters set in the LevelConfig.cs.
