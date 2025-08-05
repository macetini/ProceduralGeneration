using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.Utils;
using Assets.Scripts.RoomGenerator.Conditions;
using UnityEngine;
using Assets.Scripts.RoomGenerator.Conditions.Meta;
using Assets.Scripts.RoomGenerator.Points.Meta;
using System.Linq;

namespace Assets.Scripts.RoomGenerator
{
    public class RoomGenerator : MonoBehaviour
    {
        private readonly HashSet<Vector3> acceptedVoxelWorldPositions = new();

        private DRandom random;
        public RoomBlueprint BlueprintPrefab;
        public RoomElement RoomItemPrefab; //TODO - This should be in Blueprint.

        private RoomBlueprint blueprint;

        private void Start()
        {
            random = new DRandom();
            random.Init(Random.Range(0, int.MaxValue));

            blueprint = Instantiate(BlueprintPrefab); //TODO - This should be a factory.
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
            Vector3[] floorVoxelsWorldPositions = blueprint.FloorVoxelsWorldPositions;
            if (floorVoxelsWorldPositions.Length == 0)
            {
                string msg = "RoomGenerator:: Room has no floor voxels. Room name: " + blueprint.name;
                throw new System.Exception(msg);
            }

            //TODO - This should not be here, organize it better.
            RoomItemPrefab.InitConditionData();
            blueprint.Init();

            //TODO - Put in separate method.            
            ConditionData conditionData = new()
            {
                blueprint = blueprint,
                roomItemPrefab = RoomItemPrefab,
                randomFloorVoxelPosition = Vector3.positiveInfinity,
                takenVoxels = acceptedVoxelWorldPositions,
                endPointRotation = RotationData.DEGREES_0
            };

            //TODO - Maybe there is a better way to check for undefined vector?
            Vector3 acceptedVoxelPosition = Vector3.positiveInfinity;

            List<Vector3> floorVoxelPositionsList = floorVoxelsWorldPositions.ToList();
            floorVoxelPositionsList.Shuffle();

            //TODO - Refactor, atomize this.
            foreach (Vector3 randomFloorVoxelPosition in floorVoxelPositionsList)
            {
                Debug.Log("RoomGenerator:: Random floor voxel position: " + randomFloorVoxelPosition);

                conditionData.randomFloorVoxelPosition = randomFloorVoxelPosition;

                if (RoomItemPrefab.generationConditions == null || RoomItemPrefab.generationConditions.Count == 0)
                {
                    Debug.LogWarning("RoomGenerator:: Room Item has no generation conditions. First random Voxel accepted.");
                    acceptedVoxelPosition = randomFloorVoxelPosition;
                    break;
                }

                List<RotationData> endPointRotations = RoomItemPrefab.endPoint.Rotations;
                endPointRotations.Shuffle();

                bool testPassed = false;
                foreach (RotationData endPointRotation in endPointRotations)
                {
                    testPassed = false;
                    conditionData.endPointRotation = endPointRotation;

                    foreach (GenerationCondition condition in RoomItemPrefab.generationConditions)
                    {
                        testPassed = condition.Test(conditionData);
                        if (!testPassed) break;
                    }

                    if (testPassed) break;
                }

                if (testPassed)
                {
                    acceptedVoxelPosition = randomFloorVoxelPosition;
                    break;
                }
            }

            if (acceptedVoxelPosition.Equals(Vector3.positiveInfinity))
            {
                Debug.Log("RoomGenerator:: No more space in room. Room Item discarded.");
            }
            else
            {
                Debug.Log("RoomGenerator:: Room Item accepted: " + acceptedVoxelPosition + " | " + conditionData.endPointRotation);
                AcceptNewRoomItem(RoomItemPrefab, conditionData);
            }
        }

        private void AcceptNewRoomItem(RoomElement newRoomPrefab, ConditionData conditionData)
        {
            RoomElement newRoomInstance = Instantiate(newRoomPrefab, blueprint.transform);

            newRoomInstance.transform.SetPositionAndRotation(
                conditionData.randomFloorVoxelPosition,
                Quaternion.AngleAxis((float)conditionData.endPointRotation, Vector3.up));

            newRoomInstance.Volume.RecalculateVoxelsWorldPosition();

            acceptedVoxelWorldPositions.UnionWith(newRoomInstance.GetVoxelsWorldPositions());
        }
    }
}