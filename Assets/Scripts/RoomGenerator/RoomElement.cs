using System.Collections.Generic;
using Assets.Scripts.DungeonGenerator.VoxelData;
using Assets.Scripts.DungeonGenerator.Elements;
using Assets.Scripts.DungeonGenerator.Utils;
using Assets.Scripts.RoomGenerator.Conditions;
using UnityEngine;

namespace Assets.Scripts.RoomGenerator
{
    public class RoomElement : MonoBehaviour
    {
        public new string name;
        public EndPoint endPoint;
        public List<GenerationCondition> generationConditions;

        //private Voxel[] voxels = null;
        //private GameObject[] voxelGOs = null;
        //private Volume volume;

        public List<Voxel> Voxels { get; set; }// => voxels;
                                               // public GameObject[] VoxelGOs => voxelGOs;
        public Volume Volume { get; set; }

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            Volume = GetComponent<Volume>();
            InitVoxelData();
            InitConditionData();
        }

        private void InitVoxelData()
        {
            //Volume.voxels.ForEach(n => Debug.Log(n));
            Voxels = Volume.voxels;
                      
            /*voxelGOs = Volume.voxels.ToArray();

            int voxelsCount = VoxelGOs.Length;
            voxels = new Voxel[voxelsCount];
            for (int i = 0; i < voxelsCount; i++)
            {
                Voxel voxel = VoxelGOs[i].GetComponent<Voxel>();
                Voxels[i] = voxel;
            }*/
        }

        private void InitConditionData()
        {
            int conditionsCount = generationConditions.Count;
            for (int i = 0; i < conditionsCount; i++)
            {
                GenerationCondition condition = generationConditions[i];
                //condition.Init();
                condition.SetOwner(this);
            }
        }

        public Vector3[] GetOffsetVoxelPositions(Vector3 offset)
        {
            int voxelsCount = Voxels.Count;
            Vector3[] offsetPositions = new Vector3[voxelsCount];
            for (int i = 0; i < voxelsCount; i++)
            {
                Voxel voxel = Voxels[i];
                Vector3 offsetPosition = voxel.transform.localPosition + offset;
                offsetPositions[i] = offsetPosition.RoundVec3ToInt();
            }

            return offsetPositions;
        }

        private void OnDrawGizmosSelected()
        {
            if (endPoint == null || endPoint.directions == null) return;

            Vector3 endPointPosition = endPoint.transform.position;

            int directionsCount = endPoint.directions.Count;
            for (int i = 0; i < directionsCount; i++)
            {
                DirectionType direction = endPoint.directions[i];

                Gizmos.color = EndPoint.GetDirectionColor(direction);
                Quaternion directionRotation = EndPoint.GetRotation(direction) * transform.rotation;

                Gizmos.DrawLine(endPointPosition, endPointPosition + directionRotation * Vector3.forward);
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(endPointPosition, 0.1f);
        }
    }
}