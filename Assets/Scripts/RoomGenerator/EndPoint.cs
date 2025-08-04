using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.VoxelData;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator
{
    public class EndPoint : MonoBehaviour
    {
        public List<RotationType> Rotations = new();
        public Voxel VoxelOwner;

        public static Dictionary<RotationType, Color> RotationColors = new()
        {
            { RotationType.DEGREES_0, Color.white },
            { RotationType.DEGREES_90, Color.red },
            { RotationType.DEGREES_180, Color.green },
            { RotationType.DEGREES_270, Color.blue }
        };

        public static Quaternion GetRotation(RotationType direction)
        {
            return Quaternion.AngleAxis((float)direction, Vector3.up);
        }

        public static Color GetRotationColor(RotationType rotation)
        {
            return RotationColors[rotation];
        }
    }
}