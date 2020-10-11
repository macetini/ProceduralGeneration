using System.Collections.Generic;
using UnityEngine;

//RENAME TO CANDIDATES FACTORY
[CreateAssetMenu(fileName = "ElementsFactory", menuName = "Elements Factory", order = 1)]
public class ElementsFactory : ScriptableObject
{
    public List<DungeonSet> Sets = new List<DungeonSet>();

    public int loopCheckCount = 5;
    public bool randomizeOnStart = true;

    public bool recycle = true;

    [Range(0f, 1.0f)]
    public float closingElemsPct = 0.7f;

    //A lot of globals (try to re factor)
    private int targetElementsCount;
    private int seed = 0;
    private Transform parentTransform;
    private CandidateComponents candidateComponents;

    private Dictionary<string, List<GameObject>> pools;

    private CandidatesManager candidatesManager;

    public Dictionary<Vector3, Element> initilizedElements = new Dictionary<Vector3, Element>();

    private List<GameObject> activePoolObjects = new List<GameObject>();    
    
    public int TargetElementsCount { get => targetElementsCount; }
    public int Seed { get => seed; }
    public Transform ParentTransform { get => parentTransform; }

    public CandidateComponents CandidateComponents { get => candidateComponents; }

    public void Init(Transform parentTransform, ZonesGenerator zonesGenerator)
    {
        this.parentTransform = parentTransform;

        activePoolObjects = new List<GameObject>();

        InitDataSetElements();

        if (randomizeOnStart)
        {
            seed = Random.Range(0, int.MaxValue);
        }

        candidatesManager = new CandidatesManager(this, zonesGenerator);

        candidateComponents = new CandidateComponents(candidatesManager);
    }

    protected void InitDataSetElements()
    {
        int length = Sets.Count;
        for (int i = 0; i < length; i++)
        {
            Sets[i].InitTemplateElements();
        }
    }

    public void StartFactory(int targetElementsCount)
    {
        this.targetElementsCount = targetElementsCount;
        candidatesManager.CreateRandomSpawnPoint();
    }

    public void ReclaimAll()
    {
        if (randomizeOnStart)
        {
            seed = Random.Range(0, int.MaxValue);
        }

        if (initilizedElements.Count > 0)
        {
            Destroy(initilizedElements[Vector3.zero].gameObject);
        }

        while (activePoolObjects.Count > 0)
        {
            Reclaim(activePoolObjects[activePoolObjects.Count - 1]);
        }

        activePoolObjects = new List<GameObject>();

        initilizedElements = new Dictionary<Vector3, Element>();
        candidatesManager.CleanUp();
    }

    public ConnPoint GetNewElement(Candidate newCandidate, Transform transform)
    {
        GameObject newCandidateGO = recycle ? TryToPoolCandidateGO(newCandidate, transform) : newCandidateGO = InstantiateCandidateGO(newCandidate, transform);

        ConnPoint newConnPoint = InitCandidateElementComponent(newCandidateGO, newCandidate);

        InitCandidateVolumeComponent(newCandidateGO, newCandidate);

        activePoolObjects.Add(newCandidateGO);

        return newConnPoint;
    }

    protected GameObject TryToPoolCandidateGO(Candidate newCandidate, Transform transform)
    {
        GameObject newCandidateGO;

        if (pools == null)
        {
            InitPools();
        }

        List<GameObject> pool = pools[newCandidate.ID];
        int lastIndex = pool.Count - 1;

        if (lastIndex >= 0)
        {
            newCandidateGO = pool[lastIndex];

            newCandidateGO.transform.position = newCandidate.WorldPos;
            newCandidateGO.transform.localRotation = newCandidate.Rotation;

            newCandidateGO.gameObject.SetActive(true);

            pool.RemoveAt(lastIndex);
        }
        else
        {
            newCandidateGO = InstantiateCandidateGO(newCandidate, transform);
        }

        return newCandidateGO;
    }

    protected GameObject InstantiateCandidateGO(Candidate newCandidate, Transform transform)
    {
        GameObject newCandidateGO = Instantiate
            (
                newCandidate.GameObject,
                newCandidate.WorldPos,
                newCandidate.Rotation,
                transform
            );

        return newCandidateGO;
    }

    protected ConnPoint InitCandidateElementComponent(GameObject newCandidateGO, Candidate newCandidate)
    {
        Element newElement = newCandidateGO.GetComponent<Element>();
        ConnPoint newConnPoint = null;

        int connPointsCount = newElement.connPoints.Count;
        for (int i = 0; i < connPointsCount; i++)
        {
            ConnPoint connPoint = newElement.connPoints[i];
            connPoint.open = newCandidate.ConnPointCandidates[i].Open;
            connPoint.transform.rotation = newCandidate.ConnPointCandidates[i].Rotation;

            if (connPoint.transform.localPosition == newCandidate.NewConnPointCandidate.LocalPosition)
            {
                newConnPoint = connPoint;
            }
        }

        if (newConnPoint == null) throw new System.Exception("DungeonGenerator:: No NEW ELEMENT connection point found.");

        initilizedElements.Add(newCandidate.WorldPos, newElement);

        return newConnPoint;
    }

    protected void InitCandidateVolumeComponent(GameObject newCandidateGO, Candidate newCandidate)
    {
        Volume newVolume = newCandidateGO.GetComponent<Volume>();
        newVolume.RecalculateBounds();

        List<GameObject> newElementVoxels = newVolume.voxels;
        int voxelsCount = newVolume.voxels.Count;
        for (int i = 0; i < voxelsCount; i++)
        {
            GameObject voxelGO = newElementVoxels[i];
            Vector3 worldPosition = newCandidate.GetVoxelWorldPos(voxelGO.transform.localPosition);

            InitializeGlobalVoxel(worldPosition, voxelGO);
        }
    }

    public void Reclaim(GameObject elementToRecycle)
    {
        if (recycle)
        {
            if (pools == null)
            {
                InitPools();
            }

            Element element = elementToRecycle.GetComponent<Element>();
            pools[element.ID].Add(elementToRecycle);
            elementToRecycle.gameObject.SetActive(false);
        }
        else
        {
            Destroy(elementToRecycle.gameObject);
        }

        activePoolObjects.Remove(elementToRecycle);
    }

    protected void InitPools()
    {
        pools = new Dictionary<string, List<GameObject>>();

        int length = Sets.Count;
        for (int i = 0; i < length; i++)
        {
            pools = Sets[i].GetElementPools(pools);
        }
    }

    protected void InitializeGlobalVoxel(Vector3 voxelWorldPos, GameObject voxelGO)
    {
        Voxel voxel = voxelGO.GetComponent<Voxel>();
        voxelWorldPos = voxelWorldPos.RoundVec3ToInt();

        if (candidatesManager.CandidateVoxels.ContainsKey(voxelWorldPos))
        {
            candidatesManager.CandidateVoxels[voxelWorldPos] = voxelGO;            
            voxel.worldPosition = voxelWorldPos;
            Voxel.SetGameObjectName(voxelGO, voxelWorldPos);
        }
        else
        {
            Debug.LogError("ElementsFactory::Voxel GameObject we're trying to add to globalVoxels is NOT defined: " + voxelWorldPos.ToString());
        }
    }
}