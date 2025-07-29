using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.VoxelData;
using Assets.Scripts.DungeonGenerator.Utils;
using Assets.Scripts.RoomGenerator.Conditions;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator
{
    public class RoomGenerator : MonoBehaviour
    {
        private const int INFINITE_LOOP_CHECK_MAX_COUNT = 100000;
        private readonly HashSet<Vector3> acceptedItemVoxels = new();
        private DRandom random;

        public RoomBlueprint blueprint;
        public RoomElement roomItemPrefab;

        private void Start()
        {
            random = new DRandom();
            random.Init(Random.Range(0, int.MaxValue));

            //PositionOnFloor();

            Instantiate(blueprint); //TODO - HAS TO BE OPTIMIZED.
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
            List<Voxel> floorVoxels = blueprint.FloorVoxels;
            List<int> floorVoxelsIndexList = GetFloorVoxelsIndexList(floorVoxels);

            roomItemPrefab.Init();

            Voxel acceptedFloorVoxel = null;

            ConditionData conditionData;
            conditionData.endPointDirection = DirectionType.FORWARD;
            conditionData.blueprint = blueprint;
            conditionData.roomItemPrefab = roomItemPrefab;
            conditionData.takenVoxels = acceptedItemVoxels;

            int infiniteLoopCheckCountOuter = 0;// infiniteLoopCheck = 100000;//targetElementsCount << elementsFactory.loopCheckCount;
            do
            {
                if (infiniteLoopCheckCountOuter++ > INFINITE_LOOP_CHECK_MAX_COUNT)
                {
                    throw new System.Exception("RoomGenerator::Room generation takes too long. - Possible infinite loop.");
                }

                int floorVoxelsIndexListCount = floorVoxelsIndexList.Count;
                if (floorVoxelsIndexListCount == 0) break;

                int randomFloorVoxelIndex = Random.Range(0, floorVoxelsIndexListCount);
                randomFloorVoxelIndex = floorVoxelsIndexList[randomFloorVoxelIndex];
                floorVoxelsIndexList.Remove(randomFloorVoxelIndex);

                Voxel randomFloorVoxel = floorVoxels[randomFloorVoxelIndex];
                Vector3 randomFloorVoxelPos = randomFloorVoxel.WorldPosition;
                //itemWorldPositions = roomItemPrefab.GetOffsetVoxelPositions(randomFloorVoxelPos);

                conditionData.randomFloorVoxelPos = randomFloorVoxelPos;
                conditionData.endPointDirection = DirectionType.FORWARD;

                bool testPassed = false;

                int infiniteLoopCheckCountInner = 0;// infiniteLoopCheck2 = 100000;//targetElementsCount << elementsFactory.loopCheckCount;
                int endPointIndex = 0;
                int endPointsCount = roomItemPrefab.endPoint.directions.Count;
                do
                {
                    if (infiniteLoopCheckCountInner++ > INFINITE_LOOP_CHECK_MAX_COUNT)
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
            item.transform.SetPositionAndRotation(
                acceptedFloorVoxel.WorldPosition, Quaternion.AngleAxis((float)conditionData.endPointDirection, Vector3.up));

            InitializeNewItem(item);
        }

        private static List<int> GetFloorVoxelsIndexList(List<Voxel> floorVoxels)
        {
            int voxelsCount = floorVoxels.Count;
            List<int> floorVoxelsIndexList = new(voxelsCount);
            for (int i = 0; i < voxelsCount; i++)
            {
                floorVoxelsIndexList.Add(i);
            }
            
            return floorVoxelsIndexList;
        }

        private void InitializeNewItem(RoomElement item)
        {
            item.Volume.RecalculateVoxelsWorldSpace();

            int itemVoxelsLength = item.Voxels.Count;
            for (int i = 0; i < itemVoxelsLength; i++)
            {
                Voxel itemVoxel = item.Voxels[i];

                /*GameObject itemGO = item.VoxelGOs[i];
                Voxel.SetGameObjectName(itemGO, itemVoxel.WorldPosition);

                acceptedItemVoxels.Add(itemVoxel.WorldPosition);*/
            }
        }
    }
}