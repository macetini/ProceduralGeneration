using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator.VoxelData
{
    public class VoxelStep
    {
        public HashSet<Vector3> OldStepVoxelsPos { get; set; } = new();
        public HashSet<Vector3> NewStepVoxelsPos { get; set; } = new();

        public void ClearStepVoxels()
        {
            OldStepVoxelsPos.Clear();
            NewStepVoxelsPos.Clear();
        }
    }
}
