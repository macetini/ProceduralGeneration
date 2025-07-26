using Assets.Scripts.DungeonGenerator.Candidates;
using Assets.Scripts.DungeonGenerator.Data;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator.Elements
{
    public class ConnectionPoint : MonoBehaviour
    {
        public bool Open { get; set; } = true;
        public Voxel voxelOwner;
        public ConnectionPoint sharedConnPoint;

        public ConnectionPointCandidate CloneCandidate
        {
            get
            {
                ConnectionPointCandidate clone = new()
                {
                    Open = Open,
                    VoxelOwner = voxelOwner,

                    Name = name,
                    LocalPosition = transform.position,
                    Rotation = transform.rotation
                };

                return clone;
            }
        }        
    }
}