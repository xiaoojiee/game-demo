using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IExplosionShape
{
    List<Vector3Int> GetAffectedGridPositions(Vector3 worldCenter, Tilemap tilemap);
}
