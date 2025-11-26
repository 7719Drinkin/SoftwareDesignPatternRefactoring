# Unity RPG Game

A 2D action RPG built with Unity featuring a player character with various combat abilities, enemy AI with state machines, and a comprehensive skill system.

## 🎮 Features

### Player System
- **State Machine Combat**: Idle, Move, Jump, Attack, Dash, Counter Attack, and special abilities
- **Skill System**: 
  - Sword Skills (Aim, Throw, Catch)
  - Dash Attack with critical hits
  - Blackhole ability with clone attacks
  - Assassinate skill for teleportation
  - Thunder Strike and Crystal abilities
- **Equipment System**: Weapons, armor, accessories with stat modifiers
- **Inventory Management**: Items, equipment, consumables (flasks)
- **Elemental Effects**: Fire (Ignite), Ice (Chill), Lightning (Shock)

### Enemy AI
- **NightBorn Boss**: Multi-phase boss with various states
  - Move, Battle, Attack, Blocked, Stunned, Dead states
  - Gather state for healing
  - Wave state for ranged elemental attacks
  - Magic Circle attacks
  - Teleportation abilities
- **Skeleton Enemies**: Basic AI with combat patterns
- **Slime Enemies**: Splitting mechanics (Large → Medium → Small)

### Combat System
- **Damage Types**: Physical, Magical, Critical hits
- **Status Effects**: 
  - Ignite: Damage over time
  - Chill: Movement speed reduction
  - Shock: Lightning strikes
- **Blocking System**: Player can counter-attack when enemies are vulnerable
- **Knockback Effects**: Directional knockback based on attack direction

### UI System
- **In-Game HUD**: Health bar, endurance bar, currency display
- **Inventory UI**: Item slots, equipment management
- **Skill Tree**: Unlockable abilities and upgrades
- **Volume Controls**: Separate SFX and BGM volume sliders
- **Crafting System**: Item creation and material management

### Save System
- **Game Data Persistence**: Player progress, inventory, unlocked skills
- **Checkpoint System**: Save points throughout the game
- **Settings Save**: Volume preferences, game settings

## 🛠️ Technical Architecture

### Core Systems
- **Entity System**: Base class for all game entities (Player, Enemies)
- **State Machine Pattern**: Used for player states and enemy AI
- **Component-Based Design**: Modular systems for stats, effects, and behaviors
- **Event System**: Decoupled communication between systems

### Key Scripts
- `Player.cs`: Main player controller with state machine
- `Enemy.cs`: Base enemy class with AI and combat
- `CharacterStats.cs`: Stats system with modifiers and status effects
- `SkillManager.cs`: Centralized skill management
- `AudioManager.cs`: Sound and music management
- `SaveManager.cs`: Game data persistence

### File Structure
```
Assets/Script/
├── Player/           # Player states and behaviors
├── Enemy/            # Enemy AI and behaviors
│   ├── NightBorn/   # Boss-specific scripts
│   ├── Skeleton/    # Skeleton enemy scripts
│   └── Slime/       # Slime enemy scripts
├── Skills/          # Skill system and abilities
├── Stats/           # Character statistics
├── Item/            # Inventory and equipment
├── UI/              # User interface components
├── Managers/        # Game management systems
└── Save_And_load/   # Save system
```

## 🎯 Gameplay Mechanics

### Combat Flow
1. **Detection**: Enemies detect player and enter battle state
2. **Engagement**: Combat with attack patterns and player responses
3. **Status Effects**: Elemental damage and debuffs
4. **Special Abilities**: Boss phases and player skills
5. **Resolution**: Victory/defeat with rewards

### Boss Fight (NightBorn)
- **Phase 1**: Basic combat with movement and attacks
- **Phase 2**: Gather state for healing (interruptible)
- **Phase 3**: Wave attacks with elemental projectiles
- **Phase 4**: Magic circle attacks
- **Special**: Teleportation behind player

## 🎵 Audio System
- **SFX**: Combat sounds, UI feedback, environmental audio
- **BGM**: Dynamic music system with different tracks for areas/bosses
- **Volume Control**: Separate sliders for SFX and BGM with AudioMixer integration

## 💾 Save System
- **Player Data**: Stats, inventory, equipment, currency
- **Progress**: Unlocked skills, completed areas, chest states
- **Settings**: Volume levels, game preferences
- **Checkpoints**: Save points for progression

## 🔧 Setup Instructions

1. **Unity Version**: Compatible with Unity 2022.3 LTS or newer
2. **Dependencies**: 
   - TextMeshPro
   - Cinemachine (for camera effects)
   - Input System (for player controls)
3. **Audio Setup**: Configure AudioMixer groups for SFX and BGM volume control
4. **Layer Setup**: Configure Physics2D collision matrix for proper interactions

## 🎮 Controls
- **Movement**: WASD or Arrow Keys
- **Jump**: Space
- **Attack**: Left Mouse Button
- **Dash**: Q
- **Skills**: C (Crystal), X (Assassinate), H (Flask)
- **Inventory**: Tab or I

## 📝 Development Notes

### State Machine Pattern
The game uses a state machine pattern extensively for both player and enemy behaviors, providing clean separation of concerns and easy extensibility.

### Elemental System
The elemental damage system supports three types: Fire (Ignite), Ice (Chill), and Lightning (Shock), each with unique visual and gameplay effects.

### Boss Design
The NightBorn boss demonstrates advanced AI with multiple phases, interruptible abilities, and dynamic behavior based on player actions.

## 🤝 Contributing
This project demonstrates various Unity game development patterns and systems. Feel free to use as reference or extend with additional features.

## 📄 License
This project is for educational and demonstration purposes.
