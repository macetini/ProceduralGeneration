﻿using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 RoundVec3ToInt(this Vector3 v)
    {
        return new Vector3(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
    }
}
