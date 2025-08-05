using Assets.Scripts.Generators.Dungeon.Candidates;
using Assets.Scripts.Generators.Meta.VoxelData;
using UnityEngine;

namespace Assets.Scripts.Generators.Dungeon.Points
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