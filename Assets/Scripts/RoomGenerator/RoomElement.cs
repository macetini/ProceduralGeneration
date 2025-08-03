using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.VoxelData;
using Assets.Scripts.DungeonGenerator.Elements;
using Assets.Scripts.RoomGenerator.Conditions;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator
{
    public class RoomElement : MonoBehaviour
    {
        public new string name;
        public EndPoint endPoint;
        public List<GenerationCondition> generationConditions;
        public Volume Volume => GetComponent<Volume>();

        //TODO - Investigate why this is needed, and if it can be removed.
        //private List<Voxel> voxels;

        public Vector3[] GetVoxelsWorldPositions()
        {
            List<Voxel> localVoxels = GetComponent<Volume>().Voxels;
            Vector3[] worldPositions = new Vector3[localVoxels.Count];

            localVoxels.ForEach((voxel) => worldPositions[localVoxels.IndexOf(voxel)] = voxel.WorldPosition);

            return worldPositions;
        }

        public void InitConditionData()
        {
            generationConditions.ForEach(condition =>
            {
                //condition.Init();
                condition.SetOwner(this);
            });
        }

        //TODO - Investigate why this is needed, and if it can be removed.
        /*
        public Vector3[] GetOffsetVoxelPositions(Vector3 offset)
        {
            int voxelsCount = voxels.Count;
            Vector3[] offsetPositions = new Vector3[voxelsCount];
            for (int i = 0; i < voxelsCount; i++)
            {
                Voxel voxel = voxels[i];
                Vector3 offsetPosition = voxel.transform.localPosition + offset;
                offsetPositions[i] = offsetPosition.RoundVec3ToInt();
            }

            return offsetPositions;
        }
        */

        private void OnDrawGizmos()
        {
            if (endPoint == null || endPoint.Rotations == null) return;

            Vector3 endPointPosition = endPoint.transform.position;

            int rotationsCount = endPoint.Rotations.Count;
            for (int i = 0; i < rotationsCount; i++)
            {
                RotationType rotationType = endPoint.Rotations[i];

                Gizmos.color = EndPoint.GetRotationColor(rotationType);
                Quaternion rotationMatrix = EndPoint.GetRotation(rotationType) * transform.rotation;

                Gizmos.DrawLine(endPointPosition, endPointPosition + rotationMatrix * Vector3.forward);
            }

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(endPointPosition, 0.1f);
        }
    }
}