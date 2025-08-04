using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator
{
    public class RoomBlueprint : MonoBehaviour
    {
        public new string name;
        public RoomElement floor;
        public RoomElement ceiling;
        public List<RoomElement> walls;
        public List<RoomElement> doors;
        public List<RoomElement> windows;
        public List<RoomElement> allRoomElements;

        public Vector3[] FloorVoxelsWorldPositions => floor.GetVoxelsWorldPositions();

        public HashSet<Vector3> WallsVoxelMap { get; private set; }
        public HashSet<Vector3> DoorsVoxelMap { get; private set; }


        public void Init()
        {
            InitAllRoomElements();

            WallsVoxelMap = GetVoxelMap(walls);
            DoorsVoxelMap = GetVoxelMap(doors);
        }


        private void InitAllRoomElements()
        {
            int wallsCount = walls.Count;
            int doorsCount = doors.Count;

            allRoomElements = new List<RoomElement>(wallsCount + doorsCount + 1)
            {
                floor
            };

            allRoomElements.AddRange(walls);
            allRoomElements.AddRange(doors);
        }

        private static HashSet<Vector3> GetVoxelMap(List<RoomElement> roomElements)
        {
            HashSet<Vector3> voxelMap = new();

            foreach (var (roomElement, index) in roomElements.Select((roomElement, index) => (roomElement, index)))
            {
                if (roomElement == null)
                {
                    Debug.Log("Room element at Index: '" + index + "' is not set. Skipping it from the voxels map.");
                    continue;
                }

                Vector3[] worldPositions = roomElement.GetVoxelsWorldPositions();
                voxelMap.UnionWith(worldPositions);
            }

            return voxelMap;
        }
    }
}