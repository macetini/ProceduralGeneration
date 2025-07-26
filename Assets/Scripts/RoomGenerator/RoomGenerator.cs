using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.Data;
using Assets.Scripts.DungeonGenerator.Utils;
using Assets.Scripts.RoomGenerator.Conditions;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator
{
    public class RoomGenerator : MonoBehaviour
    {
        public RoomBlueprint blueprint;
        public RoomElement roomItemPrefab;
        private readonly HashSet<Vector3> acceptedItemVoxels = new();
        private DRandom random;

        private void Start()
        {
            random = new DRandom();
            random.Init(Random.Range(0, int.MaxValue));

            PositionOnFloor();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PositionOnFloor();
            }
        }

        void PositionOnFloor()
        {
            Voxel[] floorVoxels = blueprint.FloorVoxels;
            List<int> floorVoxelsIndexList = GetFloorVoxelsIndexList(floorVoxels);

            roomItemPrefab.Init();

            //Vector3[] itemWorldPositions = null;
            Voxel acceptedFloorVoxel = null;

            ConditionData conditionData;
            conditionData.endPointDirection = DirectionType.FORWARD;
            conditionData.blueprint = blueprint;
            conditionData.roomItemPrefab = roomItemPrefab;
            conditionData.takenVoxels = acceptedItemVoxels;

            int infiniteLoopCheckCount = 0, infiniteLoopCheck = 100000;//targetElementsCount << elementsFactory.loopCheckCount;
            do
            {
                if (infiniteLoopCheckCount++ > infiniteLoopCheck)
                {
                    throw new System.Exception("RoomGenerator::Room generation takes too long. - Possible infinite loop.");
                }

                if (floorVoxelsIndexList.Count == 0) break;

                int randomFloorVoxelIndex = Random.Range(0, floorVoxelsIndexList.Count);
                randomFloorVoxelIndex = floorVoxelsIndexList[randomFloorVoxelIndex];
                floorVoxelsIndexList.Remove(randomFloorVoxelIndex);

                Voxel randomFloorVoxel = floorVoxels[randomFloorVoxelIndex];
                Vector3 randomFloorVoxelPos = randomFloorVoxel.worldPosition;
                //itemWorldPositions = roomItemPrefab.GetOffsetVoxelPositions(randomFloorVoxelPos);          

                conditionData.randomFloorVoxelPos = randomFloorVoxelPos;
                conditionData.endPointDirection = DirectionType.FORWARD;

                bool testPassed = false;

                int infiniteLoopCheckCount2 = 0, infiniteLoopCheck2 = 100000;//targetElementsCount << elementsFactory.loopCheckCount;

                int endPointIndex = 0, endPointsCount = roomItemPrefab.endPoint.directions.Count;
                do
                {
                    if (infiniteLoopCheckCount2++ > infiniteLoopCheck2)
                    {
                        throw new System.Exception("RoomGenerator::Room item rotation takes too long. - Possible infinite loop.");
                    }

                    conditionData.endPointDirection = roomItemPrefab.endPoint.directions[endPointIndex];

                    List<GenerationCondition> generationConditions = roomItemPrefab.generationConditions;
                    int conditionsCount = generationConditions.Count;
                    for (int i = 0; i < conditionsCount; i++)
                    {
                        GenerationCondition condition = generationConditions[i];
                        testPassed = condition.Test(conditionData);

                        if (!testPassed) break;
                    }

                    if (!testPassed) endPointIndex++;//conditionData.rotationIndex++;                

                } while (!testPassed && endPointIndex < endPointsCount);

                /////

                if (!testPassed) continue;

                acceptedFloorVoxel = randomFloorVoxel;
            }
            while (acceptedFloorVoxel == null && floorVoxelsIndexList.Count > 0);

            if (acceptedFloorVoxel == null)
            {
                Debug.Log("No more space in room");
                return;
            }

            RoomElement item = Instantiate(roomItemPrefab, gameObject.transform);
            item.transform.position = acceptedFloorVoxel.worldPosition;
            item.transform.rotation = Quaternion.AngleAxis((float)conditionData.endPointDirection, Vector3.up);

            InitializeNewItem(item);
        }

        private static List<int> GetFloorVoxelsIndexList(Voxel[] floorVoxels)
        {
            int voxelsCount = floorVoxels.Length;
            List<int> floorVoxelsIndexList = new List<int>(voxelsCount);
            for (int i = 0; i < voxelsCount; i++)
            {
                floorVoxelsIndexList.Add(i);
            }

            return floorVoxelsIndexList;
        }

        private void InitializeNewItem(RoomElement item)
        {
            item.Volume.RecalculateVoxelsWorldSpace();

            int itemVoxelsLength = item.Voxels.Length;
            for (int i = 0; i < itemVoxelsLength; i++)
            {
                Voxel itemVoxel = item.Voxels[i];

                GameObject itemGO = item.VoxelGOs[i];
                Voxel.SetGameObjectName(itemGO, itemVoxel.worldPosition);

                acceptedItemVoxels.Add(itemVoxel.worldPosition);
            }
        }
    }
}