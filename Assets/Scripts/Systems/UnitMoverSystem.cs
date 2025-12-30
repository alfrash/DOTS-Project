using Unity.Burst; // Enable Burst compilation for performance
using Unity.Entities; // ECS types and SystemAPI
using Unity.Mathematics; // math types and functions (float3, math.*)
using Unity.Physics; // PhysicsVelocity component
using Unity.Transforms; // LocalTransform component

partial struct UnitMoverSystem : ISystem { // ECS system that updates unit movement

    [BurstCompile] // Compile OnUpdate with Burst
    public void OnUpdate(ref SystemState state) { // Called every frame to schedule work
        UnitMoverJob unitMoverJob = new UnitMoverJob {
            deltaTime = SystemAPI.Time.DeltaTime // Pass frame delta time into the job
        };
        unitMoverJob.ScheduleParallel(); // Schedule the job to run in parallel across entities

        /*
        // Legacy immediate-mode query (kept for reference). This iterates on main thread:
        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRO<UnitMover> unitMover,
            RefRW<PhysicsVelocity> physicsVelocity)
            in SystemAPI.Query<
            RefRW<LocalTransform>,
            RefRO<UnitMover>,
            RefRW<PhysicsVelocity>
            >()) {

            // Compute direction from current position to target and normalize
            float3 moveDirection = unitMover.ValueRO.targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            // Smoothly rotate transform to face movement direction
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation,
                quaternion.LookRotationSafe(moveDirection, math.up()),
                SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);

            // Apply linear movement and stop angular motion
            physicsVelocity.ValueRW.Linear = moveDirection * unitMover.ValueRO.moveSpeed;
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
        */
    }

    
}

[BurstCompile] // Compile the job with Burst for performance
public partial struct UnitMoverJob : IJobEntity {
    public float deltaTime; // Frame delta time injected from the system

    private void Execute(
        ref LocalTransform localTransform, // Read/write transform for rotation
        in UnitMover unitMover, // Read-only movement settings and target
        ref PhysicsVelocity physicsVelocity // Read/write physics velocity to move the entity
        ) {
        // Vector from current position to the target position
        float3 moveDirection = unitMover.targetPosition - localTransform.Position;

        // Threshold under which the unit is considered to have reached its target
        float reachedTargetPosition = 2f;
        if (math.length(moveDirection) < reachedTargetPosition) {
            // Stop motion when within threshold
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return; // Early exit — no further updates required
        }

        // Normalize to get movement direction only (unit length)
        moveDirection = math.normalize(moveDirection);

        // Smoothly rotate the local transform to face the move direction
        localTransform.Rotation = math.slerp(localTransform.Rotation,
            quaternion.LookRotationSafe(moveDirection, math.up()),
            deltaTime * unitMover.rotationSpeed);

        // Set forward linear velocity and clear angular velocity
        physicsVelocity.Linear = moveDirection * unitMover.moveSpeed;
        physicsVelocity.Angular = float3.zero;
    }
}