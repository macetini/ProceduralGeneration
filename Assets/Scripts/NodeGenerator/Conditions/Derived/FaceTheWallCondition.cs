using System.Collections.Generic;
using Assets.Scripts.Utils;
using Assets.Scripts.NodeGenerator.Conditions.Meta;
using Assets.Scripts.NodeGenerator.Points;
using UnityEngine;

namespace Assets.Scripts.NodeGenerator.Conditions.Derived
{
    public class FaceTheWallCondition : GenerationCondition
    {
        public override bool Test(ConditionData data)
        {
            NodeBlueprint blueprint = data.blueprint;
            HashSet<Vector3> wallsVoxelMap = blueprint.WallsVoxelMap;

            Vector3 randomFloorVoxelPosition = data.randomFloorVoxelPosition;

            float itemLength = owner.Volume.generatorSize.x;
            Vector3 horizontalEdgeOffset = EndPoint.GetRotation(data.endPointRotation) * new Vector3(itemLength, 0f, 0f);

            float itemHeight = owner.Volume.generatorSize.z;
            for (float i = 0; i < itemHeight; i++)
            {
                Vector3 verticalEdgeOffset = EndPoint.GetRotation(data.endPointRotation) * new Vector3(0f, 0f, i);

                Vector3 wallVoxelPosition = randomFloorVoxelPosition + horizontalEdgeOffset + verticalEdgeOffset;
                wallVoxelPosition = wallVoxelPosition.RoundVec3ToInt();

                if (wallsVoxelMap.Contains(wallVoxelPosition))
                {
                    return false;
                }
            }

            return true;
        }
    }
}