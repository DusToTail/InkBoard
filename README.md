# InkBoard

////////////////////
////Descriptions////
////////////////////
This is a Unity URP 3D project that aims to create board games revolving around a table-top board, dices, pieces and MOST IMPORTANTLY INK.
Concept (inspired by Splatoon):
A cube (or dice) that moves by rotating towards a specific direction one tile at a time. After having moved, the tile(s) underneath will be covered in ink. There can be special abilities attached to each side of the cube, triggered when moving.

////////////////////
//////Designs///////
////////////////////
1. Player cube:

2. Enemies:

3. Environment:

////////////////////
//////Gameplay//////
////////////////////
Genre: Rythm, Puzzle
Objectives (modes):
+ Paint all tiles within the limited time.
+ Reach the goal within the limited time.
+ Destroy all objectives (or enemies).
+ Progress through the levels with puzzles.

1. Movement:
Moves in cardinal directions with WASD.
Rotates 90 degrees.
There is collision detection when two objects trying to be in the same tile.
Collision is resolved by calculating the priority of movement of the objects (player -> enemies -> obstacles)

2. Painting:
Each face by default does not paint.
Each face can have basic painting functionality, allowing it to paint the tile right beneath whenever the cube moves.

3. Damage:
Player cube can not crush or kill anything without partially painting it first (50% half submerged)
Any object (including player cube) can not move whilst being painted, thus need one action to unpaint itself.
Player cube when moving will crush and insta-kill any enemies at least half submerged in ink.


4. Abilities:

Calculations:
1. Rythm system plays rythm with beats.
2. Per beat timer starts counting and repeats.
3. Player provides movement inputs by pressing WASD. The inputs can be VALID or INVALID:
+Valid input: if the input is within the 10% error away from the beat and no consecutive inputs are made within the evaluation of the beat
    +Early
    +Late
    +Perfect
+Invalid input: others
4. If the input is invalid, the player does nothing. If it is valid, the input will be recorded and executed.
5. For enemies, they move according to their AI.
+The AI is provided with:
    +Priority queue within the ranking of the enemies
    +The shared collective mappings of other AIs' next tiles
    +The location of the player after first noticing the presence of the player
+Using all those information, the AIs one-by-one will evaluate their actions and put populate the Action Queue
+The player's input will become action and populate the Action Queue


////////////////////
////Spefications////
////////////////////
Unity version: 2021.3.9f1 LTS
Rendering Pipeline: URP
Target platforms: Windows and Mac
