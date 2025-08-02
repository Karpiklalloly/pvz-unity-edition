# Unity Development Rules

## File Creation Rules

### DO NOT create .meta files
- Unity automatically generates .meta files for all assets
- Never manually create .meta files
- Only create the actual asset files (.cs, .mat, .prefab, etc.)

### Asset Creation Process
1. Create only the main asset file (e.g., Script.cs, Material.mat)
2. Let Unity generate the corresponding .meta file automatically
3. Unity will assign GUIDs and handle asset references

### Folder Structure
- Create folder .meta files only if absolutely necessary
- Usually Unity handles folder meta files automatically

## Bootstrap Architecture Rules

### Single Bootstrap Pattern
- **Use only ONE main bootstrap** for the entire game
- All systems should be initialized in GameBootstrap
- Avoid multiple bootstrap classes (ECSGridBootstrap, etc.)
- Centralized initialization and management

### Bootstrap Structure
- GameBootstrap manages all ECS systems
- Clear separation of concerns within single bootstrap
- Modular system registration
- Single point of control for the entire game

## Examples

✅ **Correct:**
- Create: `MyScript.cs`
- Unity creates: `MyScript.cs.meta` (automatically)
- Use: `GameBootstrap` for all system initialization

❌ **Incorrect:**
- Create: `MyScript.cs`
- Create: `MyScript.cs.meta` (manually) ← DON'T DO THIS
- Create: Multiple bootstrap classes ← DON'T DO THIS

## Exceptions
- Only create .meta files if specifically requested by the user
- Or if working with version control and meta files are missing

## ECS Architecture Rules

### Single Bootstrap Pattern
- Use only ONE main bootstrap class: `GameBootstrap`
- All ECS systems should be initialized in GameBootstrap
- No multiple bootstrap classes (ECSGridBootstrap, etc.)
- GameBootstrap manages the entire game's ECS world and pipeline

### System Organization
- All systems register through GameBootstrap
- GameBootstrap handles world creation and pipeline building
- Individual managers (like GridTestManager) should only provide UI/testing interface
- Keep ECS logic centralized in one place

## ECS Architecture Rules

### Single Bootstrap Pattern
- Use only ONE main Bootstrap class for ECS initialization
- All ECS systems should be initialized in one place
- Avoid multiple Bootstrap classes (GridTestManager, ECSGridBootstrap, etc.)
- Main Bootstrap should handle all game systems, not just specific features

### Bootstrap Naming
- Main Bootstrap: `GameBootstrap` or `ECSBootstrap`
- Feature-specific managers should be avoided
- Centralized initialization and management