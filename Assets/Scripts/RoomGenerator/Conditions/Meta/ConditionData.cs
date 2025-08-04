using System.Collections.Generic;
using Assets.Scripts.RoomGenerator.Points.Meta;
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
    
    public class ConditionData
    {
        public RoomBlueprint blueprint;
        public RoomElement roomItemPrefab;
        public Vector3 randomFloorVoxelPosition;
        public HashSet<Vector3> takenVoxels;
        public RotationData endPointRotation;
    }
}