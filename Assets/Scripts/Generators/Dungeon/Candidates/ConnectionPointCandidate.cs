using Assets.Meta.Sets;
using Assets.Scripts.Generators.Meta.VoxelData;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator.Candidates
{
    public class ConnectionPointCandidate
    {
        public string Name { get; set; }
        public bool Open { get; set; }
        public Voxel VoxelOwner { get; set; }
        public Vector3 LocalPosition { get; set; }
        public Vector3 WorldPosition { get; set; }
        public Quaternion Rotation { get; set; }
        public Candidate Owner { get; set; }
        public DungeonSet OwnerSet { get => Owner.Set; }
    }
}