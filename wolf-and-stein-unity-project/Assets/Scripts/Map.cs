using System;
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
    public static readonly Color Poster1Color = new Color(0.64705882352f, 1f, 0.54117647058f, 1.000f);
    public static readonly Color Poster2Color = new Color(0.2f, 0.31764705882f, 0.16078431372f, 1.000f);
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
    public static readonly Color MachineGunColor = new Color32(8, 41, 17, 255);
    public static readonly Color ChainGunColor = new Color32(98, 122, 105, 255);

    Transform floor;
    Transform ceiling;
    Transform dynamic;
    public TileType[,] MapData { get; private set; }

    List<Enemy> enemies = new List<Enemy>();

	void Start()
    {
        dynamic = transform.Find("Dynamic");
        SetFloorAndCeiling();

        if (Application.isPlaying)
            LoadLevel(1);
    }

    public List<Enemy> GetEnemies()
	{
        enemies.RemoveAll(en => en == null);
        return enemies;
	}

	public void LoadLevel(int level)
	{
        Generate(level);
        MiniMap.instance.GenerateMaps();
        CollectEnemies();
    }

	private void CollectEnemies()
	{
        Transform enemiesTransform = transform.Find("Enemies(move this to dynamic later)");
        enemies.AddRange(enemiesTransform.GetComponentsInChildren<Enemy>());
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
        for (int row = 0; row < MapData.GetLength(0); ++row)
        {
            for (int col = 0; col < MapData.GetLength(1); ++col)
            {
                s += (int)MapData[row, col];
                Vector3 spawnPos = new Vector3(col, 0.5f, -row);

                switch (MapData[row, col])
                {
                    case TileType.WALL1:
                        PlaceWall(spawnPos, row, col, GamePreferences.Instance.Wall1);
                        break;
                    case TileType.WALL2:
                        PlaceWall(spawnPos, row, col, GamePreferences.Instance.Wall2);
                        break;
                    case TileType.POSTER1:
                        PlaceWall(spawnPos, row, col, GamePreferences.Instance.Poster1);
                        break;
                    case TileType.POSTER2:
                        PlaceWall(spawnPos, row, col, GamePreferences.Instance.Poster2);
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
                    case TileType.MACHINE_GUN:
                        PlaceSimple(spawnPos, GamePreferences.Instance.MachineGun);
                        break;
                    case TileType.CHAIN_GUN:
                        PlaceSimple(spawnPos, GamePreferences.Instance.ChainGun);
                        break;
                    default:
                        break;
                }
            }
            s += "\n";
        }
        //Debug.Log(s);
    }

	internal void SpawnLootFromCorpse(Vector3 corpsePos)
	{
        int layerMask = 1 << LayerMask.NameToLayer("Blockers") | 1 << LayerMask.NameToLayer("Wall");

		Vector3 spawnPos = corpsePos;
        
        for (int i = 0; i < 4; ++i)
        {
            Vector3 offsetPos = corpsePos + Quaternion.Euler(0, i * 90, 0) * transform.forward;
            if (false == Physics.Linecast(corpsePos, offsetPos, layerMask, QueryTriggerInteraction.Ignore))
			{
                Vector3 halfOffset = corpsePos + Quaternion.Euler(0, i * 90, 0) * transform.forward / 2;
                spawnPos = halfOffset;
                break;
			}
        }

        PlaceSimple(spawnPos, GamePreferences.Instance.Ammo);

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

    public bool IsValidCoord(int row, int col)
    {
        try
        {
            var a = MapData[row, col];
            return true;
        }
        catch (IndexOutOfRangeException)
        {
            return false;
        }
    }

    private bool IsUnpassableOnCoord(int row, int col)
    {
        return MapData[row, col] <= TileType.UNPASSABLE;
    }

    private bool IsSemiPassableOnCoord(int row, int col)
    {
        return TileType.UNPASSABLE < MapData[row, col] && MapData[row, col] <= TileType.SEMIPASSABLE;
    }

    public bool IsPassableOnCoord(int row, int col)
    {
         return TileType.UNPASSABLE < MapData[row, col] && MapData[row, col] <= TileType.PASSABLE;
    }

    private void DeleteCurrentLevel()
    {
        for (var i = dynamic.childCount - 1; i >= 0; i--)
            DestroyImmediate(dynamic.GetChild(i).gameObject);
    }

    private void ReadMapData(int level)
    {
        Texture2D image = (Texture2D)Resources.Load(level.ToString());
        MapData = new TileType[image.width, image.height];

        for (int x = 0; x < image.width; ++x)
        {
            for(int y = 0; y < image.height; ++y)
            {
                int i = image.height - 1 - y;
                int j = x;
                Color c = image.GetPixel(x, y);
                if (c == Wall1Color)
                    MapData[i, j] = TileType.WALL1;
                else if (c == Wall2Color)
                    MapData[i, j] = TileType.WALL2;
                else if (c == Poster1Color)
                    MapData[i, j] = TileType.POSTER1;
                else if (c == Poster2Color)
                    MapData[i, j] = TileType.POSTER2;
                else if (c == WoodColumnColor)
                    MapData[i, j] = TileType.WOOD_COLUMN;
                else if (c == StoneColumnColor)
                    MapData[i, j] = TileType.STONE_COLUMN;
                else if (c == DoorColor)
                    MapData[i, j] = TileType.DOOR;
                else if (c == TunnelColor)
                    MapData[i, j] = TileType.TUNNEL;
                else if (c == LampColor)
                    MapData[i, j] = TileType.LAMP;
                else if (c == SpawnPositionColor)
                    MapData[i, j] = TileType.SPAWN;
                else if (c == FoodColor)
                    MapData[i, j] = TileType.FOOD;
                else if (c == AmmoColor)
                    MapData[i, j] = TileType.AMMO;
                else if (c == KeyColor)
                    MapData[i, j] = TileType.KEY;
                else if (c == NoteColor)
                    MapData[i, j] = TileType.NOTE;
                else if (c == TreasureColor)
                    MapData[i, j] = TileType.TREASURE;
                else if (c == MachineGunColor)
                    MapData[i, j] = TileType.MACHINE_GUN;
                else if (c == ChainGunColor)
                    MapData[i, j] = TileType.CHAIN_GUN;
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
        POSTER1,
        POSTER2,
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
        MACHINE_GUN,
        CHAIN_GUN,

        SPAWN,

        PASSABLE = SPAWN
    }
}
