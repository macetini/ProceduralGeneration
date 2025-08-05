using System.Collections.Generic;
using Assets.Scripts.Utils;
using Assets.Scripts.Generators.Node.Conditions;
using UnityEngine;
using Assets.Scripts.Generators.Node.Conditions.Meta;
using Assets.Scripts.Generators.Node.Points.Meta;
using System.Linq;

namespace Assets.Scripts.Generators.Node
{
    public class NodeGeneration : MonoBehaviour
    {
        private readonly HashSet<Vector3> acceptedVoxelWorldPositions = new();

        private DRandom random;
        public NodeBlueprint BlueprintPrefab;
        public Node RoomItemPrefab; //TODO - This should be in Blueprint or Factory.

        private NodeBlueprint blueprint;

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

            //TODO - This should not be here, create a factory.
            RoomItemPrefab.InitConditionData();
            blueprint.Init();
          
            ConditionData conditionData = GetInitConditionData();

            Vector3 acceptedVoxelPosition = Vector3Extensions.NaN;

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

            if (acceptedVoxelPosition.IsNaN())
            {
                Debug.Log("RoomGenerator:: No more space in room. Room Item discarded.");
            }
            else
            {
                Debug.Log("RoomGenerator:: Room Item accepted: " + acceptedVoxelPosition + " | " + conditionData.endPointRotation);
                AcceptNewRoomItem(RoomItemPrefab, conditionData);
            }
        }

        private ConditionData GetInitConditionData()
        { 
            return new()
            {
                blueprint = blueprint,
                roomItemPrefab = RoomItemPrefab,
                randomFloorVoxelPosition = Vector3Extensions.NaN,
                takenVoxels = acceptedVoxelWorldPositions,
                endPointRotation = RotationData.DEGREES_0
            };
        }

        private void AcceptNewRoomItem(Node newRoomPrefab, ConditionData conditionData)
        {
            Node newRoomInstance = Instantiate(newRoomPrefab, blueprint.transform);

            newRoomInstance.transform.SetPositionAndRotation(
                conditionData.randomFloorVoxelPosition,
                Quaternion.AngleAxis((float)conditionData.endPointRotation, Vector3.up));

            newRoomInstance.Volume.RecalculateVoxelsWorldPosition();

            acceptedVoxelWorldPositions.UnionWith(newRoomInstance.GetVoxelsWorldPositions());
        }
    }
}