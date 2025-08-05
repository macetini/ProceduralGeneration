using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.VoxelData;
using Assets.Scripts.Utils;

namespace Assets.Scripts.DungeonGenerator.Elements
{
    [System.Serializable]
    public class Volume : MonoBehaviour
    {
        private const string VOXELS_CONTAINER_NAME = "Voxels";

        public Vector3 generatorSize = new(1f, 1f, 1f);
        public Voxel voxelPrefab;
        public float VoxelScale = 10f;
        public List<Voxel> Voxels = new();
        public GameObject voxelsContainer;
        public Bounds bounds;
        public Color BoundsGizmoColor = Color.red;
        public Color VoxelsGizmoColor = Color.blue;
        public static bool DrawVolume = true;

        [ContextMenu("Generate All")]
        public void GenerateAll()
        {
            GenerateVoxelGrid();
            RecalculateBounds();
            RecalculateVoxelsWorldPosition();
        }

        //TODO - Split into multiple methods
        [ContextMenu("Generate Voxel Grid")]
        public void GenerateVoxelGrid()
        {
            if (Voxels.Count > 0)
            {
                Voxels.ForEach(voxel => DestroyImmediate(voxel));
                Voxels.Clear();
            }

            if (voxelsContainer == null)
            {
                voxelsContainer = new GameObject(VOXELS_CONTAINER_NAME);
                voxelsContainer.transform.parent = transform;
                voxelsContainer.transform.localPosition = Vector3.zero;
            }

            int totalVoxels = (int)(generatorSize.x * generatorSize.y * generatorSize.z);
            Voxels = new List<Voxel>(totalVoxels);
            for (int i = 0; i < generatorSize.x; i++)
            {
                for (int j = 0; j < generatorSize.y; j++)
                {
                    for (int k = 0; k < generatorSize.z; k++)
                    {
                        Vector3 voxelPos = (new Vector3(i, j, k) * VoxelScale).RoundVec3ToInt();
                        Voxel newVoxel = CreateNewVoxelGameObject(voxelPos);
                        Voxels.Add(newVoxel);
                    }
                }
            }
        }

        private Voxel CreateNewVoxelGameObject(Vector3 voxelPos)
        {
            Voxel newVoxel = Instantiate(voxelPrefab, voxelsContainer.transform);
            newVoxel.transform.localPosition = voxelPos;
            newVoxel.SetLocalPosition(voxelPos);

            return newVoxel;
        }

        //TODO - Broken, fix it later.
        [ContextMenu("Recalculate Bounds")]
        public void RecalculateBounds()
        {
            Vector3 initPosition = voxelsContainer.transform.position;

            Vector3 min, max;
            min = max = new Vector3(initPosition.x, initPosition.y, initPosition.z);

            Voxels.ForEach(voxel =>
            {
                Vector3 position = voxel.transform.position;

                min = Vector3.Min(min, position);
                max = Vector3.Max(max, position);
            });

            float halfVoxelScale = 0.5f * VoxelScale;
            Vector3 size = new(halfVoxelScale, halfVoxelScale, halfVoxelScale);

            bounds = new Bounds((min + max) * 0.5f, max + size - (min - size));
        }
        
        [ContextMenu("Recalculate Voxels World Position")]
        public void RecalculateVoxelsWorldPosition()
        {
            Voxels.ForEach(voxel =>
            {                
                Vector3 worldPosition = voxel.transform.position.RoundVec3ToInt();
                voxel.SetWorldPosition(worldPosition);
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
            int voxelsCount = Voxels.Count;
            List<Vector3> translatedVoxels = new(voxelsCount);

            for (int i = 0; i < voxelsCount; i++)
            {
                Vector3 newPosition = rotation * Voxels[i].transform.position + translation;
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