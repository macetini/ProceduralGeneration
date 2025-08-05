using System.Collections.Generic;
using Assets.Scripts.Generators.Dungeon.Candidates;

namespace Assets.Meta.Data.Products
{
    public class CandidateProduct
    {
        private readonly CandidatesManager candidatesManager;

        public CandidateProduct(CandidatesManager candidatesManager)
        {
            this.candidatesManager = candidatesManager;
        }

        public List<Candidate> GetAllAcceptedCandidates()
        {
            return candidatesManager.AcceptedCandidates;
        }

        public Candidate GetFirstAcceptedCandidate()
        {
            return candidatesManager.GetFirstAcceptedCandidate();
        }

        public Candidate GetLastAcceptedCandidate()
        {
            return candidatesManager.GetLastAcceptedCandidate();
        }

        public void GenerateNextCandidate()
        {
            candidatesManager.GenerateNextCandidate();
        }

        public int GetOpenCandidatesCount()
        {
            return candidatesManager.OpenCandidates.Count;
        }

        public int GetAcceptedCandidatesCount()
        {
            return candidatesManager.AcceptedCandidates.Count;
        }
    }
}