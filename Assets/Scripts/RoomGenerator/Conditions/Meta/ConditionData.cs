using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator.Conditions.Meta
{
    public enum ConditionType
    {
        must = 0,
        should = 1,
        mustNot = 2,
        shouldNot = 3
    }
    
    public struct ConditionData
    {
        public RoomBlueprint blueprint;
        public RoomElement roomItemPrefab;
        public Vector3 randomFloorVoxelPos;
        public HashSet<Vector3> takenVoxels;
        public DirectionType endPointDirection;
    }
}