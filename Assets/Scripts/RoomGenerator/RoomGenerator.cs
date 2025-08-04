using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.Utils;
using Assets.Scripts.RoomGenerator.Conditions;
using UnityEngine;
using Assets.Scripts.RoomGenerator.Conditions.Meta;

namespace Assets.Scripts.RoomGenerator
{
    public class RoomGenerator : MonoBehaviour
    {
        private const int INFINITE_LOOP_CHECK_MAX_COUNT = 100000;

        private readonly HashSet<Vector3> acceptedVoxelWorldPositions = new();

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
            int voxelPositionsCount = floorVoxelPositions.Length - 1;
            if (voxelPositionsCount <= 0)
            {
                string msg = "RoomGenerator:: Room has no floor voxels. Room name: " + blueprint.name;
                throw new System.Exception(msg);
            }

            //TODO - This should not be here, organize it better.
            roomItemPrefab.InitConditionData();
            blueprint.InitVoxelMaps();

            //TODO - Maybe there is a better way to check for undefined vector?
            Vector3 acceptedVoxelPosition = Vector3.positiveInfinity;

            //TODO - Put in separate method.
            ConditionData conditionData = new()
            {
                endPointRotation = RotationType.DEGREES_0,
                blueprint = blueprint,
                roomItemPrefab = roomItemPrefab,
                takenVoxels = acceptedVoxelWorldPositions
            };

            //TODO - refactor into multiple methods
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

                conditionData.randomFloorVoxelPosition = randomFloorVoxelPosition;
                conditionData.endPointRotation = RotationType.DEGREES_0;

                if (roomItemPrefab.generationConditions == null || roomItemPrefab.generationConditions.Count == 0)
                {
                    Debug.LogWarning("RoomGenerator:: Room Item has no generation conditions. First random Voxel accepted.");
                    acceptedVoxelPosition = randomFloorVoxelPosition;
                    break;
                }

                int infiniteLoopCheckCountInner = 0;

                int endPointIndex = 0;
                int endPointsCount = roomItemPrefab.endPoint.Rotations.Count;
                List<RotationType> endPointRotations = roomItemPrefab.endPoint.Rotations;

                bool testPassed = false;
                do
                {
                    if (infiniteLoopCheckCountInner++ > INFINITE_LOOP_CHECK_MAX_COUNT)
                    {
                        throw new System.Exception("RoomGenerator:: Room Item condition Tests take too long. - Possible infinite loop.");
                    }

                    //TODO - Add shuffle.
                    conditionData.endPointRotation = endPointRotations[endPointIndex];

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
            else
            {
                Debug.Log("RoomGenerator:: Room Item accepted: " + acceptedVoxelPosition + " | " + conditionData.endPointRotation);
            }

            AcceptNewRoomItem(roomItemPrefab, conditionData);
        }

        private void AcceptNewRoomItem(RoomElement prefab, ConditionData conditionData)
        {
            RoomElement newRoom = Instantiate(prefab, transform);

            newRoom.transform.SetPositionAndRotation(
                conditionData.randomFloorVoxelPosition,
                Quaternion.AngleAxis((float)conditionData.endPointRotation, Vector3.up));

            newRoom.Volume.RecalculateVoxelsWorldPosition();

            acceptedVoxelWorldPositions.UnionWith(newRoom.GetVoxelsWorldPositions());
        }

        //TODO - Maybe remove?
        /*private static List<int> GetFloorVoxelsIndexList(List<Voxel> floorVoxels)
        {
            int voxelsCount = floorVoxels.Count;
            List<int> floorVoxelsIndexList = new(voxelsCount);
            for (int i = 0; i < voxelsCount; i++)
            {
                floorVoxelsIndexList.Add(i);
            }

            return floorVoxelsIndexList;
        }*/
    }
}