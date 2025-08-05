using System.Collections.Generic;
using Assets.Scripts.Generators.Node.Points.Meta;
using UnityEngine;

namespace Assets.Scripts.Generators.Node.Conditions.Meta
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