using System.Collections.Generic;
using Assets.Meta.Sets;
using Assets.Scripts.Generators.Dungeon.Elements;
using Assets.Scripts.Utils;
using Assets.Scripts.Generators.Meta.VoxelData;
using UnityEngine;
using Assets.Scripts.Generators.Dungeon.Points;

namespace Assets.Scripts.Generators.Dungeon.Candidates
{
    public class Candidate
    {
        //TODO - TOO MANY GLOBALS (Way, way too many - re factor)
        private readonly DungeonSet set;

        //private Vector3 worldPosition;
        //private Quaternion rotation;
        //private List<ConnectionPointCandidate> connPointCandidates;
        //private CandidatesConnection candidatesConnection;

        private readonly GameObject gameObject;
        private readonly Element element;
        private readonly Volume volume;
        private readonly Vector3 halfStep;
        private readonly Vector3 step;
        private readonly string id;

        public DungeonSet Set => set;
        public Vector3 WorldPosition { get; set; }
        public Quaternion Rotation { get; set; }
        public List<ConnectionPointCandidate> ConnPointCandidates { get; set; }
        public CandidatesConnection CandidatesConnection { get; set; }

        //public Dictionary<Vector3, Vector3> VoxelsWorldPosition => voxelWorldPosition;
        public GameObject GameObject => gameObject;
        public Element Element => element;
        public Volume Volume => volume;
        public List<Voxel> Voxels => Volume.Voxels; //TODO - Refactor this part.

        public List<Vector3> CandidateVoxelsWorldPosition;
        public Dictionary<Vector3, Vector3> RelativeVoxelsWorldPosition { get; private set; }  //TODO - Refactor this part.

        public ConnectionPointCandidate LastConnPointCandidate => CandidatesConnection.LastConnPointCandidate;
        public ConnectionPointCandidate NewConnPointCandidate => CandidatesConnection.NewConnPointCandidate;

        public Vector3 HalfStep => halfStep;
        public Vector3 Step => step;
        public string ID => id;

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

        public ConnectionPointCandidate GetFirstOpenConnectionPoint()
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
            List<ConnectionPoint> itemConnPoints = element.connectionPoints;
            int connPointsCount = itemConnPoints.Count;

            ConnPointCandidates = new List<ConnectionPointCandidate>(connPointsCount);
            for (int i = 0; i < connPointsCount; i++)
            {
                ConnectionPointCandidate cloneGO = itemConnPoints[i].CloneCandidate;
                cloneGO.Owner = this;
                ConnPointCandidates.Add(cloneGO);
            }
        }

        public void SetWorldPosAndRotation(Vector3 worldPos, Quaternion rotation)
        {
            WorldPosition = worldPos;
            Rotation = rotation;
        }

        public ConnectionPointCandidate GetRandomOpenConnPoint(DRandom random)
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

        public List<ConnectionPointCandidate> GetAllOpenConnPoints(DRandom random)
        {
            ConnPointCandidates.Shuffle(random.random);

            List<ConnectionPointCandidate> openConnPointCandidates = new();

            int connPointsCount = ConnPointCandidates.Count;
            for (int i = 0; i < connPointsCount; i++)
            {
                ConnectionPointCandidate connPointCandidate = ConnPointCandidates[i];

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

        public void SetVoxelsWorldPosition(Vector3 translation = default(Vector3), Quaternion rotation = default(Quaternion))
        {
            int voxelsCount = Voxels.Count;

            RelativeVoxelsWorldPosition = new Dictionary<Vector3, Vector3>(voxelsCount);
            
            CandidateVoxelsWorldPosition = new List<Vector3>(voxelsCount);

            for (int i = 0; i < voxelsCount; i++)
            {
                Vector3 localPositionLoop = Voxels[i].transform.localPosition; //TODO - Maybe use Voxel internal variable for localPosition?
                Vector3 worldPositionLoop = (rotation * localPositionLoop + translation).RoundVec3ToInt();

                RelativeVoxelsWorldPosition.Add(localPositionLoop, worldPositionLoop);

                CandidateVoxelsWorldPosition.Add(worldPositionLoop);
            }
        }

        public void UpdateConnPointsWorldPos(float angleDifference = 0f)
        {
            int connPointsCount = ConnPointCandidates.Count;
            for (int i = 0; i < connPointsCount; i++) //TODO - Switch to enhanced for.
            {
                ConnectionPointCandidate connPointCandidate = ConnPointCandidates[i];

                Quaternion newRotation = Quaternion.AngleAxis(ConnPointCandidates[i].Rotation.eulerAngles.y + angleDifference, Vector3.up);
                connPointCandidate.Rotation = newRotation;

                Vector3 localPosition = connPointCandidate.VoxelOwner.transform.localPosition;
                connPointCandidate.WorldPosition = GetConnPointCandidateWorldPosition(localPosition, newRotation);
            }
        }

        protected Vector3 GetConnPointCandidateWorldPosition(Vector3 localPosition, Quaternion rotation)
        {
            Vector3 worldPosition = GetVoxelWorldPosition(localPosition);

            worldPosition += rotation * HalfStep;

            return worldPosition;
        }

        public Vector3 GetVoxelWorldPosition(Vector3 localPosition)
        {
            return RelativeVoxelsWorldPosition[localPosition];
        }
    }
}