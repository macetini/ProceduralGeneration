using UnityEngine;

namespace Assets.Scripts.DungeonGenerator.VoxelData
{
    public class Voxel : MonoBehaviour
    {
        private const string NAME = "Voxel";
        private const string VOXEL_STRING_PADDING = "00";
        
        //[SerializeField]
        public Vector3 WorldPosition; //TODO - Check how to make this better.

        public void SetLocalPositionName()
        {
            string x = transform.localPosition.x.ToString(VOXEL_STRING_PADDING);
            string y = transform.localPosition.y.ToString(VOXEL_STRING_PADDING);
            string z = transform.localPosition.z.ToString(VOXEL_STRING_PADDING);

            name = string.Format(NAME + " - {{ {0}, {1}, {2} }}", x, y, z);
        }

        public void SetWorldPositionName(Vector3 worldPosition)
        {
            string x1 = transform.localPosition.x.ToString(VOXEL_STRING_PADDING);
            string y1 = transform.localPosition.y.ToString(VOXEL_STRING_PADDING);
            string z1 = transform.localPosition.z.ToString(VOXEL_STRING_PADDING);

            string x2 = worldPosition.x.ToString(VOXEL_STRING_PADDING);
            string y2 = worldPosition.y.ToString(VOXEL_STRING_PADDING);
            string z2 = worldPosition.z.ToString(VOXEL_STRING_PADDING);

            name = string.Format(
                NAME + " - {{ {0}, {1}, {2} }} , {{ {3}, {4}, {5} }}",
                x1, y1, z1, x2, y2, z2
            );
        }
    }
}
