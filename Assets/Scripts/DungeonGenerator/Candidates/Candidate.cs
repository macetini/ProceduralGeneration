using System.Collections.Generic;
using UnityEngine;

public class Candidate
{
    //TODO - TOO MANY GLOBALS (Way, way too many - re factor)

    private readonly DungeonSet set;

    private Vector3 worldPos;
    private Quaternion rotation;
    private List<ConnPointCandidate> connPointCandidates;
    private CandidatesConnection candidatesConnection;    

    private Dictionary<Vector3, Vector3> voxWorldPos;
    private readonly GameObject gameObject;
    private readonly Element element;
    private readonly Volume volume;

    private readonly Vector3 halfStep;
    private readonly Vector3 step;
    private readonly string id;

    public Candidate(GameObject gameObject, DungeonSet set)
    {
        this.gameObject = gameObject;
        this.set = set;

        element = gameObject.GetComponent<Element>();
        volume = gameObject.GetComponent<Volume>();

        halfStep = new Vector3(volume.VoxelScale * 0.5f, 0f, 0f);
        step = new Vector3(volume.VoxelScale, 0f, 0f);
        id = element.ID;        

        CloneConnPoints();
    }

    public DungeonSet Set { get => set; }

    public Vector3 WorldPos { get => worldPos; set => worldPos = value; }
    public Quaternion Rotation { get => rotation; set => rotation = value; }
    public List<ConnPointCandidate> ConnPointCandidates { get => connPointCandidates; set => connPointCandidates = value; }
    public CandidatesConnection CandidatesConnection { get { return candidatesConnection; } set { candidatesConnection = value; } }    

    public Dictionary<Vector3, Vector3> VoxWorldPos => voxWorldPos;
    public GameObject GameObject => gameObject;
    public Element Element => element;
    public Volume Volume => volume;
    public List<GameObject> Voxels => Volume.voxels;

    public ConnPointCandidate LastConnPointCandidate => candidatesConnection.LastConnPointCandidate;
    public ConnPointCandidate NewConnPointCandidate => candidatesConnection.NewConnPointCandidate;

    public Vector3 HalfStep => halfStep;
    public Vector3 Step => step;
    public string ID => id;

    public bool HasOpenConnection()
    {
        int connPointsCount = ConnPointCandidates.Count;
        for (int i = 0; i < connPointsCount; i++)
        {
            if (ConnPointCandidates[i].Open)
            {
                return true;
            }
        }

        return false;
    }

    public ConnPointCandidate GetFirstOpenConnectionPoint()
    {
        int connPointsCount = ConnPointCandidates.Count;
        for (int i = 0; i < connPointsCount; i++)
        {
            if (ConnPointCandidates[i].Open) return ConnPointCandidates[i];
        }

        Debug.Log("Candidate::GetFirstOpenConnPoint() - No open connection points.");

        return null;
    }

    protected void CloneConnPoints()
    {
        List<ConnPoint> itemConnPoints = element.connPoints;
        int connPointsCount = itemConnPoints.Count;

        ConnPointCandidates = new List<ConnPointCandidate>(connPointsCount);

        for (int i = 0; i < connPointsCount; i++)
        {
            ConnPointCandidate cloneGO = itemConnPoints[i].CloneCandidate;
            cloneGO.Owner = this;
            ConnPointCandidates.Add(cloneGO);
        }
    }

    public void SetWorldPosAndRotation(Vector3 worldPos, Quaternion rotation)
    {
        this.worldPos = worldPos;
        this.rotation = rotation;
    }

    public ConnPointCandidate GetRandomOpenConnPoint(DRandom random)
    {
        ConnPointCandidates.Shuffle(random.random);

        int connPointsCount = ConnPointCandidates.Count;
        for (int i = 0; i < connPointsCount; i++)
        {
            if (ConnPointCandidates[i].Open)
            {
                return ConnPointCandidates[i];
            }
        }

        Debug.Log("Candidate::GetRandomOpenConnPoint() - No open connection points.");

        return null;
    }

    public List<ConnPointCandidate> GetAllOpenConnPoints(DRandom random)
    {
        ConnPointCandidates.Shuffle(random.random);

        List<ConnPointCandidate> openConnPointCandidates = new List<ConnPointCandidate>();

        int connPointsCount = ConnPointCandidates.Count;
        for (int i = 0; i < connPointsCount; i++)
        {
            ConnPointCandidate connPointCandidate = ConnPointCandidates[i];

            if (connPointCandidate.Open)
            {
                openConnPointCandidates.Add(connPointCandidate);
            }
        }

        if (openConnPointCandidates.Count == 0)
        {
            Debug.Log("Candidate::GetOpenConnPoints() - No open connection points.");
        }

        return openConnPointCandidates;
    }

    public void SetVoxelsWorldPos(Vector3 translation = default(Vector3), Quaternion rotation = default(Quaternion))
    {
        int voxelsCount = Voxels.Count;
        voxWorldPos = new Dictionary<Vector3, Vector3>(voxelsCount);

        for (int i = 0; i < voxelsCount; i++)
        {
            Vector3 localPosition = Voxels[i].transform.localPosition;
            Vector3 worldPosition = rotation * localPosition + translation;
            voxWorldPos.Add(localPosition, worldPosition);
        }
    }

    public void UpdateConnPointsWorldPos(float angleDiffernce = 0f)
    {
        int connPointsCount = ConnPointCandidates.Count;
        for (int i = 0; i < connPointsCount; i++)
        {
            ConnPointCandidate connPointCandidate = ConnPointCandidates[i];

            Quaternion newRotation = Quaternion.AngleAxis(ConnPointCandidates[i].Rotation.eulerAngles.y + angleDiffernce, Vector3.up);
            connPointCandidate.Rotation = newRotation;

            Vector3 localPosition = connPointCandidate.VoxelOwner.transform.localPosition;
            connPointCandidate.WorldPosition = GetConnPointCandidateWorldPos(localPosition, newRotation);
        }
    }

    protected Vector3 GetConnPointCandidateWorldPos(Vector3 localPosition, Quaternion rotation)
    {
        Vector3 worldPosition = GetVoxelWorldPos(localPosition);

        worldPosition += rotation * HalfStep;

        return worldPosition;
    }

    public Vector3 GetVoxelWorldPos(Vector3 position)
    {
        return voxWorldPos[position];
    }
}
