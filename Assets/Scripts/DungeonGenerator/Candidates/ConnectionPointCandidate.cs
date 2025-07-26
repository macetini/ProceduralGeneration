using Assets.Meta.Sets;
using Assets.Scripts.DungeonGenerator.Data;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator.Candidates
{
    public class ConnectionPointCandidate
    {
        private string name;
        private bool open;
        private Voxel voxelOwner;
        private Vector3 localPosition;
        private Vector3 worldPosition;
        private Quaternion rotation;
        private Candidate owner;

        public string Name { get => name; set => name = value; }
        public bool Open { get => open; set => open = value; }
        public Voxel VoxelOwner { get => voxelOwner; set => voxelOwner = value; }
        public Vector3 LocalPosition { get => localPosition; set => localPosition = value; }
        public Vector3 WorldPosition { get => worldPosition; set => worldPosition = value; }
        public Quaternion Rotation { get => rotation; set => rotation = value; }
        public Candidate Owner { get => owner; set => owner = value; }
        public DungeonSet OwnerSet { get => owner.Set; }
    }
}