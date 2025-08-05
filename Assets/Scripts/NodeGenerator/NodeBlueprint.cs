using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.NodeGenerator
{
    public class NodeBlueprint : MonoBehaviour
    {
        public new string name;
        public Node floor;
        public Node ceiling;
        public List<Node> walls;
        public List<Node> doors;
        public List<Node> windows;
        public List<Node> allNodes;

        public Vector3[] FloorVoxelsWorldPositions => floor.GetVoxelsWorldPositions();

        public HashSet<Vector3> WallsVoxelMap { get; private set; }
        public HashSet<Vector3> DoorsVoxelMap { get; private set; }


        public void Init()
        {
            InitAllNodes();

            WallsVoxelMap = GetVoxelMap(walls);
            DoorsVoxelMap = GetVoxelMap(doors);
        }


        private void InitAllNodes()
        {
            int wallsCount = walls.Count;
            int doorsCount = doors.Count;

            allNodes = new List<Node>(wallsCount + doorsCount + 1)
            {
                floor
            };

            allNodes.AddRange(walls);
            allNodes.AddRange(doors);
        }

        private static HashSet<Vector3> GetVoxelMap(List<Node> nodes)
        {
            HashSet<Vector3> voxelMap = new();

            foreach (var (node, index) in nodes.Select((node, index) => (node, index)))
            {
                if (node == null)
                {
                    Debug.Log("Room element at Index: '" + index + "' is not set. Skipping it from the voxels map.");
                    continue;
                }

                Vector3[] worldPositions = node.GetVoxelsWorldPositions();
                voxelMap.UnionWith(worldPositions);
            }

            return voxelMap;
        }
    }
}