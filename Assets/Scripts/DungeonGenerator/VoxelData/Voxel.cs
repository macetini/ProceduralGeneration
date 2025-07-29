using UnityEngine;

namespace Assets.Scripts.DungeonGenerator.VoxelData
{
    public class Voxel : MonoBehaviour
    {
        public static readonly string NAME = "Voxel";
        public Vector3 WorldPosition { get; set; }

        public void SetLocalPositionLabel()
        {
            int x = (int)transform.localPosition.x;
            int y = (int)transform.localPosition.y;
            int z = (int)transform.localPosition.z;

            name = string.Format(NAME + " - {{ {0}, {1}, {2} }}", x, y, z);
        }

        public void SetGameObjectName(Vector3 worldPosition)
        {
            int x1 = (int)transform.localPosition.x;
            int y1 = (int)transform.localPosition.y;
            int z1 = (int)transform.localPosition.z;

            int x2 = (int)transform.localPosition.x;
            int y2 = (int)transform.localPosition.y;
            int z2 = (int)transform.localPosition.z;

            name = string.Format(NAME + " - {{ {0}, {1}, {2} }} , {{ {3}, {4}, {5} }}", x1, y1, z1, x2, y2, z2);
        }
    }
}
