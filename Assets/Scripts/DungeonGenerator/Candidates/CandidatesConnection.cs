using UnityEngine;

namespace Assets.Scripts.DungeonGenerator.Candidates
{
    public class CandidatesConnection
    {
        //private Vector3 lastCandidateWorldPos;
        //private ConnectionPointCandidate newConnPointCandidate;
        //private ConnectionPointCandidate lastConnPointCandidate;
        //private VoxelStep newCandidateStepVoxel;
        public Vector3 LastCandidateWorldPos { get; set; }
        public ConnectionPointCandidate LastConnPointCandidate { get; set; }
        public ConnectionPointCandidate NewConnPointCandidate { get; set; }
        //public VoxelStep NewCandidateStepVoxel { get => newCandidateStepVoxel; set => newCandidateStepVoxel = value; }
    }
}
