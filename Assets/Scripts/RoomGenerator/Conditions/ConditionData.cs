using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ConditionData
{
    public RoomBlueprint blueprint;
    public RoomElement roomItemPrefab;
    public Vector3 randomFloorVoxelPos;
    public HashSet<Vector3> takenVoxels;
    public DirectionType endPointDirection;
}
