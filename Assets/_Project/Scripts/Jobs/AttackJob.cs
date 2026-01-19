using System.Collections;
using System.Collections.Generic;
using AndrzejKebab.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AndrzejKebab
{
    [BurstCompile]
    public partial struct AttackJob : IJobEntity
    {
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        public ComponentLookup<HealthComponent> HealthLookup;

        public EntityCommandBuffer Ecb;

        private void Execute(ref AttackComponent attack, in TargetComponent targetComp, in LocalTransform myTransform)
        {
            attack.TimeSinceLastAttack += DeltaTime;
            Entity targetEntity = targetComp.TargetEntity;

            if (targetEntity == Entity.Null) return;
            if (!HealthLookup.HasComponent(targetEntity)) return;
            if (!TransformLookup.HasComponent(targetEntity)) return;

            float3 targetPos = TransformLookup[targetEntity].Position;
            var distSq = math.distancesq(myTransform.Position, targetPos);
            var rangeSq = attack.AttackRange * attack.AttackRange;
            var delay = 1.0f / attack.AttackSpeed;

            if (!(distSq <= rangeSq) || !(attack.TimeSinceLastAttack >= delay)) return;
            
            attack.TimeSinceLastAttack = 0f;
            HealthComponent healthData = HealthLookup[targetEntity];
            healthData.CurrentHealth -= attack.AttackDamage;
            HealthLookup[targetEntity] = healthData;

            if (healthData.CurrentHealth <= 0)
            {
                Ecb.DestroyEntity(targetEntity);
            }
        }
    }
}