using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Unity.Collections.AllocatorManager;

public class Map : MonoBehaviour
{
    public Tilemap GroundTileMap;
    public int Width;
    public int Height;
    public int Seed;
    public bool isSeed;
    public float lacunarity;
    [Range(0, 1f)]
    public float Viod;
    public TileBase groundTile;
    public TileBase VoidTile;
    [Min(1)]
    public int Thickness;
    private int totalWidth;
    private int totalHeight;
    private bool[,] mapData;
    public GameObject blockObject;
    public TileBase bedrockTile;    
    public GameObject bedrockBlock;
    private List<GameObject> TileBlock=new List<GameObject>();
    public void MakerMap()
    {
        CleanMap();
        MakerMapData();
        MakerTileMap();
    }
    public void MakerMapData()
    {
        totalWidth = Width + 2;
        totalHeight = Height + 2;
        int maxBorder=Mathf.Min(Width/2,Height/2);
        int clampMin = 1;
        
        int clampMax = Mathf.Max(maxBorder, clampMin);
        Thickness = Mathf.Clamp(Thickness, clampMin, clampMax);
        
        if (!isSeed)
        {
            Seed=Time.time.GetHashCode();
        }
        UnityEngine.Random.InitState(Seed);
        mapData=new bool[totalWidth, totalHeight];
        float randomOffset=UnityEngine.Random.Range(-1000, 1000);
        for (int x = 0; x < totalWidth; x++)
        {
            for (int y = 0; y < totalHeight; y++)
            {
                bool isBedrockRing = (x == 0) || (x == totalWidth - 1)
                                  || (y == 0) || (y == totalHeight - 1);
                if (isBedrockRing)
                {
                    
                    mapData[x, y] = true;
                }
                else
                {
                    
                    int innerX = x - 1;
                    int innerY = y - 1;
                    bool border = IsBorderArea(innerX, innerY);

                    if (border)
                    {
                        mapData[x, y] = true;
                    }
                    else
                    {
                        float noiseValue = Mathf.PerlinNoise(innerX * lacunarity + randomOffset, innerY * lacunarity + randomOffset);
                        mapData[x, y] = noiseValue < Viod ? false : true;
                    }
                }
            }
        }
     }
    private bool IsBorderArea(int x,int y)
    {
        bool left = x < Thickness;
        bool right = x>=Width-Thickness;
        bool bottom = y < Thickness;
        bool top = y>=Height-Thickness;
        return left || right || bottom || top;
    }
    private void MakerTileMap()
    {
        
        for (int x = 0; x < totalWidth; x++)
        {
            for (int y = 0; y < totalHeight; y++)
            {
                bool isBedrockRing = (x == 0) || (x == totalWidth - 1)
                                  || (y == 0) || (y == totalHeight - 1);

                TileBase tile;
                if (isBedrockRing)
                {
                    
                    tile = bedrockTile;
                }
                else
                {
                    
                    tile = mapData[x, y] ? groundTile : VoidTile;
                }
                GroundTileMap.SetTile(new Vector3Int(x, y), tile);
            }
        }
        Block();
        Clean_Map();

    }
    void Block()
    {
        if(blockObject== null)
        {
            return;
        }
        if (bedrockBlock == null)
        {
            return;
        }
        for (int x = 0; x < totalWidth; x++)
        {
            for (int y = 0; y < totalHeight; y++)
            {
                if (mapData[x, y] == false) continue;

                bool isBedrockRing = (x == 0) || (x == totalWidth - 1)
                                  || (y == 0) || (y == totalHeight - 1);

                Vector3Int cellpos = new Vector3Int(x, y);
                
                Vector3 WorldPos = GroundTileMap.CellToWorld(cellpos) + new Vector3(0.5f, 0.5f, 0);

                GameObject newBlock;
                if (isBedrockRing)
                {
                    
                    newBlock = Instantiate(bedrockBlock, WorldPos, Quaternion.identity);
                }
                else
                {
                    
                    newBlock = Instantiate(blockObject, WorldPos, Quaternion.identity);
                }

                TileBlock.Add(newBlock);
            }
        }
    }
    public void Clean_Map()
    {
        GroundTileMap.ClearAllTiles();
    }
    public void CleanMap()
    {
        TileBlock.RemoveAll(item => item == null);
        foreach (var block in TileBlock)
        {
            if( block != null)
            {
                DestroyImmediate(block);
            }
            
        }
        TileBlock.Clear();
        GameObject[] allObjects=Object.FindObjectsOfType<GameObject>();
        string normalName = blockObject.name;
        string bedrockName = bedrockBlock.name;
        foreach (GameObject go in allObjects)
        {
            if (go.name.StartsWith(normalName) || go.name.StartsWith(bedrockName))
            {
                DestroyImmediate(go);
            }
        }
    }
}