using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public struct GridPosition
{
    public int x; 
    public int z;

    public static bool operator ==(GridPosition GridPosA, GridPosition GridPosB)
    {
        if(GridPosA.x == GridPosB.x
            && GridPosA.z == GridPosB.z)
        {
            return true; 
        }

        else return false;
    }
    public static bool operator !=(GridPosition GridPosA, GridPosition GridPosB)
    {
        if (GridPosA.x == GridPosB.x
            && GridPosA.z == GridPosB.z)
        {
            return false;
        }

        else return true;
    }
    public override bool Equals(object obj)
    {
        return obj is GridPosition other && this == other;
    }
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + x;
            hash = hash * 31 + z;
            return hash;
        }
    }
}
