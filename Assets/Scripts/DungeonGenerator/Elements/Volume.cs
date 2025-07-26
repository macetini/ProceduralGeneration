using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.Data;
using Assets.Scripts.DungeonGenerator.Utils;

namespace Assets.Scripts.DungeonGenerator.Elements
{
    [System.Serializable]
    public class Volume : MonoBehaviour
    {
        public static readonly string VOXELS_CONTAINER_NAME = "Voxels";

        public Vector3 generatorSize = new(1f, 1f, 1f);
        public Voxel voxelType;

        public float VoxelScale = 10f;

        public List<GameObject> voxels = new();

        public static bool drawVolume = true;

        public GameObject voxelsContainer;

        //Check why this is half 
        public Bounds bounds;

        public Color boundsGizmoColor = Color.red;
        public Color voxelsGizmoColor = Color.blue;

        [ContextMenu("Generate Voxel Grid")]
        public void GenerateVoxelGrid()
        {
            if (voxels.Count != 0)
            {
                for (int i = 0; i < voxels.Count; i++)
                {
                    DestroyImmediate(voxels[i]);
                }
            }

            if (voxelsContainer == null)
            {
                voxelsContainer = new GameObject(VOXELS_CONTAINER_NAME);
                voxelsContainer.transform.parent = transform;
            }

            voxels = new List<GameObject>();
            for (float i = 0; i < generatorSize.x; i++)
            {
                for (float j = 0; j < generatorSize.y; j++)
                {
                    for (float k = 0; k < generatorSize.z; k++)
                    {
                        CreateVoxel(i * VoxelScale,
                                    j * VoxelScale,
                                    k * VoxelScale);
                    }
                }
            }
        }

        private void CreateVoxel(float i, float j, float k)
        {
            Voxel voxel = Instantiate(voxelType);
            voxel.name = string.Format(Voxel.NAME + " - ({0}, {1}, {2})", i, j, k);
            voxel.transform.position = new Vector3(i, j, k);
            voxel.transform.parent = voxelsContainer.transform;
        }

        [ContextMenu("Assign Voxels")]
        public void AssignVoxelsToList()
        {
            Vector3 firstVoxelPos = voxelsContainer.transform.GetChild(0).transform.position;

            Vector3 min, max;
            min = max = new Vector3(firstVoxelPos.x, firstVoxelPos.y, firstVoxelPos.z);

            int childCount = voxelsContainer.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform voxelChild = voxelsContainer.transform.GetChild(i);

                voxels.Add(voxelChild.gameObject);

                Vector3 voxelChildPos = voxelChild.transform.position;

                if (voxelChildPos.x < min.x) min.x = voxelChildPos.x;
                if (voxelChildPos.y < min.y) min.y = voxelChildPos.y;
                if (voxelChildPos.z < min.z) min.z = voxelChildPos.z;

                if (voxelChildPos.x > max.x) max.x = voxelChildPos.x;
                if (voxelChildPos.y > max.y) max.y = voxelChildPos.y;
                if (voxelChildPos.z > max.z) max.z = voxelChildPos.z;
            }

            Debug.Log("Volume::AssignVoxelsToList() | " + min + " : " + max);

            float halfVoxelScale = 0.5f * VoxelScale;
            Vector3 size = new Vector3(halfVoxelScale, halfVoxelScale, halfVoxelScale);

            bounds = new Bounds((min + max) * 0.5f, max + size - (min - size));
        }


        public List<Vector3> GetTranslatedVoxels(Vector3 translation = default, Quaternion rotation = default)
        {
            int voxelsCount = voxels.Count;
            List<Vector3> translatedVoxels = new List<Vector3>(voxelsCount);

            for (int i = 0; i < voxelsCount; i++)
            {
                Vector3 newPosition = rotation * voxels[i].transform.position + translation;
                translatedVoxels.Add(newPosition);
            }

            return translatedVoxels;
        }

        [ContextMenu("Recalculate Bounds")]
        public void RecalculateBounds()
        {
            Vector3 position = voxels[0].transform.position;

            Vector3 min, max;
            min = max = new Vector3(position.x, position.y, position.z);

            int voxelsCount = voxels.Count;
            for (int i = 0; i < voxelsCount; i++)
            {
                position = voxels[i].transform.position;

                if (position.x < min.x) min.x = position.x;
                if (position.y < min.y) min.y = position.y;
                if (position.z < min.z) min.z = position.z;

                if (position.x > max.x) max.x = position.x;
                if (position.y > max.y) max.y = position.y;
                if (position.z > max.z) max.z = position.z;
            }

            float halfVoxelScale = 0.5f * VoxelScale;
            Vector3 size = new Vector3(halfVoxelScale, halfVoxelScale, halfVoxelScale);

            bounds = new Bounds((min + max) * 0.5f, max + size - (min - size));
        }

        [ContextMenu("Recalculate Voxels World Space")]
        public void RecalculateVoxelsWorldSpace()
        {
            int voxelsCount = voxels.Count;
            for (int i = 0; i < voxelsCount; i++)
            {
                GameObject voxelGO = voxels[i];
                Voxel voxel = voxelGO.GetComponent<Voxel>();

                voxel.WorldPosition = voxel.transform.position.RoundVec3ToInt();
                Voxel.SetGameObjectName(voxelGO, voxel.WorldPosition);
            }
        }

        [ContextMenu("Toggle Gizmo Mode")]
        public void ToggleGizmoToDraw()
        {
            Volume.drawVolume = !Volume.drawVolume;
        }

        public void OnDrawGizmos()
        {
            if (!drawVolume)
            {
                Gizmos.color = boundsGizmoColor;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
            else
            {
                if (voxelsContainer == null) return;

                int childCount = voxelsContainer.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Gizmos.color = voxelsGizmoColor;
                    Gizmos.DrawWireCube(voxelsContainer.transform.GetChild(i).transform.position, Vector3.one * VoxelScale);
                }
            }
        }
    }
}