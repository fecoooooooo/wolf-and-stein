using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(LevelDataImage))]
public class LevelDataImageEditor : Editor
{
    Camera sceneCamera;
    bool editing = false;
    Texture2D texture;
    LevelDataImage levelImage;

    Texture2D wall1Texture;
    Texture2D wall2Texture;
    Texture2D woodColumTexture;
    Texture2D stoneColumTexture;
    Texture2D doorTexture;
    Texture2D lampTexture;
    Texture2D tunnelTexture;
    Texture2D spawnPosTexture;
    Texture2D foodTexture;
    Texture2D ammoTexture;
    Texture2D keyTexture;
    Texture2D noteTexture;
    Texture2D treasureTexture;

    Color currentColor = Map.Wall1Color;
    bool mouseDown;

    private void Awake()
	{
        SetButtonTexture(ref wall1Texture, Map.Wall1Color);
        SetButtonTexture(ref wall2Texture, Map.Wall2Color);
        SetButtonTexture(ref woodColumTexture, Map.WoodColumnColor);
        SetButtonTexture(ref stoneColumTexture, Map.StoneColumnColor);
        SetButtonTexture(ref doorTexture, Map.DoorColor);
        SetButtonTexture(ref lampTexture, Map.LampColor);
        SetButtonTexture(ref tunnelTexture, Map.TunnelColor);
        SetButtonTexture(ref spawnPosTexture, Map.SpawnPositionColor);
        SetButtonTexture(ref foodTexture, Map.FoodColor);
        SetButtonTexture(ref ammoTexture, Map.AmmoColor);
        SetButtonTexture(ref keyTexture, Map.KeyColor);
        SetButtonTexture(ref noteTexture, Map.NoteColor);
        SetButtonTexture(ref treasureTexture, Map.TreasureColor);
	}

	void SetButtonTexture(ref Texture2D buttonTexture, Color color)
	{
        if (buttonTexture == null)
        {
            buttonTexture = new Texture2D(600, 30, TextureFormat.RGBA32, false);
            Color[] pixels = wall1Texture.GetPixels();
            for (int i = 0; i < pixels.Length; ++i)
                pixels[i] = color; //place this color to a central place
            buttonTexture.SetPixels(pixels);
            buttonTexture.Apply();
        }
    }

	public override void OnInspectorGUI()
	{
        levelImage = (LevelDataImage)target;

        if (editing)
        {
            if (GUILayout.Button("Save"))
            {
                editing = false;

                Debug.Log("Map saved");

                SaveTextureAsPng();
            }
        }
		else
		{
            if (GUILayout.Button("Edit level"))
            {
                editing = true;

                texture = (Texture2D)Resources.Load(levelImage.name);
                levelImage.GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", texture);

                Vector3 position = levelImage.transform.position + new Vector3(0, 0, 2);
                SceneView.lastActiveSceneView.pivot = position;
                SceneView.lastActiveSceneView.rotation = levelImage.transform.rotation;

                SceneView.lastActiveSceneView.Repaint();
            }
        }

        AddColoringButton("Wall 1", wall1Texture, Map.Wall1Color);
        AddColoringButton("Wall 2", wall2Texture, Map.Wall2Color);
        AddColoringButton("Wood column", woodColumTexture, Map.WoodColumnColor);
        AddColoringButton("Stone column", stoneColumTexture, Map.StoneColumnColor);
        AddColoringButton("Door", doorTexture, Map.DoorColor);
        AddColoringButton("Lamp", lampTexture, Map.LampColor);
        AddColoringButton("Tunnel", tunnelTexture, Map.TunnelColor);
        AddColoringButton("Spawn Position", spawnPosTexture, Map.SpawnPositionColor);
        AddColoringButton("Food", foodTexture, Map.FoodColor);
        AddColoringButton("Ammo", ammoTexture, Map.AmmoColor);
        AddColoringButton("Key", keyTexture, Map.KeyColor);
        AddColoringButton("Note", noteTexture, Map.NoteColor);
        AddColoringButton("Treasure", treasureTexture, Map.TreasureColor);
    }

	private void AddColoringButton(string label, Texture2D texture, Color color)
	{
        GUILayout.Label(label);
        if (GUILayout.Button(texture))
            currentColor = color;
    }

	private void SaveTextureAsPng()
	{
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();
        string fullPath = Application.dataPath + "/Data/Levels/Resources/" + levelImage.name + ".png";
        System.IO.File.WriteAllBytes(fullPath, bytes);
    }

	public void OnEnable()
    {
        SceneView.duringSceneGui += CustomOnSceneGUI;
    }

    public void OnDisable()
    {
        SceneView.duringSceneGui -= CustomOnSceneGUI;
    }

    private void CustomOnSceneGUI(SceneView s)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Event e = Event.current;

        if (e.type == EventType.MouseDown)
            mouseDown = true;

        if (e.type == EventType.MouseUp)
            mouseDown = false;

        if (editing)
            Selection.activeGameObject = ((Component)target).gameObject; //Here! Manually assign the selection to be your object
    }


    void OnSceneGUI()
    {
        if (mouseDown && editing)
            ColorPixelUnderMouse();
    }

    void ColorPixelUnderMouse()
	{
        Vector2? uv = GetUVFromMouse();

        if (uv.HasValue)
        {
            int x = (int)(uv.Value.x * texture.width);
            int y = (int)(uv.Value.y * texture.height);
            Color c = texture.GetPixel(x, y);

            if (c != currentColor)
            {
                texture.SetPixel(x, y, currentColor);
                texture.Apply();
            }
        }
    }

    void SetAllObjectSelectableState(bool isLocked)
	{
        int lockedLayer = LayerMask.NameToLayer("Locked");
        Tools.lockedLayers = Tools.lockedLayers | (1 << lockedLayer);

        foreach (GameObject go in FindObjectsOfType(typeof(GameObject)))
		{
            if(go != levelImage.gameObject)
                go.layer = isLocked ? lockedLayer : 0;
		}
    }

    Vector2? GetUVFromMouse()
	{
        Vector3 mousePosition = Event.current.mousePosition;
        Ray worldRay = HandleUtility.GUIPointToWorldRay(mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(worldRay, out hitInfo, Mathf.Infinity))
        {
            if (hitInfo.collider.gameObject != null)
            {
                if (hitInfo.collider.gameObject == levelImage.gameObject)
                {
                    var min = levelImage.GetComponent<MeshRenderer>().bounds.min;
                    var max = levelImage.GetComponent<MeshRenderer>().bounds.max;

                    float u = (hitInfo.point.x - min.x) / (max.x - min.x);
                    float v = (hitInfo.point.y - min.y) / (max.y - min.y);

                    return new Vector2(u, v);
                }
            }
        }

        return null;
    }
}
