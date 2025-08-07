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
        public DungeonSet Set { get; private set; }
        public Vector3 WorldPosition { get; set; }
        public Quaternion Rotation { get; set; }
        public List<ConnectionPointCandidate> ConnPointCandidates { get; set; }
        public CandidatesConnection CandidatesConnection { get; set; }
        public GameObject GameObject { get; private set; }
        public Element Element { get; private set; }
        public Volume Volume { get; private set; }
        public List<Voxel> Voxels => Volume.Voxels;
        public Dictionary<Vector3, Vector3> RelativeVoxelsWorldPosition { get; private set; }

        public ConnectionPointCandidate LastConnPointCandidate => CandidatesConnection.LastConnPointCandidate;
        public ConnectionPointCandidate NewConnPointCandidate => CandidatesConnection.NewConnPointCandidate;

        public Vector3 HalfStep { get; private set; }
        public Vector3 Step { get; private set; }
        public string ID { get; private set; }

        public Candidate(GameObject gameObject, DungeonSet set)
        {
            GameObject = gameObject;
            Set = set;

            Element = gameObject.GetComponent<Element>();
            Volume = gameObject.GetComponent<Volume>();

            HalfStep = new Vector3(Volume.VoxelScale * 0.5f, 0f, 0f);
            Step = new Vector3(Volume.VoxelScale, 0f, 0f);

            ID = Element.ID;

            CloneConnPoints();
        }

        protected void CloneConnPoints()
        {
            List<ConnectionPoint> itemConnPoints = Element.connectionPoints;
            ConnPointCandidates = new List<ConnectionPointCandidate>(itemConnPoints.Count);
            itemConnPoints.ForEach((itemConnPoint) =>
            {
                ConnectionPointCandidate connPointClone = itemConnPoint.CloneCandidate;
                connPointClone.Owner = this;
                ConnPointCandidates.Add(connPointClone);
            });
        }

        public void SetWorldPosAndRotation(Vector3 worldPos, Quaternion rotation)
        {
            WorldPosition = worldPos;
            Rotation = rotation;
        }

        public bool HasOpenConnection()
        {
            foreach (ConnectionPointCandidate connPointCandidate in ConnPointCandidates)
            {
                if (connPointCandidate.Open)
                {
                    return true;
                }
            }

            return false;
        }

        //NOT USED - DO NOT DELETE, possibly future use.
        public ConnectionPointCandidate GetFirstOpenConnectionPoint()
        {
            foreach (ConnectionPointCandidate connPointCandidate in ConnPointCandidates)
            {
                if (connPointCandidate.Open)
                {
                    return connPointCandidate;
                }
            }

            Debug.Log("Candidate::GetFirstOpenConnPoint() - No open connection points.");

            return null;
        }

        public ConnectionPointCandidate GetRandomOpenConnPoint(DRandom random)
        {
            ConnPointCandidates.Shuffle(random.random);

            foreach (ConnectionPointCandidate connPointCandidate in ConnPointCandidates)
            {
                if (connPointCandidate.Open)
                {
                    return connPointCandidate;
                }
            }

            Debug.Log("Candidate::GetRandomOpenConnPoint() - No open connection points.");

            return null;
        }

        public List<ConnectionPointCandidate> GetAllOpenConnPoints(DRandom random)
        {
            ConnPointCandidates.Shuffle(random.random);

            List<ConnectionPointCandidate> openConnPointCandidates = new();

            foreach (ConnectionPointCandidate connPointCandidate in ConnPointCandidates)
            {
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

        public void SetRelativeVoxelsWorldPosition(Vector3 translation = default, Quaternion rotation = default)
        {
            RelativeVoxelsWorldPosition = new Dictionary<Vector3, Vector3>(Voxels.Count);
            Voxels.ForEach((voxel) =>
            {
                Vector3 voxelLocalPosition = voxel.LocalPosition;
                Vector3 voxelWorldPosition = (rotation * voxelLocalPosition + translation).RoundVec3ToInt();

                RelativeVoxelsWorldPosition.Add(voxelLocalPosition, voxelWorldPosition);
            });
        }

        public void UpdateConnPointsWorldPos(float angleDifference = 0f)
        {
            ConnPointCandidates.ForEach((connPointCandidate) =>
            {
                Quaternion newRotation = Quaternion.AngleAxis(connPointCandidate.Rotation.eulerAngles.y + angleDifference, Vector3.up);
                connPointCandidate.Rotation = newRotation;

                Vector3 localPosition = connPointCandidate.VoxelOwner.LocalPosition;
                connPointCandidate.WorldPosition = GetConnPointCandidateWorldPosition(localPosition, newRotation);
            });
        }

        private Vector3 GetConnPointCandidateWorldPosition(Vector3 localPosition, Quaternion rotation)
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