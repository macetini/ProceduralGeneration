﻿using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.Utils;
using Assets.Scripts.RoomGenerator.Conditions.Meta;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator.Conditions
{
    public class ConnectWithWallCondition : GenerationCondition
    {
        public override bool Test(ConditionData data)
        {
            EndPoint endPoint = owner.endPoint;

            Vector3 rightEndPointDir = endPoint.transform.right.RoundVec3ToInt();
            bool overlap = AllPointsNextToWall(data, rightEndPointDir);
            if (overlap) return overlap;

            Vector3 leftEndPointDir = (endPoint.transform.right * -1f).RoundVec3ToInt();
            overlap |= AllPointsNextToWall(data, leftEndPointDir);
            if (overlap) return overlap;

            return overlap;
        }

        protected bool AllPointsNextToWall(ConditionData data, Vector3 pointDir)
        {
            HashSet<Vector3> wallsVoxelMap = data.blueprint.WallsVoxelMap;

            Quaternion rotation = Quaternion.AngleAxis((float)data.endPointDirection, Vector3.up);

            float edgeLength = owner.Volume.generatorSize.z;
            for (float i = 0; i < edgeLength; i++)
            {
                Vector3 edgeOffset = rotation * new Vector3(0f, 0f, i);
                Vector3 rotatedPointDir = rotation * pointDir;

                Vector3 wallVoxelPosition = data.randomFloorVoxelPos + rotatedPointDir + edgeOffset;
                wallVoxelPosition = wallVoxelPosition.RoundVec3ToInt();

                if (!wallsVoxelMap.Contains(wallVoxelPosition))
                {
                    return false;
                }
            }

            return true;
        }
    }
}