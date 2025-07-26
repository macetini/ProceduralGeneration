using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.Utils;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator.Conditions
{
    public class FaceTheWallCondition : GenerationCondition
    {
        public override bool Test(ConditionData data)
        {
            RoomBlueprint blueprint = data.blueprint;
            HashSet<Vector3> wallsVoxelMap = blueprint.WallsVoxelMap;

            Vector3 randomFloorVoxelPos = data.randomFloorVoxelPos;
            //Quaternion edgeRotation = EndPoint.GetRotation(data.endPointDirection);

            float itemLength = owner.Volume.generatorSize.x;
            Vector3 horizontalEdgeOffset = EndPoint.GetRotation(data.endPointDirection) * new Vector3(itemLength, 0f, 0f);

            float itemHeight = owner.Volume.generatorSize.z;
            for (float i = 0; i < itemHeight; i++)
            {
                Vector3 verticalEdgeOffset = EndPoint.GetRotation(data.endPointDirection) * new Vector3(0f, 0f, i);

                Vector3 wallVoxelPosition = randomFloorVoxelPos + horizontalEdgeOffset + verticalEdgeOffset;
                wallVoxelPosition = wallVoxelPosition.RoundVec3ToInt();

                if (wallsVoxelMap.Contains(wallVoxelPosition))
                {
                    GameObject wallVoxelGO = data.blueprint.GetWallVoxelGO(wallVoxelPosition);

                    Vector3 directionsDifference = horizontalEdgeOffset + wallVoxelGO.transform.right;

                    if (directionsDifference == Vector3.zero)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}