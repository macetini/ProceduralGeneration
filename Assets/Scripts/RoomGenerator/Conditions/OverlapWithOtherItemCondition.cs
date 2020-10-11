using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapWithOtherItemCondition : GenerationCondition
{
    public override bool Test(ConditionData data)
    {
        EndPoint endPoint = owner.endPoint;

        Vector3 pointDir = endPoint.transform.forward;

        HashSet<Vector3> takenVoxelsMap = data.takenVoxels;

        Quaternion rotation = Quaternion.AngleAxis((float)data.endPointDirection, Vector3.up);

        float edgeLength = owner.Volume.generatorSize.z;
        for (float i = 0; i < edgeLength; i++)
        {
            Vector3 edgeOffset = rotation * new Vector3(0f, 0f, i);

            Vector3 wallVoxelPosition = data.randomFloorVoxelPos + edgeOffset;

            if (takenVoxelsMap.Contains(wallVoxelPosition.RoundVec3ToInt()))
            {
                return false;
            }
        }

        return true;
    }
}
