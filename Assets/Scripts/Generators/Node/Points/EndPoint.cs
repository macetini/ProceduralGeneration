using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.VoxelData;
using Assets.Scripts.Generators.Node.Points.Meta;
using UnityEngine;

namespace Assets.Scripts.Generators.Node.Points
{
    public class EndPoint : MonoBehaviour
    {
        public List<RotationData> Rotations = new();
        public Voxel VoxelOwner;

        public static Dictionary<RotationData, Color> RotationColors = new()
        {
            { RotationData.DEGREES_0, Color.white },
            { RotationData.DEGREES_90, Color.red },
            { RotationData.DEGREES_180, Color.green },
            { RotationData.DEGREES_270, Color.blue }
        };

        public static Quaternion GetRotation(RotationData direction)
        {
            return Quaternion.AngleAxis((float)direction, Vector3.up);
        }

        public static Color GetRotationColor(RotationData rotation)
        {
            return RotationColors[rotation];
        }
    }
}