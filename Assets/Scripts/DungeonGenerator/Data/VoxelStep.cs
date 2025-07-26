using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator.Data
{
    public class VoxelStep
    {
        public HashSet<Vector3> oldStepVoxelsPos = new HashSet<Vector3>();
        public HashSet<Vector3> newStepVoxelsPos = new HashSet<Vector3>();

        public void ClearStepVoxels()
        {
            oldStepVoxelsPos.Clear();
            newStepVoxelsPos.Clear();
        }
    }
}
