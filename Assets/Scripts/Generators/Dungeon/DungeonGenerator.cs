using System.Collections.Generic;
using Assets.Meta.Data;
using Assets.Scripts.Generators.Dungeon.Candidates;
using Assets.Scripts.Generators.Dungeon.Elements;
using Assets.Scripts.Generators.Meta.VoxelData;
using Assets.Scripts.Generators.Zone;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.DungeonGenerator
{
    public class DungeonGenerator : MonoBehaviour
    {
        public CandidatesFactory candidatesFactory;
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

            candidatesFactory.Init(transform, zonesGenerator);

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

            candidatesFactory.ReclaimAll();

            GenerateCandidates();
            BuildSpawnPointElement();
            BuildAllElements();
        }

        protected void GenerateCandidates()
        {
            DebugTimer.Start();

            //ClearStepVoxels();

            candidatesFactory.StartFactory(targetElementsCount);

            if (stepByStep) return;

            int infiniteLoopCheckCount = 0, infiniteLoopCheck = targetElementsCount << candidatesFactory.loopCheckCount;
            while (candidatesFactory.CandidateProduct.GetOpenCandidatesCount() > 0)
            {
                if (infiniteLoopCheckCount++ > infiniteLoopCheck)
                {
                    throw new System.Exception("DungeonGenerator::Dungeon generation takes too long. - Possible infinite loop.");
                }

                GenerateNextCandidate();

                if (stepByStep) break;
            }

            Debug.Log("DungeonGenerator::Generation completed : " + DebugTimer.Lap() + "ms");

            Debug.Log("DungeonGenerator::Elements targeted : " + targetElementsCount +
                    ". Candidates accepted : " + candidatesFactory.CandidateProduct.GetAcceptedCandidatesCount());
        }

        protected void GenerateNextCandidate()
        {
            if (candidatesFactory.CandidateProduct.GetOpenCandidatesCount() <= 0)
            {
                throw new System.Exception("DungeonGenerator::No open set.");
            }

            candidatesFactory.CandidateProduct.GenerateNextCandidate();
        }

        protected void BuildSpawnPointElement()
        {
            Candidate startCandidate = candidatesFactory.CandidateProduct.GetFirstAcceptedCandidate();
            GameObject randomSpawnGO = Instantiate(startCandidate.GameObject, this.gameObject.transform);

            Element spawnElement = randomSpawnGO.GetComponent<Element>();
            candidatesFactory.initializedElements.Add(startCandidate.WorldPosition, spawnElement);
            Volume spawnVolume = randomSpawnGO.GetComponent<Volume>();
            spawnVolume.RecalculateBounds();
        }

        protected void BuildLastStepElement()
        {
            Candidate lastCandidate = candidatesFactory.CandidateProduct.GetLastAcceptedCandidate();
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
            DebugTimer.Start();

            List<Candidate> acceptedCandidates = candidatesFactory.CandidateProduct.GetAllAcceptedCandidates();
            int count = acceptedCandidates.Count;
            for (int i = 1; i < count; i++)
            {
                Candidate acceptedCandidate = acceptedCandidates[i];
                BuildElement(acceptedCandidate);
            }

            Debug.Log("DungeonGenerator::Generated Elements Initialized : " + DebugTimer.Lap() + "ms");
        }

        /// <summary>
        /// Creates a dungeon room <b>GameObject</b> from a accepted <b>Candidate</b> object.    
        /// If it is possible the object will be pooled, else it is fully initialized.
        /// </summary>
        /// <param name="acceptedCandidate">Object that defines the new element. Objects are usually provided by <b>Elements Factory</b>.</param>
        protected void BuildElement(Candidate acceptedCandidate)
        {
            CandidatesConnection candidatesConnection = acceptedCandidate.CandidatesConnection;
            Element previousElement = candidatesFactory.initializedElements[candidatesConnection.LastCandidateWorldPos];

            ConnectionPoint lastConnPoint = GetConnPointAtPosition(
                previousElement.connectionPoints, acceptedCandidate.LastConnPointCandidate.LocalPosition);

            ConnectionPoint newConnPoint = candidatesFactory.GetNewElement(acceptedCandidate, transform);

            newConnPoint.sharedConnPoint =
                lastConnPoint != null ? lastConnPoint : throw new System.Exception("DungeonGenerator:: No PREVIOUS ELEMENT connection point found.");

            lastConnPoint.sharedConnPoint = newConnPoint;
        }

        protected static ConnectionPoint GetConnPointAtPosition(List<ConnectionPoint> connPoints, Vector3 localPosition)
        {
            int connPointsCount = connPoints.Count;
            for (int j = 0; j < connPointsCount; j++)
            {
                ConnectionPoint connPoint = connPoints[j];

                if (connPoint.transform.localPosition == localPosition)
                {
                    return connPoint;
                }
            }

            return null;
        }

        static void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(new Vector3(10f, 0f, 10f), new Vector3(30f, 15f, 30f));
        }
    }
}
