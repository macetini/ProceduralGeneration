using UnityEngine;

namespace Assets.Scripts.RoomGenerator
{
    struct RoomElementCandidate
    {
        public RoomElement prefab;
        public Vector3 randomFloorVoxelPos;
        public int rotationIndex;
    }
}
