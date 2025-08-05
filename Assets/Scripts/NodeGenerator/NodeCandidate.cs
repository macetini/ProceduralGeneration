using UnityEngine;

namespace Assets.Scripts.NodeGenerator
{
    //TODO - Create a factory with a candidates.
    struct NodeCandidate
    {
        public Node prefab;
        public Vector3 randomFloorVoxelPos;
        public int rotationIndex;
    }
}
