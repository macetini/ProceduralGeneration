using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.Data;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator
{
    public class EndPoint : MonoBehaviour
    {
        public List<DirectionType> directions = new List<DirectionType>();
        public Voxel voxelOwner;

        public static readonly Dictionary<DirectionType, Color> directionColors = new Dictionary<DirectionType, Color>() {
        { DirectionType.FORWARD, Color.blue },
        { DirectionType.LEFT, Color.yellow },
        { DirectionType.DOWN, Color.red },
        { DirectionType.RIGHT, Color.green },
    };

        public static Quaternion GetRotation(DirectionType direction)
        {
            return Quaternion.AngleAxis((float)direction, Vector3.up);
        }

        public static Color GetDirectionColor(DirectionType direction)
        {
            return directionColors[direction];
        }
    }
}