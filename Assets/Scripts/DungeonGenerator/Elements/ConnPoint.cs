using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConnPoint : MonoBehaviour
{
    public bool open = true;
    public Voxel voxelOwner;    

    public ConnPoint sharedConnPoint;

    //public Door door;

    public ConnPointCandidate CloneCandidate
    {
        get
        {
            ConnPointCandidate clone = new ConnPointCandidate
            {
                Open = open,
                VoxelOwner = voxelOwner,

                Name = name,
                LocalPosition = transform.position,
                Rotation = transform.rotation
            };

            return clone;
        }
    }
}
