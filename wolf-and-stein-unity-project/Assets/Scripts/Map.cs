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
    TileTypes[,] mapData;


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
        floor.GetComponent<MeshRenderer>().sharedMaterial.color = floorColor;
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

                if (IsValidCoord(row - 1, col) && IsTunnelOnCoord(row - 1, col))
                    Instantiate(GamePreferences.Instance.Wall1, spawnPos + new Vector3(0, 0, 0.5f), Quaternion.identity, dynamic);
                if (IsValidCoord(row + 1, col) && IsTunnelOnCoord(row + 1, col))
                    Instantiate(GamePreferences.Instance.Wall1, spawnPos + new Vector3(0, 0, -0.5f), Quaternion.identity, dynamic);
                if (IsValidCoord(row, col - 1) && IsTunnelOnCoord(row, col - 1))
                    Instantiate(GamePreferences.Instance.Wall1, spawnPos + new Vector3(-0.5f, 0, 0), Quaternion.Euler(0, 90, 0), dynamic);
                if (IsValidCoord(row, col + 1) && IsTunnelOnCoord(row, col + 1))
                    Instantiate(GamePreferences.Instance.Wall1, spawnPos + new Vector3(0.5f, 0, 0), Quaternion.Euler(0, 90, 0), dynamic);
            }
            s += "\n";
        }
        //Debug.Log(s);
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
        return mapData[row, col] <= TileTypes.WALL_MAX;
    }

    private bool IsTunnelOnCoord(int row, int col)
    {
         return mapData[row, col] == TileTypes.TUNNEL;
    }



    private void DeleteCurrentLevel()
    {
        foreach (Transform t in dynamic)
            DestroyImmediate(t.gameObject);
    }

    private void ReadMapData(int level)
    {
        Texture2D image = (Texture2D)Resources.Load(level.ToString());
        mapData = new TileTypes[image.width, image.height];

        for (int x = 0; x < image.width; ++x)
        {
            for(int y = 0; y < image.height; ++y)
            {
                int i = image.height - 1 - y;
                int j = x;
                Color c = image.GetPixel(x, y);
                if (c == Color.black)
                    mapData[i, j] = TileTypes.WALL1;
                else if (c == Color.white)
                    mapData[i, j] = TileTypes.TUNNEL;
            }
        }


    }

    public enum TileTypes : int
    {
        WALL1,

        WALL_MAX = WALL1,
        TUNNEL,
        DOOR
    }
}
