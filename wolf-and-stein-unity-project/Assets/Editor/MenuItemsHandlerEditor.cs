using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(MenuItemsHandler))]
public class MenuItemsHandlerEditor : Editor
{
    float gapBetweenItems = 60f;
    float selectorOffset = 30f;

    MenuButton[] menuItems;
    Image selectorImage;

    public override void OnInspectorGUI()
    {
        MenuItemsHandler menuItemsHandler = (MenuItemsHandler)target;
        
        selectorImage = menuItemsHandler.transform.Find("SelectorImg").GetComponent<Image>();
        menuItems = menuItemsHandler.GetComponentsInChildren<MenuButton>();

        DrawDefaultInspector();

        EditorGUI.BeginChangeCheck();
        gapBetweenItems = EditorGUILayout.FloatField("Gap between items", gapBetweenItems);
        selectorOffset = EditorGUILayout.FloatField("Selector offset", selectorOffset);
        if (EditorGUI.EndChangeCheck())
            PositionElements();
    }

    private void PositionElements()
    {
        for (int i = 0; i < menuItems.Length; ++i)
            menuItems[i].transform.localPosition = new Vector3(0, -i * gapBetweenItems, 0);

        selectorImage.transform.localPosition = new Vector3(-selectorOffset, menuItems[0].transform.localPosition.y, 0);
    }
}
