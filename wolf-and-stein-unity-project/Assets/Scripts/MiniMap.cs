using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    const int VISION_DISTANCE = 6;
    readonly Color UnrevealedColor = new Color(.5f, .5f, .5f, 0.5f);

    public Texture2D DynamicMinimapTexture { get; private set; }
    public Texture2D StaticMinimapTexture { get; private set; }
    public EventHandler MinimapUpdated;

    Vector2Int playerCoords;
    Vector2Int? prevPlayerCoords;
    Color previousColorOnPlayerCoord = new Color(1f, 1f, 1f, .5f);
    
    bool[,] revealedMap;

    Map _map;
    Map map 
    {
		get
		{
            if (null == _map)
                _map = GetComponent<Map>();
            return _map;
		}
    }

	void Update()
    {
        if (!Application.isPlaying)
            return;

        playerCoords = GetPlayerCoords();

        if (playerCoords != prevPlayerCoords || Door.DoorOpened)
		{
            UpdateMiniMap();
            Door.DoorOpened = false;
        }

        prevPlayerCoords = playerCoords;
    }

    private void UpdateMiniMap()
    {
        RevealSurroundings();
        DrawPlayerPosition();

        DynamicMinimapTexture.Apply();

        MinimapUpdated?.Invoke(this, null);
    }

    private void DrawPlayerPosition()
    {
        if (prevPlayerCoords.HasValue)
        {
            DynamicMinimapTexture.SetPixel(prevPlayerCoords.Value.x, prevPlayerCoords.Value.y, previousColorOnPlayerCoord);
            previousColorOnPlayerCoord = DynamicMinimapTexture.GetPixel(playerCoords.x, playerCoords.y);
        }

        DynamicMinimapTexture.SetPixel(playerCoords.x, playerCoords.y, Color.red);
    }

    private void RevealSurroundings()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Wall");

        for (int i = -VISION_DISTANCE; i <= VISION_DISTANCE; ++i)
		{
            for(int j = -VISION_DISTANCE; j <= VISION_DISTANCE; ++j)
			{
                int x = playerCoords.x + i;
                int y = playerCoords.y + j;

                //ha nemtom ezt hogy kene igazabol szal kiserletezessel igy kijött heha
                int mapCoordX = y;
                int mapCoordY = x;

                if (map.IsValidCoord(x, y))
				{
                    if (revealedMap[x, y])
                        continue;

                    if (map.IsPassableOnCoord(mapCoordX, mapCoordY))
					{
                        /*var a = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        a.GetComponent<Collider>().enabled = false;
                        a.transform.position = new Vector3(x, .5f, -y);
                        Destroy(a, 3);*/

                        if (x == playerCoords.x && y == playerCoords.y)
                        {
                            RevealCoordinate(x, y);
                            continue;
                        }

                        if (false == Physics.Linecast(Character.instance.transform.position, new Vector3(x, .5f, -y), layerMask, QueryTriggerInteraction.Ignore))
                            RevealCoordinate(x, y);
					}
					else
                        revealedMap[x, y] = true;
				}
			}
		}
    }

	private void RevealCoordinate(int x, int y)
	{
        Color newColor = StaticMinimapTexture.GetPixel(x, y);
        newColor.a = .5f;
        DynamicMinimapTexture.SetPixel(x, y, newColor);
        revealedMap[x, y] = true;
    }

	private Vector2Int GetPlayerCoords()
    {
        int x = Mathf.FloorToInt(Character.instance.transform.position.x);
        x += Character.instance.transform.position.x % 1 < .5f ? 0 : 1;

        float absZ = Mathf.Abs(Character.instance.transform.position.z); //since Z goes on the negative direction
        int y = Mathf.FloorToInt(absZ);
        y += absZ % 1 < .5f ? 0 : 1;

        return new Vector2Int(x, y);
    }

    public void GenerateMaps()
	{
        GenerateStaticMinimap();
        GenerateDynamicMinimap();
        GenerateRevealedMap();
    }

	private void GenerateRevealedMap()
	{
        revealedMap = new bool[map.MapData.GetLength(0), map.MapData.GetLength(1)];
    }

    void GenerateStaticMinimap()
    {
        StaticMinimapTexture = new Texture2D(map.MapData.GetLength(0), map.MapData.GetLength(1));
        StaticMinimapTexture.filterMode = FilterMode.Point;
        StaticMinimapTexture.anisoLevel = 1;
        StaticMinimapTexture.mipMapBias = -0.5f;

        Color[] colors = new Color[StaticMinimapTexture.width * StaticMinimapTexture.height];

        for (int i = 0; i < map.MapData.GetLength(0); ++i)
        {
            for (int j = 0; j < map.MapData.GetLength(1); ++j)
                colors[i * map.MapData.GetLength(0) + j] = map.IsPassableOnCoord(i, j) ? Color.white : Color.black;
        }

        StaticMinimapTexture.SetPixels(colors);
        StaticMinimapTexture.Apply();
    }

    void GenerateDynamicMinimap()
    {
        DynamicMinimapTexture = new Texture2D(map.MapData.GetLength(0), map.MapData.GetLength(1));
        DynamicMinimapTexture.filterMode = FilterMode.Point;
        DynamicMinimapTexture.anisoLevel = 1;
        DynamicMinimapTexture.mipMapBias = -0.5f;

        Color[] colors = new Color[StaticMinimapTexture.width * StaticMinimapTexture.height];

        for (int i = 0; i < map.MapData.GetLength(0); ++i)
        {
            for (int j = 0; j < map.MapData.GetLength(1); ++j)
                colors[i * map.MapData.GetLength(0) + j] = UnrevealedColor;
        }

        DynamicMinimapTexture.SetPixels(colors);
        DynamicMinimapTexture.Apply();
    }
}
