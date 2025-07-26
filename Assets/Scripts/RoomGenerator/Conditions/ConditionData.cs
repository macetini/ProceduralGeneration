using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator.Conditions
{
    public struct ConditionData
    {
        public RoomBlueprint blueprint;
        public RoomElement roomItemPrefab;
        public Vector3 randomFloorVoxelPos;
        public HashSet<Vector3> takenVoxels;
        public DirectionType endPointDirection;
    }
}