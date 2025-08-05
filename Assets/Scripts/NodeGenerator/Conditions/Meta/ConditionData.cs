using System.Collections.Generic;
using Assets.Scripts.NodeGenerator.Points.Meta;
using UnityEngine;

namespace Assets.Scripts.NodeGenerator.Conditions.Meta
{
    public class ConditionData
    {
        public NodeBlueprint blueprint;
        public Node roomItemPrefab;
        public Vector3 randomFloorVoxelPosition;
        public HashSet<Vector3> takenVoxels;
        public RotationData endPointRotation;
    }
}