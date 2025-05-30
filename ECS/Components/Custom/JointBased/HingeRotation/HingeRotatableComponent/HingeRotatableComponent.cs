using EscapeRooms.Data;
using EscapeRooms.Helpers;
using Scellecs.Morpeh;
using Sirenix.OdinInspector;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Serialization;

namespace EscapeRooms.Components
{
    [System.Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct HingeRotatableComponent : IComponent
    {
        [MinValue(0.01f)] 
        public float Spring;
            
        [MinValue(0f)]
        public float Damper;

        public HingeRotatableBorder LeftBorder;
        public HingeRotatableBorder RightBorder;
        
        [MinValue(0.001f)]
        public float MassWhileRotate;
        
        [FoldoutGroup(Consts.COMPONENT_RUNTIME_FOLDOUT_NAME)] 
        [ReadOnly] public float MassBeforeRotate;
        
#if UNITY_EDITOR
        [FoldoutGroup(Consts.DEBUG_FOLDOUT_NAME)]
        public bool ShowQuarterAndAngle;
#endif
        
        [System.Serializable]
        public struct HingeRotatableBorder
        {
            public QuaternionQuarter AngleQuarter;
        
            [MinValue(0f), MaxValue(90f)]
            public float Angle;

            public BorderCheckMode Mode;

        }
        
        [System.Serializable]
        public enum BorderCheckMode
        {
            Max,
            Min
        }
    }
}