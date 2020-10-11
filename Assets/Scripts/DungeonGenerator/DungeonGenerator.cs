using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main Class that spawns dungeon items(rooms). It consists of <b>Elements factory</b> and <b>Zones generator</b>.
/// The zone generator defines <b>dungeon sets</b> - the arrays of items that can be spawned in before mentioned zone.
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    public ElementsFactory elementsFactory;
    public ZonesGenerator zonesGenerator;

    public bool stepByStep = false;

    public Button genButton;

    //TODO - perhaps later on
    //public bool showStepVoxels = false;

    public int targetElementsCount = 10;

    //private List<GameObject> stepVoxels = new List<GameObject>();

    void Start()
    {
        if (genButton != null)
        {
            genButton.onClick.AddListener(ResetGeneration);
        }

        elementsFactory.Init(this.transform, zonesGenerator);

        /*
        GenerateCandidates();
        BuildSpawnPointElement();
        BuildAllElements();
        */

        //ProcessConnPoints();
        //Debug.Break();
    }

    public void Update()
    {
        if (stepByStep && Input.GetKeyDown(KeyCode.N))
        {
            GenerateNextCandidate();
            BuildLastStepElement();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ResetGeneration();
        }
    }

    protected void ResetGeneration()
    {
        Debug.Log("New Generation!");

        elementsFactory.ReclaimAll();

        GenerateCandidates();
        BuildSpawnPointElement();
        BuildAllElements();
    }

    protected void GenerateCandidates()
    {
        DDebugTimer.Start();

        //ClearStepVoxels();
        
        elementsFactory.StartFactory(targetElementsCount);

        if (stepByStep) return;

        int infiniteLoopCheckCount = 0, infiniteLoopCheck = targetElementsCount << elementsFactory.loopCheckCount;
        while (elementsFactory.CandidateComponents.GetOpenCandidatesCount() > 0)
        {
            if (infiniteLoopCheckCount++ > infiniteLoopCheck)
            {
                throw new System.Exception("DungeonGenerator::Dungeon generation takes too long. - Possible infinite loop.");
            }

            GenerateNextCandidate();

            if (stepByStep) break;
        }

        Debug.Log("DungeonGenerator::Generation completed : " + DDebugTimer.Lap() + "ms");
        Debug.Log("DungeonGenerator::Elements targeted : " + targetElementsCount + ". Candidates accepted : " + elementsFactory.CandidateComponents.GetAcceptedCandidatesCount());
    }

    protected void GenerateNextCandidate()
    {        
        if (elementsFactory.CandidateComponents.GetOpenCandidatesCount() <= 0)
        {
            throw new System.Exception("DungeonGenerator::No open set.");
        }

        elementsFactory.CandidateComponents.GenerateNextCandidate();
    }

    protected void BuildSpawnPointElement()
    {
        Candidate startCandidate = elementsFactory.CandidateComponents.GetFirstAcceptedCandidate();
        GameObject randomSpawnGO = Instantiate(startCandidate.GameObject, this.gameObject.transform);

        Element spawnElement = randomSpawnGO.GetComponent<Element>();
        elementsFactory.initilizedElements.Add(startCandidate.WorldPos, spawnElement);
        Volume spawnVolume = randomSpawnGO.GetComponent<Volume>();
        spawnVolume.RecalculateBounds();
    }

    protected void BuildLastStepElement()
    {
        Candidate lastCandidate = elementsFactory.CandidateComponents.GetLastAcceptedCandidate();
        BuildElement(lastCandidate);

        /*
        ClearStepVoxels();

        foreach (Vector3 voxelPos in lastCandidate.CandidatesConnection.NewCandidateStepVoxel.newStepVoxelsPos)
        {
            AddStepVoxel(voxelPos, Color.green);
        }
        */
    }

    /*
    protected void ClearStepVoxels()
    {
        int length = stepVoxels.Count;
        for (int i = 0; i < length; i++)
        {
            GameObject.DestroyImmediate(stepVoxels[i]);
        }

        stepVoxels.Clear();
    }

    protected void AddStepVoxel(Vector3 stepVoxelWorldPos, Color color)
    {
        GameObject sphereGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        sphereGO.transform.parent = this.transform;
        sphereGO.transform.position = stepVoxelWorldPos;
        sphereGO.GetComponent<Renderer>().material.color = color;
        sphereGO.name += stepVoxelWorldPos;

        stepVoxels.Add(sphereGO);
    }   
    */
    /// <summary>
    /// Gets all accepted candidates from Elements Factory and initializes new element for each.
    /// </summary>
    protected void BuildAllElements()
    {
        DDebugTimer.Start();

        List<Candidate> acceptedCandidates = elementsFactory.CandidateComponents.GetAllAcceptedCandidates();
        int count = acceptedCandidates.Count;
        for (int i = 1; i < count; i++)
        {
            Candidate acceptedCandidate = acceptedCandidates[i];
            BuildElement(acceptedCandidate);
        }

        Debug.Log("DungeonGenerator::Generated Elements Initialized : " + DDebugTimer.Lap() + "ms");
    }    

    /// <summary>
    /// Creates a dungeon room <b>GameObject</b> from a accepted <b>Candidate</b> object.    
    /// If it is possible the object will be pooled, else it is fully initialized.
    /// </summary>
    /// <param name="acceptedCandidate">Object that defines the new element. Objects are usually provided by <b>Elements Factory</b>.</param>
    protected void BuildElement(Candidate acceptedCandidate)
    {
        ConnPoint newConnPoint = elementsFactory.GetNewElement(acceptedCandidate, this.transform);

        CandidatesConnection candidatesConnection = acceptedCandidate.CandidatesConnection;
        Element previousElement = elementsFactory.initilizedElements[candidatesConnection.LastCandidateWorldPos];

        ConnPoint lastConnPoint = GetConnPointAtPosition(previousElement.connPoints, acceptedCandidate.LastConnPointCandidate.LocalPosition);

        newConnPoint.sharedConnPoint = lastConnPoint ?? throw new System.Exception("DungeonGenerator:: No PREVIOUS ELEMENT connection point found.");

        lastConnPoint.sharedConnPoint = newConnPoint;
    }    

    protected ConnPoint GetConnPointAtPosition(List<ConnPoint> connPoints, Vector3 localPosition)
    {
        int connPointsCount = connPoints.Count;
        for (int j = 0; j < connPointsCount; j++)
        {
            ConnPoint connPoint = connPoints[j];

            if (connPoint.transform.localPosition == localPosition)
            {
                return connPoint;
            }
        }

        return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(10f, 0f, 10f), new Vector3(30f, 15f, 30f));
    }
}
