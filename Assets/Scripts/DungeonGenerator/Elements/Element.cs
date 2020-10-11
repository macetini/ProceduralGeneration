using UnityEngine;
using System.Collections.Generic;

public class Element : MonoBehaviour
{
    public List<ConnPoint> connPoints;

    public ElementTypeEnum type;

    //FIX! FIX THIS!
    public string ID;

    //WARNING - when doing the dungeon gen we sometimes Instantiate a room, check if it will fit and if it doesn't
    //we IMMEDIATLY destroy it.  Awake() is called with instantiation - Start() waits until the function returns..
    //SO to be safe, don't use Awake if you don't have to.  Put all enemy and room specific instantiation in START!
    void Awake()
    {
        //DungeonGenerator.roomsCalledStart++;
        //Debug.Log("AWAKE");
    }

    void Start()
    {        
        //Debug.Log("START");
    }

    private void OnDrawGizmos()
    {
        int connPointsCount = connPoints.Count;
        for (int i = 0; i < connPointsCount; i++)
        {
            Gizmos.color = Color.red;
            
            Gizmos.DrawSphere(connPoints[i].transform.position, 0.1f);

            Gizmos.DrawRay(new Ray(connPoints[i].transform.position, connPoints[i].transform.right));

            Gizmos.color = Color.green;
            Gizmos.DrawRay(new Ray(connPoints[i].transform.position, connPoints[i].transform.up));

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(new Ray(connPoints[i].transform.position, connPoints[i].transform.forward));
        }
    }    

    public ConnPoint GetRandomOpenConnPoint(DRandom random)
    {
        connPoints.Shuffle(random.random);

        int connPointsCount = connPoints.Count;
        for (int i = 0; i < connPointsCount; i++)
        {
            if (connPoints[i].open)
            {
                return connPoints[i];
            }
        }

        Debug.Log("Room::GetRandomOpenConnPoint() - No open connection points.");
        return null;
    }

    public bool HasOpenConnection()
    {
        int connPointsCount = connPoints.Count;
        for (int i = 0; i < connPointsCount; i++)
        {
            if (connPoints[i].open)
            {
                return true;
            }
        }

        return false;
    }    

    public ConnPoint GetFirstOpenConnectionPoint()
    {
        int connPointsCount = connPoints.Count;
        for (int i = 0; i < connPointsCount; i++)
        {
            if (connPoints[i].open) return connPoints[i];
        }

        Debug.Log("Room::GetFirstOpenConnPoint() - No open connection points.");

        return null;
    }

    public Dictionary<Voxel, Vector3> GetConnPointsVoxelWorldPosition(Vector3 translation = default(Vector3), Quaternion rotation = default(Quaternion))
    {
        int connPointCount = connPoints.Count;
        Dictionary<Voxel, Vector3> connPointVoxelWorldPositions = new Dictionary<Voxel, Vector3>(connPointCount);

        for (int i = 0; i < connPointCount; i++)
        {
            ConnPoint connPoint = connPoints[i];
            Voxel voxelOwner = connPoint.voxelOwner;
            Vector3 newPostion = rotation * voxelOwner.transform.position + translation;

            connPointVoxelWorldPositions[voxelOwner] = newPostion;
        }

        return connPointVoxelWorldPositions;
    }    
}
