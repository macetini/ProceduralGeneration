using UnityEngine;

public class CandidatesConnection
{
    private Vector3 lastCandidateWorldPos;
    private ConnPointCandidate newConnPointCandidate;
    private ConnPointCandidate lastConnPointCandidate;
    //private VoxelStep newCandidateStepVoxel;

    public Vector3 LastCandidateWorldPos { get { return lastCandidateWorldPos; } set { lastCandidateWorldPos = value; } }    
    public ConnPointCandidate LastConnPointCandidate { get { return lastConnPointCandidate; } set { lastConnPointCandidate = value; } }    
    public ConnPointCandidate NewConnPointCandidate { get { return newConnPointCandidate; } set { newConnPointCandidate = value; } }
    //public VoxelStep NewCandidateStepVoxel { get => newCandidateStepVoxel; set => newCandidateStepVoxel = value; }
}
