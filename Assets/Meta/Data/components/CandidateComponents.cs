using System.Collections.Generic;

public class CandidateComponents
{
    private CandidatesManager candidatesManager;

    public CandidateComponents(CandidatesManager candidatesManager)
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
