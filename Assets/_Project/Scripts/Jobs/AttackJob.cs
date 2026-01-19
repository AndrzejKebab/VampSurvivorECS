using AndrzejKebab.Components;
using AndrzejKebab.Components.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace AndrzejKebab.Jobs
{
    [BurstCompile]
    public partial struct AttackJob : IJobEntity
    {
        [ReadOnly] public float                            DeltaTime;
        [ReadOnly] public ComponentLookup<LocalTransform>  TransformLookup;
        [ReadOnly] public ComponentLookup<PlayerTag>       PlayerTagLookup;
        [ReadOnly] public ComponentLookup<IsDeadTag>       IsDeadLookup;
        public            ComponentLookup<HealthComponent> HealthLookup;
        public EntityCommandBuffer Ecb;

        private void Execute(Entity entity, ref AttackComponent attack, in TargetComponent targetComp, in LocalTransform myTransform)
        {
            if (IsDeadLookup.HasComponent(entity)) return;
            Entity targetEntity = targetComp.TargetEntity;

            if (targetEntity == Entity.Null) return;
            if (!HealthLookup.HasComponent(targetEntity)) return;
            if (!TransformLookup.HasComponent(targetEntity)) return;
            if (IsDeadLookup.HasComponent(targetEntity)) return;
            
            float3 targetPos = TransformLookup[targetEntity].Position;
            var distSq = math.distancesq(myTransform.Position, targetPos);
            var rangeSq = attack.AttackRange * attack.AttackRange;
            var delay = 1.0f / attack.AttackSpeed;
            attack.TimeSinceLastAttack += DeltaTime;

            if (!(distSq <= rangeSq) || !(attack.TimeSinceLastAttack >= delay)) return;
            
            attack.TimeSinceLastAttack = 0f;
            HealthComponent healthData = HealthLookup[targetEntity];
            healthData.CurrentHealth -= attack.AttackDamage;
            HealthLookup[targetEntity] = healthData;

            if (healthData.CurrentHealth > 0) return;
            if (PlayerTagLookup.HasComponent(targetEntity))
            {
                Ecb.AddComponent<IsDeadTag>(targetEntity);
                Ecb.RemoveComponent<PhysicsCollider>(targetEntity);
            }
            else
            {
                Ecb.DestroyEntity(targetEntity);
            }
        }
    }
}