using System.ComponentModel;
using UnityEngine;

namespace Assets.Scripts.DungeonGenerator.VoxelData
{
    public class Voxel : MonoBehaviour
    {
        private const string NAME = "Voxel";
        private const string VOXEL_STRING_PADDING = "00";

        [field: SerializeField, ReadOnly(true)] // Does not work as it should, the value is still set in the inspector
        public Vector3 LocalPosition { get; private set; } = Vector3.positiveInfinity;

        [field: SerializeField, ReadOnly(true)] // Does not work as it should, the value is still set in the inspector
        public Vector3 WorldPosition { get; private set; } = Vector3.positiveInfinity;

        public void SetLocalPosition(Vector3 localPosition)
        {
            LocalPosition = localPosition;
            WorldPosition = Vector3.positiveInfinity;

            string x = localPosition.x.ToString(VOXEL_STRING_PADDING);
            string y = localPosition.y.ToString(VOXEL_STRING_PADDING);
            string z = localPosition.z.ToString(VOXEL_STRING_PADDING);

            name = string.Format(NAME + " - {{ {0}, {1}, {2} }}", x, y, z);
        }

        public void SetWorldPosition(Vector3 worldPosition)
        {
            if (LocalPosition.Equals(Vector3.positiveInfinity))
            {
                throw new System.Exception("Volume:: Cannot set world position if local position is not defined.");
            }

            WorldPosition = worldPosition;

            string x1 = LocalPosition.x.ToString(VOXEL_STRING_PADDING);
            string y1 = LocalPosition.y.ToString(VOXEL_STRING_PADDING);
            string z1 = LocalPosition.z.ToString(VOXEL_STRING_PADDING);

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
