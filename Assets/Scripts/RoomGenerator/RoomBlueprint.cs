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

        private HashSet<Vector3> wallsVoxelMap;
        private HashSet<Vector3> doorsVoxelMap;

        public Vector3[] FloorVoxelWorldPositions => floor.GetVoxelsWorldPositions();
        public Vector3[] WallsVoxelWorldPositions => walls.First().GetVoxelsWorldPositions();

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
            Init();
        }

        protected void Init()
        {
            //RecalculateAll();
            InitWallsVoxelMap();
            InitDoorsVoxelMap();
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
            //int doorsCount = doors.Count;

            roomElements = new List<RoomElement>(wallsCount + 1)//(wallsCount + doorsCount + 1)
            {
                floor
            };

            roomElements.AddRange(walls);

            /*for (int i = 0; i < doorsCount; i++)
            {
                roomElements.Add(doors[i]);
            }*/
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
            wallsVoxelMap = new HashSet<Vector3>();
            //wallsVoxelGoMap = new Dictionary<Vector3, GameObject>();

            walls.ForEach(wall =>
            {
                Vector3[] worldPositions = wall.GetVoxelsWorldPositions();
                wallsVoxelMap.UnionWith(worldPositions);
            });

            /*
            int wallsCount = walls.Count;
            for (int i = 0; i < wallsCount; i++)
            {
                RoomElement wall = walls[i];

                Voxel[] wallVoxels = wall.Voxels;
                //GameObject[] wallVoxelGOs = wall.VoxelGOs;

                int wallsVoxelLength = wallVoxels.Length;
                for (int j = 0; j < wallsVoxelLength; j++)
                {
                    Vector3 voxelWorldPosition = wallVoxels[j].WorldPosition;

                    wallsVoxelMap.Add(voxelWorldPosition);
                    //wallsVoxelGoMap.Add(voxelWorldPosition, wallVoxelGOs[j]);
                }
            }
            */
        }

        private void InitDoorsVoxelMap()
        {
            doorsVoxelMap = new HashSet<Vector3>();

            doors.ForEach(door =>
            {
                Vector3[] worldPositions = door.GetVoxelsWorldPositions();
                doorsVoxelMap.UnionWith(worldPositions);
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