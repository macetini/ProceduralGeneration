using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.Utils;
using Assets.Scripts.RoomGenerator.Conditions.Meta;
using Assets.Scripts.RoomGenerator.Points;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator.Conditions.Derived
{
    public class ConnectWithDoorCondition : GenerationCondition
    {
        public override bool Test(ConditionData data)
        {
            RoomBlueprint blueprint = data.blueprint;
            HashSet<Vector3> doorsVoxelMap = blueprint.DoorsVoxelMap;

            Vector3 randomFloorVoxelPos = data.randomFloorVoxelPosition;            

            float itemLength = owner.Volume.generatorSize.x;
            Vector3 rightHorizontalEdgeOffset = EndPoint.GetRotation(data.endPointRotation) * new Vector3(itemLength, 0f, 0f);
            Vector3 leftHorizontalEdgeOffset = rightHorizontalEdgeOffset * -1f;

            float itemHeight = owner.Volume.generatorSize.z;
            for (float i = 0; i < itemHeight; i++)
            {
                Vector3 verticalEdgeOffset = EndPoint.GetRotation(data.endPointRotation) * new Vector3(0f, 0f, i);

                Vector3 rightEdgeVoxelPosition = randomFloorVoxelPos + rightHorizontalEdgeOffset + verticalEdgeOffset;
                rightEdgeVoxelPosition = rightEdgeVoxelPosition.RoundVec3ToInt();

                Vector3 leftEdgeVoxelPosition = randomFloorVoxelPos + leftHorizontalEdgeOffset + verticalEdgeOffset;
                leftEdgeVoxelPosition = leftEdgeVoxelPosition.RoundVec3ToInt();

                if (doorsVoxelMap.Contains(rightEdgeVoxelPosition) || doorsVoxelMap.Contains(leftEdgeVoxelPosition))
                {
                    return false;
                }
            }

            return true;
        }
    }
}