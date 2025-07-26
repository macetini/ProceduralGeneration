using UnityEngine;

namespace Assets.Scripts.DungeonGenerator.Candidates
{
    public class CandidatesConnection
    {
        private Vector3 lastCandidateWorldPos;
        private ConnectionPointCandidate newConnPointCandidate;
        private ConnectionPointCandidate lastConnPointCandidate;
        //private VoxelStep newCandidateStepVoxel;

        public Vector3 LastCandidateWorldPos { get { return lastCandidateWorldPos; } set { lastCandidateWorldPos = value; } }
        public ConnectionPointCandidate LastConnPointCandidate { get { return lastConnPointCandidate; } set { lastConnPointCandidate = value; } }
        public ConnectionPointCandidate NewConnPointCandidate { get { return newConnPointCandidate; } set { newConnPointCandidate = value; } }
        //public VoxelStep NewCandidateStepVoxel { get => newCandidateStepVoxel; set => newCandidateStepVoxel = value; }
    }
}
