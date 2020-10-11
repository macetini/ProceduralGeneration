using System.Collections.Generic;
using UnityEngine;

public class RoomElement : MonoBehaviour
{
    public new string name;
    public EndPoint endPoint;
    public List<GenerationCondition> generationConditions;

    private Voxel[] voxels = null;
    private GameObject[] voxelGOs = null;

    private Volume volume;

    public Voxel[] Voxels => voxels;
    public GameObject[] VoxelGOs => voxelGOs;
    public Volume Volume => volume;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        volume = GetComponent<Volume>();
        InitVoxelData();
        InitConditionData();
    }

    public Vector3[] GetOffsetedVoxelPostions(Vector3 offset)
    {
        int voxelsCount = Voxels.Length;

        Vector3[] offsetedPositions = new Vector3[voxelsCount];

        for (int i = 0; i < voxelsCount; i++)
        {
            Voxel voxel = Voxels[i];
            Vector3 offstedPosition = voxel.transform.localPosition + offset;
            offsetedPositions[i] = offstedPosition.RoundVec3ToInt();
        }

        return offsetedPositions;
    }

    private void InitVoxelData()
    {
        voxelGOs = volume.voxels.ToArray();

        int voxelsCount = VoxelGOs.Length;

        voxels = new Voxel[voxelsCount];

        for (int i = 0; i < voxelsCount; i++)
        {
            Voxel voxel = VoxelGOs[i].GetComponent<Voxel>();
            Voxels[i] = voxel;
        }
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

    private void OnDrawGizmos()
    {
        Vector3 endPointPosition = endPoint.transform.position;

        int directionsCount = endPoint.directions.Count;
        for (int i = 0; i < directionsCount; i++)
        {
            DirectionType direction = endPoint.directions[i];

            Gizmos.color = EndPoint.GetDirectionColor(direction);
            Quaternion directionRotation = EndPoint.GetRoation(direction) * transform.rotation;

            Gizmos.DrawLine(endPointPosition, endPointPosition + directionRotation * Vector3.forward);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(endPointPosition, 0.1f);
    }
}