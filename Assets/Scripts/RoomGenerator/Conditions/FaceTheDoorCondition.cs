using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTheDoorCondition : GenerationCondition
{
    public override bool Test(ConditionData data)
    {
        RoomBlueprint blueprint = data.blueprint;
        HashSet<Vector3> doorsVoxelMap = blueprint.DoorsVoxelMap;

        Vector3 randomFloorVoxelPos = data.randomFloorVoxelPos;

        Quaternion edgeRotation = EndPoint.GetRoation(data.endPointDirection);

        float itemLength = owner.Volume.generatorSize.x;
        Vector3 horizontalEdgeOffset = EndPoint.GetRoation(data.endPointDirection) * new Vector3(itemLength, 0f, 0f);

        float itemHeight = owner.Volume.generatorSize.z;
        for (float i = 0; i < itemHeight; i++)
        {
            Vector3 verticalEdgeOffset = EndPoint.GetRoation(data.endPointDirection) * new Vector3(0f, 0f, i);

            Vector3 doorVoxelPosition = randomFloorVoxelPos + horizontalEdgeOffset + verticalEdgeOffset;
            doorVoxelPosition = doorVoxelPosition.RoundVec3ToInt();

            if (doorsVoxelMap.Contains(doorVoxelPosition))
            {
                GameObject doorVoxelGO = data.blueprint.GetDoorVoxelGO(doorVoxelPosition);

                Vector3 directionsDifference = horizontalEdgeOffset + doorVoxelGO.transform.right;

                if (directionsDifference == Vector3.zero)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
