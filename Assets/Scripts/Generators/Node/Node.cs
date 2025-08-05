using System.Collections.Generic;
using Assets.Scripts.Generators.Meta.VoxelData;
using Assets.Scripts.DungeonGenerator.Elements;
using Assets.Scripts.Generators.Node.Conditions;
using UnityEngine;
using Assets.Scripts.Generators.Node.Points;
using Assets.Scripts.Generators.Node.Points.Meta;

namespace Assets.Scripts.Generators.Node
{
    public class Node : MonoBehaviour
    {
        public new string name;
        public EndPoint endPoint;
        public List<GenerationCondition> generationConditions;
        public Volume Volume => GetComponent<Volume>();

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
                condition.SetOwner(this);
            });
        }

        private void OnDrawGizmos()
        {
            if (endPoint == null || endPoint.Rotations == null) return;

            Vector3 endPointPosition = endPoint.transform.position;

            int rotationsCount = endPoint.Rotations.Count;
            for (int i = 0; i < rotationsCount; i++)
            {
                RotationData RotationData = endPoint.Rotations[i];

                Gizmos.color = EndPoint.GetRotationColor(RotationData);
                Quaternion rotationMatrix = EndPoint.GetRotation(RotationData) * transform.rotation;

                Gizmos.DrawLine(endPointPosition, endPointPosition + rotationMatrix * Vector3.forward);
            }

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(endPointPosition, 0.1f);
        }
    }
}