using UnityEngine;

namespace Assets.Scripts.Generators.Dungeon.Candidates
{
    public class CandidatesConnection
    {
        public Vector3 LastCandidateWorldPos { get; set; }
        public ConnectionPointCandidate LastConnPointCandidate { get; set; }
        public ConnectionPointCandidate NewConnPointCandidate { get; set; }        
    }
}
