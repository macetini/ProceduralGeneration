using System.Collections.Generic;
using Assets.Meta.Data.Products;
using Assets.Meta.Sets;
using Assets.Scripts.Generators.Zone;
using Assets.Scripts.Generators.Dungeon.Candidates;
using Assets.Scripts.Generators.Meta.VoxelData;
using Assets.Scripts.Generators.Dungeon.Elements;
using Assets.Scripts.Utils;
using UnityEngine;
using Assets.Scripts.Generators.Dungeon.Points;

namespace Assets.Meta.Data
{
    //TODO - Transfer to Scripts.
    [CreateAssetMenu(fileName = "CandidatesFactory", menuName = "Candidates Factory", order = 1)]
    public class CandidatesFactory : ScriptableObject
    {
        public List<DungeonSet> Sets = new();
        public int loopCheckCount = 5;
        public bool randomizeOnStart = true;

        public bool recycle = true;

        [Range(0f, 1.0f)]
        public float closingElemsPct = 0.7f;

        //TODO: move to root of globals (try to re factor)
        private int targetElementsCount;
        private int seed = 0;
        private Transform parentTransform;
        private CandidateProduct candidateProduct;
        private Dictionary<string, List<GameObject>> pools;

        private CandidatesManager candidatesManager;

        public Dictionary<Vector3, Element> initializedElements = new();

        private List<GameObject> activePoolObjects = new();

        public int TargetElementsCount { get => targetElementsCount; }
        public int Seed { get => seed; }
        public Transform ParentTransform { get => parentTransform; }

        public CandidateProduct CandidateProduct { get => candidateProduct; }

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
            candidateProduct = new CandidateProduct(candidatesManager);
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

            if (initializedElements.Count > 0)
            {
                Destroy(initializedElements[Vector3.zero].gameObject);
            }

            while (activePoolObjects.Count > 0)
            {
                Reclaim(activePoolObjects[activePoolObjects.Count - 1]);
            }

            activePoolObjects = new List<GameObject>();

            initializedElements = new Dictionary<Vector3, Element>();
            candidatesManager.CleanUp();
        }

        public ConnectionPoint GetNewElement(Candidate newCandidate, Transform transform)
        {
            GameObject newCandidateGO = recycle ? TryToPoolCandidateGO(newCandidate, transform) : InstantiateCandidateGO(newCandidate, transform);

            ConnectionPoint newConnPoint = InitCandidateElementComponent(newCandidateGO, newCandidate);

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

                newCandidateGO.transform.position = newCandidate.WorldPosition;
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

        protected static GameObject InstantiateCandidateGO(Candidate newCandidate, Transform transform)
        {
            GameObject newCandidateGO = Instantiate
                (
                    newCandidate.GameObject,
                    newCandidate.WorldPosition,
                    newCandidate.Rotation,
                    transform
                );

            return newCandidateGO;
        }

        protected ConnectionPoint InitCandidateElementComponent(GameObject newCandidateGO, Candidate newCandidate)
        {
            Element newElement = newCandidateGO.GetComponent<Element>();
            ConnectionPoint newConnPoint = null;

            int connPointsCount = newElement.connectionPoints.Count;
            for (int i = 0; i < connPointsCount; i++)
            {
                ConnectionPoint connPoint = newElement.connectionPoints[i];
                connPoint.Open = newCandidate.ConnPointCandidates[i].Open;
                connPoint.transform.rotation = newCandidate.ConnPointCandidates[i].Rotation;

                if (connPoint.transform.localPosition == newCandidate.NewConnPointCandidate.LocalPosition)
                {
                    newConnPoint = connPoint;
                }
            }

            if (newConnPoint == null) throw new System.Exception("DungeonGenerator:: No NEW ELEMENT connection point found.");

            initializedElements.Add(newCandidate.WorldPosition, newElement);

            return newConnPoint;
        }

        protected void InitCandidateVolumeComponent(GameObject newCandidateGO, Candidate newCandidate)
        {
            Volume newVolume = newCandidateGO.GetComponent<Volume>();
            newVolume.RecalculateBounds();

            List<Voxel> newElementVoxels = newVolume.Voxels;
            int voxelsCount = newVolume.Voxels.Count;
            for (int i = 0; i < voxelsCount; i++)
            {
                Voxel voxel = newElementVoxels[i];
                Vector3 worldPosition = newCandidate.GetVoxelWorldPosition(voxel.transform.localPosition);

                InitializeGlobalVoxel(worldPosition, voxel);
            }
        }

        protected void InitializeGlobalVoxel(Vector3 voxelWorldPosition, Voxel voxel)
        {
            //Voxel voxel = voxelGO.GetComponent<Voxel>();
            //voxelWorldPos = voxelWorldPos.RoundVec3ToInt();

            if (candidatesManager.CandidateVoxels.ContainsKey(voxelWorldPosition))
            {
                candidatesManager.CandidateVoxels[voxelWorldPosition] = voxel;
                //voxel.WorldPosition = voxelWorldPos;
                voxel.SetWorldPosition(voxelWorldPosition);
            }
            else
            {
                Debug.LogError("ElementsFactory::Voxel GameObject we're trying to add to globalVoxels is NOT defined: " + voxelWorldPosition.ToString());
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
                elementToRecycle.SetActive(false);
            }
            else
            {
                Destroy(elementToRecycle);
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
    }
}