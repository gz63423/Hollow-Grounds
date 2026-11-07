THE HOLLOW GROUNDS

Independent Software Development Project







PROJECT INTRODUCTION



The Hollow Grounds is a 2D wave-survival game that I designed and developed independently using Unity and C#.



I started this project to learn game development, but over time it grew into a much larger project involving programming, artificial intelligence, data management, game systems, and problem-solving.



I have worked on nearly every part of development myself, including programming gameplay mechanics, creating enemy behaviors, designing progression systems, making pixel art, producing audio, and debugging problems as they arise.



The project has given me experience working on a large software project from its initial concept to a publicly available game demo on Steam.







PROJECT OVERVIEW



Project: The Hollow Grounds

Developer: Gavin Zastrow

Development Type: Independent / Solo Development

Game Engine: Unity

Programming Language: C#

Genre: 2D Wave Survival

Platform: Windows PC, Linux

Version Control: Git / GitHub



The game is centered around surviving increasingly difficult waves of enemies while upgrading the player, defending a house, and building long-term progression.



As development progressed, I added more interconnected systems, including enemy AI, combat mechanics, player progression, saving, currency management, and multiple upgrade systems.







TECHNOLOGIES \& TOOLS USED



Unity



\- Used for the overall game development, scene management, physics, animations, and gameplay systems.



C#



\- Used to program gameplay mechanics, enemy behavior, progression systems, data management, and user interfaces.



Pixilart.com



\- Used to create pixel art and visual assets for the game.



FL Studio



\- Used for audio production and sound design.



Git



\- Used to track changes to the project and maintain version history.



GitHub



\- Used to host and manage the project's source code.







KEY ENGINEERING \& TECHNICAL FEATURES



1\. ENEMY AI \& BEHAVIOR



I programmed the enemy systems to detect nearby targets, decide whether to pursue the player or the house, and move toward their selected target.



Some of the features include:



\- Checking distances between enemies, the player, and the house.

\- Selecting targets based on proximity and other conditions.

\- Aggro locking to prevent enemies from constantly switching targets.

\- Moving enemies toward their selected targets.

\- Different enemy types with their own stats and behaviors.

\- Increasing enemy difficulty as the player progresses through waves.



2\. WAVE GENERATION \& DIFFICULTY SCALING



I created a wave spawning system that manages enemy encounters and gradually increases the difficulty as the player progresses.



Some of the features include:



\- Automatically spawning enemies based on the current wave.

\- Managing wave progression and completion.

\- Adjusting enemy health and damage as waves increase.

\- Configurable enemy statistics and spawn settings.

\- Special enemy and boss encounters.

\- Managing transitions between waves.



3\. DATA MANAGEMENT \& SAVE SYSTEMS



I developed systems to save and manage player progression so that important information is maintained between gameplay sessions and scene changes.



Some of the features include:



\- Saving player currency and statistics.

\- Tracking upgrade levels and progression.

\- Using Unity PlayerPrefs to store persistent data.

\- Managing permanent upgrades and rebirth progression.

\- Keeping temporary and permanent upgrades separate.

\- Using events to notify other systems when player data changes.



4\. COMBAT \& DAMAGE SYSTEMS



I programmed the combat systems that handle interactions between the player and enemies.



Some of the features include:



\- Player attack mechanics and damage calculations.

\- Enemy health and damage processing.

\- Armor and equipment systems.

\- Attack speed and damage upgrades.

\- Poison and freezing abilities.

\- Damage-over-time calculations.

\- Managing interactions between attacks, animations, and equipment.



5\. SCENE MANAGEMENT \& SYSTEM ARCHITECTURE



I developed systems to manage the different scenes and make sure important gameplay objects and data remain consistent when moving between them.



Some of the features include:



\- A persistent player instance that survives scene transitions.

\- Scene loading and transition management.

\- First-launch and startup initialization.

\- Managing persistent gameplay objects.

\- Coordinating the initialization of different game systems.

\- Preventing duplicate player instances and initialization issues.



6\. ECONOMY \& PROGRESSION SYSTEMS



I created an economy and progression system that manages player resources, purchases, and upgrades throughout the game.



Some of the features include:



\- Currency management and purchasing.

\- Upgrade prices and calculations.

\- Soulstones as a permanent progression currency.

\- Rebirth systems and progression requirements.

\- Player net worth calculations.

\- Temporary upgrades that reset and permanent upgrades that carry over.

\- Integration with Steam leaderboards.



7\. USER INTERFACE \& EVENT SYSTEMS



I programmed different UI systems that communicate with gameplay systems and update when important information changes.



Some of the features include:



\- Dynamic upgrade menus.

\- Player statistics and currency displays.

\- Event-based UI updates.

\- Enemy index and progression interfaces.

\- Achievement and objective tracking.

\- Menu navigation and gameplay feedback.







SOFTWARE DEVELOPMENT \& ENGINEERING PRACTICES



Throughout development, I have learned and applied different practices that help keep a project organized and easier to maintain.



Version Control:

I use Git and GitHub to track changes, maintain previous versions, and keep a history of development.



Code Organization:

I separate different gameplay systems into individual scripts so that they can be maintained and updated without unnecessarily affecting other parts of the game.



Debugging:

A large part of development has involved finding and fixing problems. Some examples include enemy targeting issues, scene initialization, animation synchronization, and interactions between different scripts.



Data Management:

I have worked on keeping player information consistent across different systems, scenes, and gameplay sessions.



Testing \& Improvements:

I regularly test new features, identify problems, and make adjustments as the project continues to grow.







SCREENSHOTS \& GAMEPLAY



The following Google Drive link displays different parts of the current game.



https://drive.google.com/drive/u/0/folders/1hLr2Sukz-jq768BB9XjtXIr334bM7siR







DEVELOPMENT CHALLENGES



While developing The Hollow Grounds, I have encountered and worked through many problems involving different systems interacting with each other.



Some examples include:



AI Initialization:

I encountered situations where enemies attempted to select targets before the player had finished initializing. I had to adjust how these systems communicate and handle initialization.



Animation Synchronization:

I worked through issues where buffered attacks could cause player armor animations to become unsynchronized with the actual player animation.



Persistent Data:

I had to make sure player progression remained consistent when switching scenes and restarting the game.



Enemy Targeting:

I developed and adjusted the targeting logic to make enemies prioritize the house while still allowing certain conditions to cause them to pursue the player.



System Compatibility:

As more features were added, I had to make sure existing systems continued working correctly with new mechanics and upgrades.



Working through these problems has taught me a lot about debugging, organizing code, understanding how different systems depend on each other, and finding solutions when something doesn't work as expected.







PROJECT LINKS



Steam:

https://store.steampowered.com/app/5212390/The\_Hollow\_Grounds/



YouTube:

https://www.youtube.com/@the\_hollow\_grounds



Instagram:

https://www.instagram.com/the\_hollow\_grounds



TikTok:

https://www.tiktok.com/@the\_hollow\_grounds







ABOUT THE DEVELOPER



My name is Gavin Zastrow, and I am an independent game developer.



I started developing The Hollow Grounds to learn Unity and C#, and it eventually became a much larger project than I originally expected.



Working on this game independently has given me experience with programming, debugging, software organization, artificial intelligence, data management, and problem-solving.



One of the biggest things I have learned is how different systems can depend on and interact with each other, and how important it is to make sure everything works together correctly.



I plan to continue learning about software development, computer science, and engineering while working on new projects and improving my technical skills.







The Hollow Grounds is an independent development project created by Gavin Zastrow. This repository contains source code and development materials.

