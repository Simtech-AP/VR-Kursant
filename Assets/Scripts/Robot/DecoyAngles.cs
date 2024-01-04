using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DecoyAngles
{
    public int PointIndex;
    public List<Quaternion> SavedAngles;

    public DecoyAngles(int index, List<Quaternion> angles)
    {
        PointIndex = index;
        SavedAngles = angles;
    }
}