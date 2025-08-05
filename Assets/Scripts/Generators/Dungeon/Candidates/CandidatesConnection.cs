using UnityEngine;

namespace Assets.Scripts.DungeonGenerator.Candidates
{
    public class CandidatesConnection
    {
        public Vector3 LastCandidateWorldPos { get; set; }
        public ConnectionPointCandidate LastConnPointCandidate { get; set; }
        public ConnectionPointCandidate NewConnPointCandidate { get; set; }        
    }
}
