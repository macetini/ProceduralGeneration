using System.Collections.Generic;
using UnityEngine;

public class CandidatesManager
{
    private ElementsFactory factoryOwner;
    private ZonesGenerator zonesGenerator;

    private DRandom random;
    private Element[] potentialCandidates;

    private Dictionary<Vector3, GameObject> candidateVoxels = new Dictionary<Vector3, GameObject>();
    private List<Candidate> openCandidates = new List<Candidate>();
    private List<Candidate> acceptedCandidates = new List<Candidate>();

    public CandidatesManager(ElementsFactory factoryOwner, ZonesGenerator zonesGenerator)
    {
        this.factoryOwner = factoryOwner;
        this.zonesGenerator = zonesGenerator;

        random = new DRandom();
        random.Init(factoryOwner.Seed);
    }

    public List<Candidate> OpenCandidates => openCandidates;
    public List<Candidate> AcceptedCandidates => acceptedCandidates;
    public Dictionary<Vector3, GameObject> CandidateVoxels => candidateVoxels;

    public void CleanUp()
    {
        candidateVoxels = new Dictionary<Vector3, GameObject>();
        acceptedCandidates = new List<Candidate>();

        openCandidates = new List<Candidate>();
    }

    protected Candidate GetFirstOpenCandidate()
    {
        return openCandidates[0];
    }

    public Candidate GetFirstAcceptedCandidate()
    {
        return acceptedCandidates[0];
    }

    public Candidate GetLastAcceptedCandidate()
    {
        int lastIndex = AcceptedCandidates.Count - 1;
        Candidate lastCandidate = AcceptedCandidates[lastIndex];

        return lastCandidate;
    }

    public void CreateRandomSpawnPoint()
    {
        List<Element> spawnTemplates = factoryOwner.Sets[0].spawnTemplates;

        int spawnTemplatesCount = factoryOwner.Sets[0].spawnTemplates.Count - 1;
        int spawnRandomIndex = random.range(0, spawnTemplatesCount);

        Candidate startCandidate = new Candidate(spawnTemplates[spawnRandomIndex].gameObject, factoryOwner.Sets[0]);

        openCandidates.Add(startCandidate);

        startCandidate.SetVoxelsWorldPos();

        AddGlobalVoxelCandidates(startCandidate.VoxWorldPos);
        startCandidate.UpdateConnPointsWorldPos();

        acceptedCandidates.Add(startCandidate);
    }

    public void GenerateNextCandidate()
    {
        Candidate lastCandidate = GetFirstOpenCandidate();

        //GET NEW RANDOM CONNECTION POINTS FROM THE CURRENTLY OPEN ELEMENT
        List<ConnPointCandidate> connPointCandidate = lastCandidate.GetAllOpenConnPoints(random);

        int connPointCandidateCount = connPointCandidate.Count;
        int connPointCandidateIndex = 0;

        Candidate newCandidate = null;
        do
        {
            ConnPointCandidate lastConnPointCandidate = connPointCandidate[connPointCandidateIndex++];
            newCandidate = ConnectNewCandidateToConnPoint(lastConnPointCandidate);

        } while (newCandidate == null && connPointCandidateIndex < connPointCandidateCount);

        if (newCandidate == null)
        {
            throw new System.Exception("CandidateManager::No Candidate fits.");
        }
    }

    protected Candidate ConnectNewCandidateToConnPoint(ConnPointCandidate lastConnPointCandidate)
    {
        Candidate lastCandidate = lastConnPointCandidate.Owner;

        //LOOK IF ELEMENT IN THE SPAWN ZONE
        ZoneItem zoneItem = GetCandidateSpawnZoneItem(lastConnPointCandidate, lastCandidate.Step);

        //PUT IN SEPARATE FUNCTION
        DungeonSet spawnZoneSet = null;
        if (zoneItem != null)
        {
            spawnZoneSet = zoneItem.spawnSet;

            if (spawnZoneSet == null && zoneItem.randomizeSpawnSet)
            {
                int randomSetIndex = Random.Range(1, factoryOwner.Sets.Count);
                spawnZoneSet = factoryOwner.Sets[randomSetIndex];
                zoneItem.spawnSet = spawnZoneSet;
            }
        }
        
        DungeonSet lastCandidateSet = lastCandidate.Set;
        
        DungeonSet candidatesSet = SetPotentialCandidates(spawnZoneSet, lastCandidateSet);

        //PUT IN SEPARATE FUNCTION       
        if (zoneItem != null && zoneItem.emptySpace)
        {
            potentialCandidates = lastCandidateSet.GetAllClosingElementsShuffled(random);
        }        
        //

        Candidate newCandidate = null;

        int timesLooped = 0;
        int maximumLoopsAllowed = potentialCandidates.Length << factoryOwner.loopCheckCount;
        do
        {
            if (timesLooped++ > maximumLoopsAllowed)
            {
                throw new System.Exception("CandidatesManager::Candidate generation takes too long - Possible infinite loop.");
            }

            newCandidate = CreateNewCandidate(lastConnPointCandidate, candidatesSet);

        } while (potentialCandidates.Length > 0 && newCandidate == null);

        if (newCandidate != null)
        {
            AcceptNewCandidate(newCandidate, lastCandidate);
        }

        return newCandidate;
    }

