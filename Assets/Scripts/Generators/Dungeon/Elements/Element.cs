using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Generators.Meta.VoxelData;
using Assets.Scripts.Utils;
using Assets.Scripts.Generators.Dungeon.Points;

namespace Assets.Scripts.Generators.Dungeon.Elements
{
    public class Element : MonoBehaviour
    {
        public List<ConnectionPoint> connectionPoints;
        public ElementType type;
        public string ID;

        public ConnectionPoint GetRandomOpenConnPoint(DRandom random)
        {
            connectionPoints.Shuffle(random.random);

            foreach (ConnectionPoint connectionPoint in connectionPoints)
            {
                if (connectionPoint.Open)
                {
                    return connectionPoint;
                }
            }

            Debug.Log("Room::GetRandomOpenConnPoint() - No open connection points.");
            return null;
        }

        public bool HasOpenConnection()
        {
            foreach (ConnectionPoint connectionPoint in connectionPoints)
            {
                if (connectionPoint.Open)
                {
                    return true;
                }
            }

            return false;
        }

        public ConnectionPoint GetFirstOpenConnectionPoint()
        {
            foreach (ConnectionPoint connectionPoint in connectionPoints)
            {
                if (connectionPoint.Open) return connectionPoint;
            }

            Debug.Log("Room::GetFirstOpenConnPoint() - No open connection points.");

            return null;
        }

        public Dictionary<Voxel, Vector3> GetConnPointsVoxelWorldPosition(Vector3 translation = default(Vector3), Quaternion rotation = default(Quaternion))
        {
            Dictionary<Voxel, Vector3> connPointVoxelWorldPositions = new(connectionPoints.Count);

            foreach (ConnectionPoint connectionPoint in connectionPoints)
            {
                Voxel voxelOwner = connectionPoint.voxelOwner;
                Vector3 newPosition = rotation * voxelOwner.LocalPosition + translation;

                connPointVoxelWorldPositions[voxelOwner] = newPosition;
            }

            return connPointVoxelWorldPositions;
        }

        private void OnDrawGizmos()
        {
            foreach (ConnectionPoint connectionPoint in connectionPoints)
            {
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
    }
}