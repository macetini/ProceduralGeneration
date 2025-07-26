using UnityEngine;

namespace Assets.Scripts.DungeonGenerator.Data
{
    public class Voxel : MonoBehaviour
    {
        public static readonly string NAME = "Voxel";
        public Vector3 worldPosition;

        public static void SetGameObjectName(GameObject voxelGO, Vector3 worldPosition)
        {
            voxelGO.name = NAME + " - " + voxelGO.transform.localPosition + " - " + worldPosition;
        }
    }
}