    protected DungeonSet SetPotentialCandidates(DungeonSet candidatesSet, DungeonSet lastCandidateSet)
    {
        if (candidatesSet == null)
        {
            if (zonesGenerator.clamp)
            {
                candidatesSet = lastCandidateSet;
                potentialCandidates = candidatesSet.GetAllClosingElementsShuffled(random);

                return lastCandidateSet;
            }

            candidatesSet = factoryOwner.Sets[0];
        }

        bool onlyTwoWayElemsAllowed = OnlyTwoWayElemsAllowed();
        potentialCandidates = onlyTwoWayElemsAllowed ? candidatesSet.GetAllTwoWayOpenElementsShuffled(random) : candidatesSet.GetAllOpenElementsShuffled(random);

        return candidatesSet;
    }

    protected Candidate CreateNewCandidate(ConnPointCandidate lastConnPointCandidate, DungeonSet candidatesSet)
    {
        VoxelStep newCandidateStepVoxel = new VoxelStep();

        // GENERATE ROOM WITH ONE CONNECTION POINT (only after all target rooms have been exhausted)
        if (acceptedCandidates.Count >= factoryOwner.TargetElementsCount)
        {
            potentialCandidates = lastConnPointCandidate.OwnerSet.GetAllClosingElementsShuffled(random);
        }

        // GET RANDOM NEW CANDIDATE                    
        Candidate newCandidate = GetNewCandidate(potentialCandidates, candidatesSet);
        potentialCandidates = potentialCandidates.RemoveFromArray(newCandidate.Element);

        // CHECK IF NEW ELEMENT ALLOWED NEIGHBOUR TO LAST ELEMENT            
        bool notAllowedNeighbours = CheckIfCandidatesCanBeNeighbours(GetFirstOpenCandidate(), newCandidate);
        if (notAllowedNeighbours)
        {
            potentialCandidates = candidatesSet.GetAllHallwayElementsShuffled(random);
            return null;
        }

        // COONECT THE CANDIDATES                         
        ConnPointCandidate newConnPointCandidate = AlignNewCandidateWithConnPoint(lastConnPointCandidate, newCandidate);

        CandidatesConnection candidatesConnection = CreateCandidatesConnection(lastConnPointCandidate, newConnPointCandidate);
        candidatesConnection.LastCandidateWorldPos = GetFirstOpenCandidate().WorldPos;

        newCandidate.CandidatesConnection = candidatesConnection;

        //STEP VOXELS
        newCandidateStepVoxel.oldStepVoxelsPos = GetOldStepVoxels(lastConnPointCandidate);
        newCandidateStepVoxel.newStepVoxelsPos = GetNewStepVoxels(newCandidate);
        //

        //candidatesConnection.NewCandidateStepVoxel = newCandidateStepVoxel;

        //CHECK OVERLAP
        bool overlap = CheckIfNewElementOverlaps(newCandidateStepVoxel, newCandidate);
        if (overlap)
        {
            if (potentialCandidates.Length == 0)
            {
                potentialCandidates = candidatesSet.GetAllClosingElementsShuffled(random);
            }

            return null;
        }        

        return newCandidate;
    }

    protected ZoneItem GetCandidateSpawnZoneItem(ConnPointCandidate lastConnPointCandidate, Vector3 step)
    {
        Vector3 connPointCandidateStep = lastConnPointCandidate.Rotation * step;
        Vector3 oldConnPointWorldPostion = lastConnPointCandidate.WorldPosition;
        Vector3 oldConnPointStepVoxel = (oldConnPointWorldPostion + connPointCandidateStep).RoundVec3ToInt();

        List<ZoneItem> spawnZones = zonesGenerator.SpawnZones;

        int spawnZonesCount = spawnZones.Count;
        for (int i = 0; i < spawnZonesCount; i++)
        {
            ZoneItem zoneItem = spawnZones[i];

            bool candidateInSpawnZone = zoneItem.ContainsPoint(oldConnPointStepVoxel);
            if (candidateInSpawnZone)
            {
                return zoneItem;
            }
        }

        return null;
    }

