using UnityEngine;

namespace Assets.Scripts.Generators.Node.Candidates
{
    //TODO - Create factory for candidates.
    struct NodeCandidate
    {
        public Node prefab;
        public Vector3 randomFloorVoxelPos;
        public int rotationIndex;
    }
}
