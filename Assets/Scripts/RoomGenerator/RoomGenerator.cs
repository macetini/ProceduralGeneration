using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.VoxelData;
using Assets.Scripts.DungeonGenerator.Utils;
using Assets.Scripts.RoomGenerator.Conditions;
using UnityEngine;
using Assets.Scripts.RoomGenerator.Conditions.Meta;

namespace Assets.Scripts.RoomGenerator
{
    public class RoomGenerator : MonoBehaviour
    {
        private const int INFINITE_LOOP_CHECK_MAX_COUNT = 100000;

        private readonly HashSet<Vector3> acceptedItemVoxels = new();
        
        private DRandom random;
        public RoomBlueprint blueprint;
        public RoomElement roomItemPrefab; //TODO - This should be in Blueprint

        private void Start()
        {
            random = new DRandom();
            random.Init(Random.Range(0, int.MaxValue));

            Instantiate(blueprint); //TODO - HAS TO BE OPTIMIZED.
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PositionRoomItemOnFloor();
            }
        }

        //TODO - refactor into multiple methods
        void PositionRoomItemOnFloor()
        {
            Vector3[] floorVoxelPositions = blueprint.FloorVoxelWorldPositions;
            int voxelPositionsCount = floorVoxelPositions.Length;
            if (voxelPositionsCount == 0)
            {
                string msg = "RoomGenerator:: Room has no floor voxels. Room name: " + blueprint.name;
                throw new System.Exception(msg);
            }

            //TODO - This should not be here.
            roomItemPrefab.InitConditionData();

            //TODO - Maybe there is a better way to check for undefined vector?
            Vector3 acceptedVoxelPosition = Vector3.positiveInfinity;

            //TODO - Put in separate method.
            ConditionData conditionData;
            conditionData.endPointDirection = DirectionType.FORWARD;
            conditionData.blueprint = blueprint;
            conditionData.roomItemPrefab = roomItemPrefab;
            conditionData.takenVoxels = acceptedItemVoxels;

            //TODO - optimize (refactor)
            int infiniteLoopCheckCountOuter = 0;
            do
            {
                if (infiniteLoopCheckCountOuter++ > INFINITE_LOOP_CHECK_MAX_COUNT)
                {
                    throw new System.Exception("RoomGenerator:: Room generation takes too long. - Possible infinite loop.");
                }

                int randomFloorPositionIndex = random.RangeInt(0, voxelPositionsCount--);
                Vector3 randomFloorVoxelPosition = floorVoxelPositions[randomFloorPositionIndex];
                floorVoxelPositions.RemoveFromArray(randomFloorVoxelPosition);

                Debug.Log("RoomGenerator:: Random floor voxel position: " + randomFloorVoxelPosition);

                conditionData.randomFloorVoxelPos = randomFloorVoxelPosition;
                conditionData.endPointDirection = DirectionType.FORWARD;

                bool testPassed = false;

                if (roomItemPrefab.generationConditions == null || roomItemPrefab.generationConditions.Count == 0)
                {
                    Debug.LogWarning("RoomGenerator:: Room Item has no generation conditions. First random Voxel accepted.");
                    acceptedVoxelPosition = randomFloorVoxelPosition;
                    break;
                }

                int infiniteLoopCheckCountInner = 0;
                int endPointIndex = 0;
                int endPointsCount = roomItemPrefab.endPoint.directions.Count;
                do
                {
                    if (infiniteLoopCheckCountInner++ > INFINITE_LOOP_CHECK_MAX_COUNT)
                    {
                        throw new System.Exception("RoomGenerator:: Room item rotation takes too long. - Possible infinite loop.");
                    }

                    conditionData.endPointDirection = roomItemPrefab.endPoint.directions[endPointIndex];

                    List<GenerationCondition> generationConditions = roomItemPrefab.generationConditions;
                    foreach (GenerationCondition condition in generationConditions)
                    {
                        testPassed = condition.Test(conditionData);
                        if (!testPassed) break;
                    }

                    if (!testPassed) endPointIndex++;

                } while (!testPassed && endPointIndex < endPointsCount);

                if (!testPassed) continue;

                acceptedVoxelPosition = randomFloorVoxelPosition;
            }
            while (acceptedVoxelPosition.Equals(Vector3.positiveInfinity) && voxelPositionsCount > 0);

            if (acceptedVoxelPosition.Equals(Vector3.positiveInfinity))
            {
                Debug.Log("RoomGenerator:: No more space in room. Room Item discarded.");
                return;
            }

            RoomElement item = Instantiate(roomItemPrefab, gameObject.transform);
            item.transform.SetPositionAndRotation(
                acceptedVoxelPosition, Quaternion.AngleAxis((float)conditionData.endPointDirection, Vector3.up));

            //InitializeNewItem(item);
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

        //TODO - Investigate if needed.
        /*private void InitializeNewItem(RoomElement item)
        {
            item.Volume.RecalculateVoxelsWorldSpace();

            int itemVoxelsLength = item.Voxels.Count;
            for (int i = 0; i < itemVoxelsLength; i++)
            {
                Voxel itemVoxel = item.Voxels[i];

                GameObject itemGO = item.VoxelGOs[i];
                Voxel.SetGameObjectName(itemGO, itemVoxel.WorldPosition);

                acceptedItemVoxels.Add(itemVoxel.WorldPosition);
            }
        }*/
    }
}