    protected Candidate GetNewCandidate(Element[] possibleElements, DungeonSet ownerSet)
    {
        bool onlyTwoWayElemsAllowed = OnlyTwoWayElemsAllowed();

        GameObject randomElement;

        if (onlyTwoWayElemsAllowed)
        {
            int randomRoomIndex = random.range(0, possibleElements.Length - 1);
            randomElement = possibleElements[randomRoomIndex].gameObject;
        }
        else
        {
            randomElement = GetElemThatMightBeOneWay(possibleElements);
        }

        
        Candidate newCandidate = new Candidate(randomElement, ownerSet);
        newCandidate.SetWorldPosAndRotation(factoryOwner.ParentTransform.position, factoryOwner.ParentTransform.rotation);

        return newCandidate;
    }

    protected bool OnlyTwoWayElemsAllowed()
    {
        float elementsCountRatio = Mathf.Clamp(acceptedCandidates.Count / (float)factoryOwner.TargetElementsCount, 0, 1);
        bool onlyTwoWayElemsAllowed = elementsCountRatio <= factoryOwner.closingElemsPct;

        return onlyTwoWayElemsAllowed;
    }

    protected GameObject GetElemThatMightBeOneWay(Element[] possibleElements)
    {
        int randomRoomIndex = random.range(0, possibleElements.Length - 1);
        GameObject roomToTry = possibleElements[randomRoomIndex].gameObject;

        int connPointsCount = roomToTry.GetComponent<Element>().connPoints.Count;

        if (connPointsCount == 1 && possibleElements.Length > 1)
        {
            float elementsCountRatio = acceptedCandidates.Count / (float)factoryOwner.TargetElementsCount;
            float chance = 1f - Mathf.Sqrt(elementsCountRatio); //the closer we are to target the less of a chance of changing rooms
            float randomValue = random.value();

            if (randomValue < chance)
            {
                return GetElemThatMightBeOneWay(possibleElements);
            }
        }

        return roomToTry;
    }

    protected bool CheckIfCandidatesCanBeNeighbours(Candidate lastCandidate, Candidate newCandidate)
    {
        bool isLastElementRoom = lastCandidate.Element.type == ElementTypeEnum.ROOM;
        bool isCurrentElementRoom = newCandidate.Element.type == ElementTypeEnum.ROOM;

        bool notAllowed = isLastElementRoom && isCurrentElementRoom;

        return notAllowed;
    }

    protected ConnPointCandidate AlignNewCandidateWithConnPoint(ConnPointCandidate lastConnPointCandidate, Candidate newCandidate)
    {
        ConnPointCandidate newConnPointCandidate = newCandidate.GetRandomOpenConnPoint(random);

        float lastAngleY = lastConnPointCandidate.Rotation.eulerAngles.y;
        float newAngleY = newConnPointCandidate.Rotation.eulerAngles.y;
        float angleDiffernce = lastAngleY - newAngleY + 180f;

        newCandidate.Rotation = Quaternion.AngleAxis(angleDiffernce, Vector3.up);

        Vector3 lastConnPointWorldPos = lastConnPointCandidate.WorldPosition;

        Vector3 translation = lastConnPointWorldPos - newCandidate.Rotation * newConnPointCandidate.LocalPosition;
        newCandidate.WorldPos += translation;

        newCandidate.SetVoxelsWorldPos(translation, newCandidate.Rotation);
        newCandidate.UpdateConnPointsWorldPos(angleDiffernce);

        return newConnPointCandidate;
    }

    protected CandidatesConnection CreateCandidatesConnection(ConnPointCandidate lastConnPointCandidate, ConnPointCandidate newConnPointCandidate)
    {
        CandidatesConnection candidatesConnection = new CandidatesConnection
        {
            LastConnPointCandidate = lastConnPointCandidate,
            NewConnPointCandidate = newConnPointCandidate
        };

        return candidatesConnection;
    }

    //TODO - MAYBE SPLIT IN TWO FUNCTIONS?
    protected bool CheckIfNewElementOverlaps(VoxelStep voxelStep, Candidate newCandidate)
    {
        List<GameObject> voxels = newCandidate.Voxels;

        int newVoxelsCount = voxels.Count;
        for (int i = 0; i < newVoxelsCount; i++)
        {
            GameObject voxel = voxels[i];

            Vector3 voxelWorldPosition = newCandidate.GetVoxelWorldPos(voxel.transform.localPosition);
            Vector3 newElementVoxel = voxelWorldPosition.RoundVec3ToInt();

            bool voxelOverlap = candidateVoxels.ContainsKey(newElementVoxel);
            if (voxelOverlap) return true;

            voxelOverlap = voxelStep.oldStepVoxelsPos.Contains(newElementVoxel);
            if (voxelOverlap) return true;
        }

        foreach (Vector3 newStepVoxelPos in voxelStep.newStepVoxelsPos)
        {
            bool voxelOverlap = candidateVoxels.ContainsKey(newStepVoxelPos);
            if (voxelOverlap) return true;

            voxelOverlap = voxelStep.oldStepVoxelsPos.Contains(newStepVoxelPos);
            if (voxelOverlap) return true;
        }

        return false;
    }

