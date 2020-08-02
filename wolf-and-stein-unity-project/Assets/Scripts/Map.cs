using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Map : MonoBehaviourSingleton<Map>
{
    public Color floorColor;
    public Color ceilingColor;

    Transform floor;
    Transform ceiling;
    Transform dynamic;
    TileType[,] mapData;

    readonly Color Wall1Color = Color.black;
    readonly Color Wall2Color = new Color(0.4980392f, 0.4980392f, 0.4980392f, 1.000f);
    readonly Color TunnelColor = Color.white;

    void Start()
    {
        dynamic = transform.Find("Dynamic");
        dynamic = transform.Find("Dynamic");

        SetFloorAndCeiling();
    }

    public void SetFloorAndCeiling()
    {
        floor = transform.Find("Static/Floor");
        ceiling = transform.Find("Static/Ceiling");
    }

    public void SetFloorAndCeilingColors()
    {
        if (null != floor && null != ceiling)
        {
            floor.GetComponent<MeshRenderer>().sharedMaterial.color = floorColor;
            ceiling.GetComponent<MeshRenderer>().sharedMaterial.color = ceilingColor;
        }
    }

    public void Generate(int level)
    {
        Debug.Log("Generatre level: " + level);
        
        ReadMapData(level);
        DeleteCurrentLevel();
        PlaceLevelPrefabs();
    }

    private void OnValidate()
    {
        SetFloorAndCeilingColors();
    }

    private void PlaceLevelPrefabs()
    {
        string s = "";
        for (int row = 0; row < mapData.GetLength(0); ++row)
        {
            for (int col = 0; col < mapData.GetLength(1); ++col)
            {
                s += (int)mapData[row, col];
                Vector3 spawnPos = new Vector3(col, 0.5f, -row);

                if (false == IsWallOnCoord(row, col))
                    continue;

                GameObject prefab = GetPrefabByTileType(mapData[row, col]);

                if (IsValidCoord(row - 1, col) && IsTunnelOnCoord(row - 1, col))
                    Instantiate(prefab, spawnPos + new Vector3(0, 0, 0.5f), Quaternion.identity, dynamic);
                if (IsValidCoord(row + 1, col) && IsTunnelOnCoord(row + 1, col))
                    Instantiate(prefab, spawnPos + new Vector3(0, 0, -0.5f), Quaternion.identity, dynamic);
                if (IsValidCoord(row, col - 1) && IsTunnelOnCoord(row, col - 1))
                    Instantiate(prefab, spawnPos + new Vector3(-0.5f, 0, 0), Quaternion.Euler(0, 90, 0), dynamic);
                if (IsValidCoord(row, col + 1) && IsTunnelOnCoord(row, col + 1))
                    Instantiate(prefab, spawnPos + new Vector3(0.5f, 0, 0), Quaternion.Euler(0, 90, 0), dynamic);
            }
            s += "\n";
        }
        //Debug.Log(s);
    }

    private GameObject GetPrefabByTileType(TileType type)
    {
        switch (type)
        {
            case TileType.WALL1:
                return GamePreferences.Instance.Wall1;
            case TileType.WALL2:
                return GamePreferences.Instance.Wall2;
            default:
                throw new Exception("No such prefab exsist for this tiletype");
        }
    }

    private bool IsValidCoord(int row, int col)
    {
        try
        {
            var a = mapData[row, col];
            return true;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    private bool IsWallOnCoord(int row, int col)
    {
        return mapData[row, col] <= TileType.WALL_MAX;
    }

    private bool IsTunnelOnCoord(int row, int col)
    {
         return mapData[row, col] == TileType.TUNNEL;
    }



    private void DeleteCurrentLevel()
    {
        foreach (Transform t in dynamic)
            DestroyImmediate(t.gameObject);
    }

    private void ReadMapData(int level)
    {
        Texture2D image = (Texture2D)Resources.Load(level.ToString());
        mapData = new TileType[image.width, image.height];

        for (int x = 0; x < image.width; ++x)
        {
            for(int y = 0; y < image.height; ++y)
            {
                int i = image.height - 1 - y;
                int j = x;
                Color c = image.GetPixel(x, y);
                if (c == Wall1Color)
                    mapData[i, j] = TileType.WALL1;
                else if (c == Wall2Color)
                    mapData[i, j] = TileType.WALL2;
                else if (c == TunnelColor)
                    mapData[i, j] = TileType.TUNNEL;
                else
                    throw new Exception("This color is not specified yet: " + c);
            }
        }


    }

    public enum TileType : int
    {
        WALL1,
        WALL2,

        WALL_MAX = WALL2,
        TUNNEL,
        DOOR
    }
}
