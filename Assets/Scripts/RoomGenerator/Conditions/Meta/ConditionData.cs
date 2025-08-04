using System.Collections.Generic;
using Assets.Scripts.RoomGenerator.Points.Meta;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator.Conditions.Meta
{
    public struct ConditionData
    {
        public RoomBlueprint blueprint;
        public RoomElement roomItemPrefab;
        public Vector3 randomFloorVoxelPosition;
        public HashSet<Vector3> takenVoxels;
        public RotationData endPointRotation;
    }
}