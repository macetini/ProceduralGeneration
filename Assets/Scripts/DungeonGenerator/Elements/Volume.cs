using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.VoxelData;
using Assets.Scripts.DungeonGenerator.Utils;

namespace Assets.Scripts.DungeonGenerator.Elements
{
    [System.Serializable]
    public class Volume : MonoBehaviour
    {
        private const string VOXELS_CONTAINER_NAME = "Voxels";

        public Vector3 generatorSize = new(1f, 1f, 1f);
        public Voxel voxelType;
        public float VoxelScale = 10f;
        public List<Voxel> voxels = new();
        public static bool DrawVolume = true;
        public GameObject voxelsContainer;
        public Bounds bounds;
        public Color BoundsGizmoColor { get; } = Color.red;
        public Color VoxelsGizmoColor { get; } = Color.blue;

        [ContextMenu("Generate All")]
        public void GenerateAll()
        {
            GenerateVoxelGrid();
            RecalculateBounds();
            RecalculateVoxelsWorldSpace();
        }

        [ContextMenu("Generate Voxel Grid")]
        public void GenerateVoxelGrid()
        {
            if (voxels.Count > 0)
            {
                voxels.ForEach(voxel => DestroyImmediate(voxel));
                voxels.Clear();
            }

            if (voxelsContainer == null)
            {
                voxelsContainer = new GameObject(VOXELS_CONTAINER_NAME);
                voxelsContainer.transform.parent = transform;
            }

            int totalVoxels = (int)(generatorSize.x * generatorSize.y * generatorSize.z);
            voxels = new List<Voxel>(totalVoxels);
            for (int i = 0; i < generatorSize.x; i++)
            {
                for (int j = 0; j < generatorSize.y; j++)
                {
                    for (int k = 0; k < generatorSize.z; k++)
                    {
                        Vector3 voxelPos = (new Vector3(i, j, k) * VoxelScale).RoundVec3ToInt();
                        Voxel newVoxel = CreateNewVoxelGo(voxelPos);
                        voxels.Add(newVoxel);
                    }
                }
            }
        }

        private Voxel CreateNewVoxelGo(Vector3 voxelPos)
        {
            Voxel newVoxel = Instantiate(voxelType);
            
            newVoxel.transform.position = voxelPos;
            newVoxel.transform.parent = voxelsContainer.transform;

            newVoxel.SetLocalPositionName();
            newVoxel.WorldPosition = Vector3.positiveInfinity;

            return newVoxel;
        }

        [ContextMenu("Recalculate Bounds")]
        public void RecalculateBounds()
        {
            Vector3 initPosition = voxels[0].transform.position;

            Vector3 min, max;
            min = max = new Vector3(initPosition.x, initPosition.y, initPosition.z);

            voxels.ForEach(voxel =>
            {
                Vector3 position = voxel.transform.position;

                min = Vector3.Min(min, position);
                max = Vector3.Max(max, position);
            });

            float halfVoxelScale = 0.5f * VoxelScale;
            Vector3 size = new(halfVoxelScale, halfVoxelScale, halfVoxelScale);

            bounds = new Bounds((min + max) * 0.5f, max + size - (min - size));
        }

        //TODO: Investigate why this is needed
        [ContextMenu("Recalculate Voxels World Space")]
        public void RecalculateVoxelsWorldSpace()
        {
            voxels.ForEach(voxel =>
            {
                voxel.WorldPosition = voxel.transform.position.RoundVec3ToInt();
                voxel.SetWorldPositionName(voxel.WorldPosition);
            });
        }

        [ContextMenu("Toggle Gizmo Mode")]
        public void ToggleGizmoToDraw()
        {
            DrawVolume = !DrawVolume;
        }

        //TODO: Investigate why this is needed
        public List<Vector3> GetTranslatedVoxels(Vector3 translation = default, Quaternion rotation = default)
        {
            int voxelsCount = voxels.Count;
            List<Vector3> translatedVoxels = new(voxelsCount);

            for (int i = 0; i < voxelsCount; i++)
            {
                Vector3 newPosition = rotation * voxels[i].transform.position + translation;
                translatedVoxels.Add(newPosition);
            }

            return translatedVoxels;
        }

        public void OnDrawGizmos()
        {
            if (!DrawVolume)
            {
                Gizmos.color = BoundsGizmoColor;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
            else
            {
                if (voxelsContainer == null) return;

                int childCount = voxelsContainer.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Gizmos.color = VoxelsGizmoColor;
                    Gizmos.DrawWireCube(voxelsContainer.transform.GetChild(i).transform.position, Vector3.one * VoxelScale);
                }
            }
        }
    }
}