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

        public Vector3[] FloorVoxelWorldPositions => floor.GetVoxelsWorldPositions();
        //public Vector3[] WallsVoxelWorldPositions => walls.First().GetVoxelsWorldPositions();

        public HashSet<Vector3> WallsVoxelMap { get; private set; }
        public HashSet<Vector3> DoorsVoxelMap { get; private set; }

        private List<RoomElement> roomElements;

        /*public GameObject GetWallVoxelGO(Vector3 worldPosition)
        {
            return wallsVoxelGoMap[worldPosition];
        }

        public GameObject GetDoorVoxelGO(Vector3 worldPosition)
        {
            return doorsVoxelGoMap[worldPosition];
        }*/

        private void Start()
        {
            //Init();
        }

        public void InitVoxelMaps()
        {
            //RecalculateAll();
            InitWallsVoxelMap();
            //InitDoorsVoxelMap();
        }

        [ContextMenu("Recalculate All")]
        public void RecalculateAll()
        {
            InitRoomElements();
            //RecalculateWorldPosition();
            //RecalculateBounds();
        }

        private void InitRoomElements()
        {
            int wallsCount = walls.Count;
            int doorsCount = doors.Count;

            roomElements = new List<RoomElement>(wallsCount + doorsCount + 1)
            {
                floor
            };

            roomElements.AddRange(walls);
            roomElements.AddRange(doors);
        }

        //TODO: Investigate why this is needed
        /*
        [ContextMenu("Recalculate World Position")]
        public void RecalculateWorldPosition()
        {
            int roomElementsCount = roomElements.Count;
            for (int i = 0; i < roomElementsCount; i++)
            {
                RoomElement elements = roomElements[i];
                Volume volume = elements.GetComponent<Volume>();
                volume.RecalculateVoxelsWorldSpace();
            }
        }*/

        //TODO: Investigate why this is needed
        /*
        [ContextMenu("Recalculate Bounds")]
        public void RecalculateBounds()
        {
            int roomElementsCount = roomElements.Count;
            for (int i = 0; i < roomElementsCount; i++)
            {
                RoomElement elements = roomElements[i];
                Volume volume = elements.GetComponent<Volume>();
                volume.RecalculateBounds();
            }
        }*/

        private void InitWallsVoxelMap()
        {
            WallsVoxelMap = GetVoxelMap(walls);

            /*foreach (var (wall, index) in walls.Select((wall, index) => (wall, index)))
            {
                if (wall == null)
                {
                    Debug.Log("Wall '" + index + "' is not set. Skipping it from the voxels map.");
                    continue;
                }

                Vector3[] worldPositions = wall.GetVoxelsWorldPositions();
                WallsVoxelMap.UnionWith(worldPositions);
            }*/
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

        private void InitDoorsVoxelMap()
        {
            DoorsVoxelMap = new HashSet<Vector3>();

            doors.ForEach(door =>
            {
                Vector3[] worldPositions = door.GetVoxelsWorldPositions();
                DoorsVoxelMap.UnionWith(worldPositions);
            });


            /*
            int doorsCount = doors.Count;
            for (int i = 0; i < doorsCount; i++)
            {
                RoomElement door = doors[i];

                Voxel[] doorVoxels = door.Voxels;
                GameObject[] doorVoxelGOs = door.VoxelGOs;

                int wallsVoxelLength = doorVoxels.Length;
                for (int j = 0; j < wallsVoxelLength; j++)
                {
                    Vector3 voxelWorldPosition = doorVoxels[j].WorldPosition;

                    doorsVoxelMap.Add(voxelWorldPosition);
                    doorsVoxelGoMap.Add(voxelWorldPosition, doorVoxelGOs[j]);
                }
            }
            */
        }

        /*[ContextMenu("Toggle Gizmo Mode")]
        public void ToggleGizmoToDraw()
        {
            Volume.drawVolume = !Volume.drawVolume;
        }

        private void OnDrawGizmos()
        {
            RoomElement.DrawEndPoint(floor.endPoint, floor.transform.rotation);
        }*/
    }
}