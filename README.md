# Console RPG (C#)

A simple turn-based RPG made in C# using a console interface.

### Features

- Class system: Warrior, Mage, Archer
- Turn-based combat system
- Unique skills for each class
- Status effects (Poison, Shield Block)
- Enemy encounters (Goblin, Orc, Wyrm)
- Rest system with limited uses
- Basic progression after battles

# How it works

### The game flow:

1. Player enters a name
2. Chooses a class
3. Faces a sequence of enemies
4. Before each fight, the player can:
   - Fight
   - Rest (limited uses)
   - View status
5. Combat happens in turns:
   - Player chooses a skill
   - Enemy attacks back
   - Status effects are applied each turn
6. Game ends when:
   - Player defeats all enemies (win)
   - Player HP reaches 0 (game over)

# Classes

### Warrior

- Higher HP
- Balanced MP
- Skills:
  - Slash (damage)
  - Shield Block (damage reduction)
  - Battle Cry (increase attack)

### Mage

- Lower HP, higher MP
- Skills:
  - Fireball (high damage, MP cost)
  - Ice Barrier (damage + defense)
  - Arcane Blast (medium damage)

### Archer

- Balanced stats
- Skills:
  - Rapid Shot (damage)
  - Poison Arrow (damage over time)
  - Eagle Eye (attack boost)

### Enemies

- Goblin (weak)
- Orc (medium)
- Wyrm (strong)

# Technologies

- C#
- .NET Console Application

### What I learned

- Structuring a turn-based system
- Using enums for classes and enemies
- Organizing code into methods and classes
- Handling game loops and conditions
- Implementing simple status effects

# How to run

1. Open the project in Visual Studio
2. Run the application
3. Follow the instructions in the console

# Future improvements

- Add level system
- Add inventory and items
- Improve enemy AI
- Add more enemies and skills
- Create save/load system
- Build a graphical interface (GUI)

#   Notes

This project was built for learning purposes and to practice logic, structure, and game mechanics.