    protected HashSet<Vector3> GetOldStepVoxels(ConnPointCandidate lastConnPoint)
    {
        HashSet<Vector3> stepVoxelsPos = new HashSet<Vector3>();

        int openSetCount = openCandidates.Count;
        for (int i = 0; i < openSetCount; i++)
        {
            Candidate openCandidate = openCandidates[i];
            Volume volume = openCandidate.Volume;

            int openSetConnPointsCount = openCandidates[i].ConnPointCandidates.Count;
            for (int j = 0; j < openSetConnPointsCount; j++)
            {
                ConnPointCandidate oldConnPointCandidate = openCandidate.ConnPointCandidates[j];

                if (!oldConnPointCandidate.Open || oldConnPointCandidate == lastConnPoint) continue;

                Vector3 connPointCandidateStep = oldConnPointCandidate.Rotation * openCandidate.HalfStep;

                Vector3 oldConnPointWorldPostion = oldConnPointCandidate.WorldPosition;
                Vector3 oldConnPointStepVoxel = (oldConnPointWorldPostion + connPointCandidateStep).RoundVec3ToInt();

                stepVoxelsPos.Add(oldConnPointStepVoxel);
            }
        }

        return stepVoxelsPos;
    }

    protected HashSet<Vector3> GetNewStepVoxels(Candidate newCandidate)
    {
        HashSet<Vector3> StepVoxelsPos = new HashSet<Vector3>();

        Volume volume = newCandidate.Volume;

        ConnPointCandidate firstOpenConnPointCandidate = newCandidate.NewConnPointCandidate;

        int connPointsCount = newCandidate.ConnPointCandidates.Count;
        for (int i = 0; i < connPointsCount; i++)
        {
            ConnPointCandidate newConnPointCandidate = newCandidate.ConnPointCandidates[i];

            //check all OPEN connection points BUT the one we're connecting with.
            if (!newConnPointCandidate.Open || newConnPointCandidate == firstOpenConnPointCandidate)
            {
                continue;
            }

            Vector3 connPointCandidateStep = newConnPointCandidate.Rotation * newCandidate.HalfStep;

            Vector3 newConnPointWorldPostion = newConnPointCandidate.WorldPosition;
            Vector3 StepVoxelPosition = (newConnPointWorldPostion + connPointCandidateStep).RoundVec3ToInt();

            StepVoxelsPos.Add(StepVoxelPosition);
        }

        return StepVoxelsPos;
    }

    /*
    protected void AddStepVoxel(Vector3 connPointVoxelOwnerPostion, Color color)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        sphere.transform.parent = this.parentTransform;
        sphere.transform.position = connPointVoxelOwnerPostion;
        sphere.GetComponent<Renderer>().material.color = color;
        sphere.name += connPointVoxelOwnerPostion;

        StepVoxels.Add(sphere);
    }
    */

    protected void AddGlobalVoxelCandidates(Dictionary<Vector3, Vector3> voxelsWorldPos)
    {
        foreach (KeyValuePair<Vector3, Vector3> entry in voxelsWorldPos)
        {
            Vector3 voxelWorldPos = entry.Value.RoundVec3ToInt();

            if (!candidateVoxels.ContainsKey(voxelWorldPos))
            {
                candidateVoxels.Add(voxelWorldPos, null);
            }
            else
            {
                Debug.LogError("Voxel Candidate we're trying to add to globalVoxels is already defined: " + voxelWorldPos.ToString());
            }
        }
    }

    protected void AcceptNewCandidate(Candidate newCandidate, Candidate lastCandidate)
    {
        newCandidate.LastConnPointCandidate.Open = newCandidate.NewConnPointCandidate.Open = false;

        if (!lastCandidate.HasOpenConnection()) openCandidates.Remove(lastCandidate);

        if (newCandidate.HasOpenConnection()) openCandidates.Add(newCandidate);

        AddGlobalVoxelCandidates(newCandidate.VoxWorldPos);

        acceptedCandidates.Add(newCandidate);
    }
}