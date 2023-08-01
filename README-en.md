# TankBattle

#### Introduction

This is a Unity assignment to create a Tank Battle game.

#### How to Play

After unzipping the file, run Build\Tank.exe to play the game.

#### Game Modes

1.  Single-player mode: One player controls the Tank 1 and eliminates all enemy tanks to pass the level and move onto the next one. The game ends if the base is destroyed before the level is passed. The player will respawn at the starting point after a certain amount of time if they are killed.
2.  Multiplayer mode: Players 1 and 2 cooperate to pass the levels. The enemy tank's health is doubled, but it is otherwise the same as the single-player mode.
3.  Two-player battle mode: Players 1 and 2 fight against each other in a symmetrical map. The first player to destroy the other's base wins. The player will respawn at the starting point after a certain amount of time if they are killed.

#### How to Play

1.  Player 1 uses WASD to move and the space bar or left mouse button to shoot.
2.  Player 2 uses arrow keys to move and the enter key or right mouse button to shoot.
3.  The tank's health will decrease when hit by an enemy's bullet and will also be temporarily slowed down. Friendly tanks can be hit by bullets.
4.  The tank will explode when its health reaches zero.
5.  The base's health will decrease when hit by a bullet and will be considered destroyed when its health reaches zero.
6.  Tanks and bullets can pass through grass. Enemy tanks hiding in the grass will have their health bars hidden.
7.  Bullets can destroy objects in the scene, and two adjacent objects can be hit at the same time.
8.  Colliding bullets will cancel each other out.
9.  Tanks cannot pass through rivers. They will catch fire and be damaged when on lava. Friction is reduced and speed is increased when on ice.

#### Enemy Tank Types

1.  Grey Tank: A mediocre tank with low health, low bullet damage, and medium speed and attack capability.
2.  Green Tank: A heavy tank with high health, high bullet damage, and a strong slowing effect. However, its bullets have a slow initial speed and long firing interval and are good at destroying walls.
3.  Red Tank: A special tank that gives the player a health recovery item and recovers a certain amount of health when killed. Its health, speed, and other attributes are generally low.
4.  Blue Tank: A rare but dangerous special tank that fires high-speed bullets with extremely high damage and moves very quickly. It is good at quickly advancing and destroying the player's base.