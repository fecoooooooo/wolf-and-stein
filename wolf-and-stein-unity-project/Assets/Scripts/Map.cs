﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Map:MonoBehaviourSingleton<Map>
{
    public Color floorColor;
    public Color ceilingColor;
    public int CurrentLevel { get; private set; }

    public static readonly Color Wall1Color = Color.black;
    public static readonly Color Wall2Color = new Color(0.4980392f, 0.4980392f, 0.4980392f, 1.000f);
    public static readonly Color WoodColumnColor = new Color32(166, 109, 109, 255);
    public static readonly Color StoneColumnColor = new Color32(94, 0, 0, 255);
    public static readonly Color DoorColor = new Color(0.7254902f, 0.4784314f, 0.3411765f, 1.000f);
    public static readonly Color LampColor = new Color(1f, 0.9490196f, 0f, 1.000f);
    public static readonly Color TunnelColor = Color.white;
    public static readonly Color SpawnPositionColor = Color.magenta;
    public static readonly Color FoodColor = new Color32(55, 255, 0, 255);
    public static readonly Color AmmoColor = new Color32(170, 0, 232, 255);
    public static readonly Color KeyColor = new Color32(183, 255, 0, 255);
    public static readonly Color NoteColor = new Color32(0, 4, 255, 255);
    public static readonly Color TreasureColor = new Color32(237, 255, 135, 255);

    public EventHandler MinimapUpdated;

    Transform floor;
    Transform ceiling;
    Transform dynamic;
    TileType[,] mapData;
    public Texture2D MinimapTexture { get; private set; }

    Vector2Int playerCoords;
    Vector2Int prevPlayerCoords;
    Color previousColorOnPlayerCoord;

    void Start()
    {
        dynamic = transform.Find("Dynamic");

        LoadLevel(1);

        SetFloorAndCeiling();
    }

	private void Update()
	{
        if (!Application.isPlaying)
            return;

        playerCoords = GetPlayerCoords();

        if(playerCoords != prevPlayerCoords)
		{
            MinimapTexture.SetPixel(prevPlayerCoords.x, prevPlayerCoords.y, previousColorOnPlayerCoord);
            previousColorOnPlayerCoord = MinimapTexture.GetPixel(playerCoords.x, playerCoords.y);

            MinimapTexture.SetPixel(playerCoords.x, playerCoords.y, Color.red);
            MinimapTexture.Apply();

            MinimapUpdated?.Invoke(this, null);
        }

        prevPlayerCoords = playerCoords;
        //Debug.Log(GetPlayerCoords());
	}

	private Vector2Int GetPlayerCoords()
	{
        int x = Mathf.FloorToInt(Character.instance.transform.position.x);
        int y = Mathf.FloorToInt(Mathf.Abs(Character.instance.transform.position.z)); //since Z goes on the negative direction

        return new Vector2Int(x, y);
	}

	public void LoadLevel(int level)
	{
        Generate(level);

        MinimapTexture = new Texture2D(mapData.GetLength(0), mapData.GetLength(1));
        MinimapTexture.filterMode = FilterMode.Point;
        MinimapTexture.anisoLevel = 1;
        MinimapTexture.mipMapBias = -0.5f;

        Color[] colors = new Color[MinimapTexture.width * MinimapTexture.height];

        for(int i = 0; i < mapData.GetLength(0); ++i)
		{
            for(int j = 0; j < mapData.GetLength(1); ++j)
                colors[i * mapData.GetLength(0) + j] = IsPassableOnCoord(i, j) ? Color.white : Color.black;
		}

        MinimapTexture.SetPixels(colors);
        MinimapTexture.Apply();
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

        CurrentLevel = level;

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

                switch (mapData[row, col])
                {
                    case TileType.WALL1:
                        PlaceWall(spawnPos, row, col, GamePreferences.Instance.Wall1);
                        break;
                    case TileType.WALL2:
                        PlaceWall(spawnPos, row, col, GamePreferences.Instance.Wall2);
                        break;
                    case TileType.WOOD_COLUMN:
                        PlaceSimple(spawnPos, GamePreferences.Instance.WoodColumn);
                        break;
                    case TileType.STONE_COLUMN:
                        PlaceSimple(spawnPos, GamePreferences.Instance.StoneColumn);
                        break;
                    case TileType.DOOR:
                        PlaceDoor(spawnPos, row, col);
                        break;
                    case TileType.TUNNEL:
                        break;           
                    case TileType.LAMP:
                        PlaceSimple(spawnPos, GamePreferences.Instance.Lamp);
                        break;
                    case TileType.SPAWN:
                        PlaceCharacter(spawnPos, row, col);
                        break;
                    case TileType.FOOD:
                        PlaceSimple(spawnPos, GamePreferences.Instance.Food);
                        break;
                    case TileType.AMMO:
                        PlaceSimple(spawnPos, GamePreferences.Instance.Ammo);
                        break;
                    case TileType.KEY:
                        PlaceSimple(spawnPos, GamePreferences.Instance.Key);
                        break;
                    case TileType.NOTE:
                        PlaceSimple(spawnPos, GamePreferences.Instance.Note);
                        break;
                    case TileType.TREASURE:
                        PlaceSimple(spawnPos, GamePreferences.Instance.Treasure);
                        break;
                    default:
                        break;
                }
            }
            s += "\n";
        }
        //Debug.Log(s);
    }

	private void PlaceCharacter(Vector3 spawnPos, int row, int col)
	{
        Transform characterTransform = GameObject.Find("Character").transform;
        characterTransform.position = spawnPos;

        if (IsValidCoord(row - 1, col) && IsPassableOnCoord(row - 1, col))
            characterTransform.rotation = Quaternion.Euler(0, 0, 0);
        else if (IsValidCoord(row + 1, col) && IsPassableOnCoord(row + 1, col))
            characterTransform.rotation = Quaternion.Euler(0, 180, 0);
        else if (IsValidCoord(row, col - 1) && IsPassableOnCoord(row, col - 1))
            characterTransform.rotation = Quaternion.Euler(0, -90, 0);
        else if (IsValidCoord(row, col + 1) && IsPassableOnCoord(row, col + 1))
            characterTransform.rotation = Quaternion.Euler(0, 90, 0);
    }

	private void PlaceSimple(Vector3 spawnPos, GameObject prefab)
    {
        Instantiate(prefab, spawnPos, Quaternion.identity, dynamic);
    }

    private void PlaceDoor(Vector3 spawnPos, int row, int col)
    {
        //place door object
        bool isTunnelOverAndUnderDoor = IsValidCoord(row - 1, col) && IsPassableOnCoord(row - 1, col) || IsValidCoord(row + 1, col) && IsPassableOnCoord(row + 1, col);
        if (isTunnelOverAndUnderDoor)
            Instantiate(GamePreferences.Instance.Door, spawnPos, Quaternion.identity, dynamic);
		else
		{
            bool doorToLeft = IsValidCoord(row, col - 1) && IsPassableOnCoord(row, col - 1);
            bool doorToRight = IsValidCoord(row, col + 1) && IsPassableOnCoord(row, col + 1);
            if (doorToLeft)
                Instantiate(GamePreferences.Instance.Door, spawnPos, Quaternion.Euler(0, -90, 0), dynamic);
            else if (doorToRight)
                Instantiate(GamePreferences.Instance.Door, spawnPos, Quaternion.Euler(0, 90, 0), dynamic);
        }
        

        //place frame of door
        if (IsValidCoord(row - 1, col) && IsUnpassableOnCoord(row - 1, col))
            Instantiate(GamePreferences.Instance.DoorFrame, spawnPos + new Vector3(0, 0, 0.5f), Quaternion.identity, dynamic);
        if (IsValidCoord(row + 1, col) && IsUnpassableOnCoord(row + 1, col))
            Instantiate(GamePreferences.Instance.DoorFrame, spawnPos + new Vector3(0, 0, -0.5f), Quaternion.identity, dynamic);
        if (IsValidCoord(row, col - 1) && IsUnpassableOnCoord(row, col - 1))
            Instantiate(GamePreferences.Instance.DoorFrame, spawnPos + new Vector3(-0.5f, 0, 0), Quaternion.Euler(0, 90, 0), dynamic);
        if (IsValidCoord(row, col + 1) && IsUnpassableOnCoord(row, col + 1))
            Instantiate(GamePreferences.Instance.DoorFrame, spawnPos + new Vector3(0.5f, 0, 0), Quaternion.Euler(0, 90, 0), dynamic);
    }

    private void PlaceWall(Vector3 spawnPos, int row, int col, GameObject prefab)
    {
        if (IsValidCoord(row - 1, col) && IsPassableOnCoord(row - 1, col))
            Instantiate(prefab, spawnPos + new Vector3(0, 0, 0.5f), Quaternion.identity, dynamic);
        if (IsValidCoord(row + 1, col) && IsPassableOnCoord(row + 1, col))
            Instantiate(prefab, spawnPos + new Vector3(0, 0, -0.5f), Quaternion.identity, dynamic);
        if (IsValidCoord(row, col - 1) && IsPassableOnCoord(row, col - 1))
            Instantiate(prefab, spawnPos + new Vector3(-0.5f, 0, 0), Quaternion.Euler(0, 90, 0), dynamic);
        if (IsValidCoord(row, col + 1) && IsPassableOnCoord(row, col + 1))
            Instantiate(prefab, spawnPos + new Vector3(0.5f, 0, 0), Quaternion.Euler(0, 90, 0), dynamic);
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

    private bool IsUnpassableOnCoord(int row, int col)
    {
        return mapData[row, col] <= TileType.UNPASSABLE;
    }

    private bool IsSemiPassableOnCoord(int row, int col)
    {
        return TileType.UNPASSABLE < mapData[row, col] && mapData[row, col] <= TileType.SEMIPASSABLE;
    }

    private bool IsPassableOnCoord(int row, int col)
    {
         return TileType.UNPASSABLE < mapData[row, col] && mapData[row, col] <= TileType.PASSABLE;
    }

    private void DeleteCurrentLevel()
    {
        for (var i = dynamic.childCount - 1; i >= 0; i--)
            DestroyImmediate(dynamic.GetChild(i).gameObject);
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
                else if (c == WoodColumnColor)
                    mapData[i, j] = TileType.WOOD_COLUMN;
                else if (c == StoneColumnColor)
                    mapData[i, j] = TileType.STONE_COLUMN;
                else if (c == DoorColor)
                    mapData[i, j] = TileType.DOOR;
                else if (c == TunnelColor)
                    mapData[i, j] = TileType.TUNNEL;
                else if (c == LampColor)
                    mapData[i, j] = TileType.LAMP;
                else if (c == SpawnPositionColor)
                    mapData[i, j] = TileType.SPAWN;
                else if (c == FoodColor)
                    mapData[i, j] = TileType.FOOD;
                else if (c == AmmoColor)
                    mapData[i, j] = TileType.AMMO;
                else if (c == KeyColor)
                    mapData[i, j] = TileType.KEY;
                else if (c == NoteColor)
                    mapData[i, j] = TileType.NOTE;
                else if (c == TreasureColor)
                    mapData[i, j] = TileType.TREASURE;
                else
                    throw new Exception("This color is not specified yet: " + c);
            }
        }
    }

    internal void SetLevel(int Level)
	{
        this.CurrentLevel = Level;
	}

	public enum TileType : int
    {
        WALL1,
        WALL2,
        WOOD_COLUMN,
        STONE_COLUMN,
        
        UNPASSABLE = STONE_COLUMN,
        
        DOOR,

        SEMIPASSABLE = DOOR,

        TUNNEL,
        LAMP,
        
        FOOD,
        AMMO,
        KEY,
        NOTE,
        TREASURE,

        SPAWN,

        PASSABLE = SPAWN
    }
}
