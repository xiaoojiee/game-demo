using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CircleExplosion : MonoBehaviour,IExplosionShape
{
    public int R;
    public List<Vector3Int> GetAffectedGridPositions(Vector3 worldCenter,Tilemap tilemap)
    {
        List<Vector3Int>Result = new List<Vector3Int>();
        Vector3Int centerGrid=tilemap.WorldToCell(worldCenter);
        for(int x = -R; x <= R; x++)
        {
            for(int y = -R; y <= R; y ++)
            {
                if(Mathf.Sqrt(x * x + y * y) <= R)
                {
                    Vector3Int TargetGrid = new Vector3Int(
                        centerGrid.x + x,
                        centerGrid.y + y,
                        centerGrid.z

                        );
                    Result.Add( TargetGrid );
                }
            }
        }
        return Result;
    }
}

