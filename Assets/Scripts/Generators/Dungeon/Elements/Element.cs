using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.VoxelData;
using Assets.Scripts.Utils;

namespace Assets.Scripts.DungeonGenerator.Elements
{
    public class Element : MonoBehaviour
    {
        public List<ConnectionPoint> connectionPoints;

        public ElementType type;

        //TODO: FIX! FIX THIS!
        public string ID;

        //WARNING - when doing the dungeon gen we sometimes Instantiate a room, check if it will fit and if it doesn't
        //we IMMEDIATELY destroy it. Awake() is called with instantiation - Start() waits until the function returns.
        //SO to be safe, don't use Awake() if you don't have to. Put all enemy and room specific instantiation in Start()!
        void Awake()
        {
            //DungeonGenerator.roomsCalledStart++;
            //Debug.Log("AWAKE");
        }

        void Start()
        {
            //Debug.Log("START");
        }

        private void OnDrawGizmos()
        {            
            int connPointsCount = connectionPoints.Count;
            for (int i = 0; i < connPointsCount; i++)
            {
                ConnectionPoint connectionPoint = connectionPoints[i];
                if (connectionPoint == null)
                {
                    continue;
                }

                Gizmos.color = Color.red;

                Gizmos.DrawSphere(connectionPoint.transform.position, 0.1f);

                Gizmos.DrawRay(new Ray(connectionPoint.transform.position, connectionPoint.transform.right));

                Gizmos.color = Color.green;
                Gizmos.DrawRay(new Ray(connectionPoint.transform.position, connectionPoint.transform.up));

                Gizmos.color = Color.blue;
                Gizmos.DrawRay(new Ray(connectionPoint.transform.position, connectionPoint.transform.forward));
            }
        }

        public ConnectionPoint GetRandomOpenConnPoint(DRandom random)
        {
            connectionPoints.Shuffle(random.random);

            int connPointsCount = connectionPoints.Count;
            for (int i = 0; i < connPointsCount; i++)
            {
                if (connectionPoints[i].Open)
                {
                    return connectionPoints[i];
                }
            }

            Debug.Log("Room::GetRandomOpenConnPoint() - No open connection points.");
            return null;
        }

        public bool HasOpenConnection()
        {
            int connPointsCount = connectionPoints.Count;
            for (int i = 0; i < connPointsCount; i++)
            {
                if (connectionPoints[i].Open)
                {
                    return true;
                }
            }

            return false;
        }

        public ConnectionPoint GetFirstOpenConnectionPoint()
        {
            int connPointsCount = connectionPoints.Count;
            for (int i = 0; i < connPointsCount; i++)
            {
                if (connectionPoints[i].Open) return connectionPoints[i];
            }

            Debug.Log("Room::GetFirstOpenConnPoint() - No open connection points.");

            return null;
        }

        public Dictionary<Voxel, Vector3> GetConnPointsVoxelWorldPosition(Vector3 translation = default(Vector3), Quaternion rotation = default(Quaternion))
        {
            int connPointCount = connectionPoints.Count;
            Dictionary<Voxel, Vector3> connPointVoxelWorldPositions = new Dictionary<Voxel, Vector3>(connPointCount);

            for (int i = 0; i < connPointCount; i++)
            {
                ConnectionPoint connPoint = connectionPoints[i];
                Voxel voxelOwner = connPoint.voxelOwner;
                Vector3 newPosition = rotation * voxelOwner.transform.position + translation;

                connPointVoxelWorldPositions[voxelOwner] = newPosition;
            }

            return connPointVoxelWorldPositions;
        }
    }
}