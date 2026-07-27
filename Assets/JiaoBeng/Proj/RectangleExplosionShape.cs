using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RectangleExplosionShape : MonoBehaviour, IExplosionShape
{
    public int width = 3;
    
    public List<Vector3Int> GetAffectedGridPositions(Vector3 worldCenter, Tilemap tilemap)
    {
        List<Vector3Int> Result = new List<Vector3Int>();
        Vector3Int centerGrid = tilemap.WorldToCell(worldCenter);
        for(int i = -width; i <= width; i++)
        {
            for(int j = -width; j <= width; j++)
            {
                Vector3Int pos = new Vector3Int(
                    centerGrid.x + i, centerGrid.y + j,centerGrid.z

                    );
                Result.Add(pos);
            }
        }
        return Result;
    }
}
