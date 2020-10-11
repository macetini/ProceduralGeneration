using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Condition tests if item is facing the wall. Are voxels along item's front edge directly facing any wall voxels. 
/// </summary>
public class FaceTheWallCondition : GenerationCondition
{
    public override bool Test(ConditionData data)
    {
        RoomBlueprint blueprint = data.blueprint;
        HashSet<Vector3> wallsVoxelMap = blueprint.WallsVoxelMap;

        Vector3 randomFloorVoxelPos = data.randomFloorVoxelPos;

        Quaternion edgeRotation = EndPoint.GetRoation(data.endPointDirection);

        float itemLength = owner.Volume.generatorSize.x;
        Vector3 horizontalEdgeOffset = EndPoint.GetRoation(data.endPointDirection) * new Vector3(itemLength, 0f, 0f);

        float itemHeight = owner.Volume.generatorSize.z;
        for (float i = 0; i < itemHeight; i++)
        {
            Vector3 verticalEdgeOffset = EndPoint.GetRoation(data.endPointDirection) * new Vector3(0f, 0f, i);

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
