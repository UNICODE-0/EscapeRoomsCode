using EscapeRooms.Components;
using EscapeRooms.Events;
using EscapeRooms.Helpers;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Collections;
using Scellecs.Morpeh.Providers;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace EscapeRooms.Systems
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class HingeRotationSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<HingeRotatableComponent> _rotatableStash;
        private Stash<HingeRotationComponent> _rotationStash;
        private Stash<ConfigurableJointComponent> _jointStash;
        private Stash<TransformComponent> _transformStash;
        private Stash<OnHingeRotationFlag> _onRotationStash;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<HingeRotatableComponent>()
                .With<TransformComponent>()
                .With<OnHingeRotationFlag>()
                .Build();

            _rotatableStash = World.GetStash<HingeRotatableComponent>();
            _jointStash = World.GetStash<ConfigurableJointComponent>();
            _rotationStash = World.GetStash<HingeRotationComponent>();
            _onRotationStash = World.GetStash<OnHingeRotationFlag>();
            _transformStash = World.GetStash<TransformComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var rotatableComponent = ref _rotatableStash.Get(entity);
                ref var jointComponent = ref _jointStash.Get(entity);
                
                ref var onRotationComponent = ref _onRotationStash.Get(entity);
                ref var transformComponent = ref _transformStash.Get(entity);
                ref var rotationComponent = ref _rotationStash.Get(onRotationComponent.Owner);

                float input =
                    -Mathf.Clamp(rotationComponent.RotateDeltaInput, rotationComponent.DeltaRange.x,
                        rotationComponent.DeltaRange.y) * rotationComponent.RotationSpeed;
                
                Quaternion rotationX = Quaternion.AngleAxis(input, transformComponent.Transform.right);
                
                Quaternion result = jointComponent.ConfigurableJoint.targetRotation * rotationX;

                float left = result.GetXAxisAngleInQuarter(rotatableComponent.LeftBorder.AngleQuarter);
                float right = result.GetXAxisAngleInQuarter(rotatableComponent.RightBorder.AngleQuarter);

#if UNITY_EDITOR
                if(rotatableComponent.ShowQuarterAndAngle)
                {
                    float Q1 = result.GetXAxisAngleInQuarter(QuaternionQuarter.First);
                    float Q2 = result.GetXAxisAngleInQuarter(QuaternionQuarter.Second);
                    float Q3 = result.GetXAxisAngleInQuarter(QuaternionQuarter.Third);
                    float Q4 = result.GetXAxisAngleInQuarter(QuaternionQuarter.Fourth);

                    if (Q1 >= 0)
                        Debug.Log("Quarter 1: " + Q1);
                    else if (Q2 >= 0)
                        Debug.Log("Quarter 2: " + Q2);
                    else if (Q3 >= 0)
                        Debug.Log("Quarter 3: " + Q3);
                    else if (Q4 >= 0)
                        Debug.Log("Quarter 4: " + Q4);
                }
#endif
            
                if (CheckBorder(left, rotatableComponent.LeftBorder)) return;
                if (CheckBorder(right, rotatableComponent.RightBorder)) return;
                
                jointComponent.ConfigurableJoint.targetRotation = result;
            }
        }
        
        private bool CheckBorder(float value, HingeRotatableComponent.HingeRotatableBorder border)
        {
            switch (border.Mode)
            {
                case HingeRotatableComponent.BorderCheckMode.Max:
                    return value >= border.Angle;

                case HingeRotatableComponent.BorderCheckMode.Min:
                    return value >= 0 && value <= border.Angle;

                default:
                    return false;
            }
        }
        
        public void Dispose()
        {
        }
    }
}