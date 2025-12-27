# DOTS Project AI Coding Guidelines

## Architecture Overview
This is a Unity DOTS (Data-Oriented Technology Stack) project using ECS (Entity Component System) for high-performance game logic. Key components:
- **Entities**: Game objects represented as entities with components
- **Components**: Pure data structs (e.g., `MoveSpeed` in `Assets/Scripts/Authoring/MoveSpeedAuthoring.cs`)
- **Systems**: Logic that operates on components (e.g., `UnitMoverSystem` in `Assets/Scripts/Systems/UnitMoverSystem.cs`)
- **Authoring**: MonoBehaviours for editor setup, converted via Bakers

Data flows from authoring (editor-time) to runtime components via baking process.

## Key Patterns
- **System Structure**: Use `partial struct` implementing `ISystem` with `[BurstCompile]` for performance
- **Queries**: Use `SystemAPI.Query<RefRW<T>, RefRO<U>>()` for component iteration
- **Authoring Pattern**: `MonoBehaviour` + `IComponentData` + `Baker<T>` class (see `MoveSpeedAuthoring.cs`)
- **Component Access**: `RefRW` for read-write, `RefRO` for read-only in queries

## Conventions
- Scripts organized in `Assets/Scripts/Authoring/` and `Assets/Scripts/Systems/`
- Component structs use PascalCase (e.g., `MoveSpeed`)
- Systems named with "System" suffix (e.g., `UnitMoverSystem`)
- Use `Unity.Mathematics` types (float3, etc.) for vectors

## Dependencies
- Unity Entities 1.4.3 (ECS core)
- Entities.Graphics, Physics, InputSystem for full DOTS stack
- Burst compiler for performance

## Workflows
- Build via Unity Editor (File > Build Settings)
- Baking happens automatically on scene load/play
- Debug ECS via Entity Debugger window (Window > DOTS > Entity Debugger)

## Examples
- Adding movement: Create component like `MoveSpeed`, query in system with `localTransform.ValueRW.Position += direction * speed * deltaTime`
- New systems: Implement `OnUpdate` with queries, add `[BurstCompile]` for jobs</content>
<parameter name="filePath">d:\UnityProjects\DOTS Project\.github\copilot-instructions.md