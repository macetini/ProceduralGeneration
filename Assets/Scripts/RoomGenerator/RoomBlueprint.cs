using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.Data;
using Assets.Scripts.DungeonGenerator.Elements;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator
{
    public class RoomBlueprint : MonoBehaviour
    {
        public new string name;

        private List<RoomElement> roomElements;

        public RoomElement floor;
        public List<RoomElement> walls;
        public List<RoomElement> doors;

        private HashSet<Vector3> wallsVoxelMap;
        private Dictionary<Vector3, GameObject> wallsVoxelGoMap;

        private HashSet<Vector3> doorsVoxelMap;
        private Dictionary<Vector3, GameObject> doorsVoxelGoMap;

        public Voxel[] FloorVoxels => floor.Voxels;

        public HashSet<Vector3> WallsVoxelMap => wallsVoxelMap;
        public HashSet<Vector3> DoorsVoxelMap => doorsVoxelMap;

        public GameObject GetWallVoxelGO(Vector3 worldPosition)
        {
            return wallsVoxelGoMap[worldPosition];
        }

        public GameObject GetDoorVoxelGO(Vector3 worldPosition)
        {
            return doorsVoxelGoMap[worldPosition];
        }

        private void Start()
        {
            Init();
        }

        protected void Init()
        {
            RecalculateAll();
            InitWallsVoxelMap();
            InitDoorsVoxelMap();
        }

        [ContextMenu("Recalculate All")]
        public void RecalculateAll()
        {
            InitRoomElements();
            RecalculateWorldPosition();
            RecalculateBounds();
        }

        private void InitRoomElements()
        {
            int wallsCount = walls.Count;
            int doorsCount = doors.Count;

            roomElements = new List<RoomElement>(wallsCount + doorsCount + 1)
        {
            floor
        };

            for (int i = 0; i < wallsCount; i++)
            {
                roomElements.Add(walls[i]);
            }

            for (int i = 0; i < doorsCount; i++)
            {
                roomElements.Add(doors[i]);
            }
        }

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
        }

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
        }

        private void InitWallsVoxelMap()
        {
            wallsVoxelMap = new HashSet<Vector3>();
            wallsVoxelGoMap = new Dictionary<Vector3, GameObject>();

            int wallsCount = walls.Count;
            for (int i = 0; i < wallsCount; i++)
            {
                RoomElement wall = walls[i];

                Voxel[] wallVoxels = wall.Voxels;
                GameObject[] wallVoxelGOs = wall.VoxelGOs;

                int wallsVoxelLength = wallVoxels.Length;
                for (int j = 0; j < wallsVoxelLength; j++)
                {
                    Vector3 voxelWorldPosition = wallVoxels[j].WorldPosition;

                    wallsVoxelMap.Add(voxelWorldPosition);
                    wallsVoxelGoMap.Add(voxelWorldPosition, wallVoxelGOs[j]);
                }
            }
        }

        private void InitDoorsVoxelMap()
        {
            doorsVoxelMap = new HashSet<Vector3>();
            doorsVoxelGoMap = new Dictionary<Vector3, GameObject>();

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
        }

        [ContextMenu("Toggle Gizmo Mode")]
        public void ToggleGizmoToDraw()
        {
            Volume.drawVolume = !Volume.drawVolume;
        }
    }
}