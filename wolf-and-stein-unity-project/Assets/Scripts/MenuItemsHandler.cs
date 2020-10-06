using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemsHandler : MonoWithInput
{
    MenuButton[] menuItems;
    Image selectorImage;
    int selectedIndex = 0;

    int FirstSelectableElement
    {
        get
        {
            for (int i = 0; i < menuItems.Length; ++i)
            {
                if (!menuItems[i].Inactive)
                    return i;
            }
            throw new Exception("All menuitems cannot be unselectable");
        }
    }

    int LastSelectableElement {
        get
        {
            for(int i = menuItems.Length - 1; i >= 0; --i)
			{
                if (!menuItems[i].Inactive)
                    return i;
			}
            throw new Exception("All menuitems cannot be unselectable");
        }
    }

    void Start()
    {
        selectorImage = transform.Find("SelectorImg").GetComponent<Image>();
        menuItems = GetComponentsInChildren<MenuButton>();

        for (int i = 0; i < menuItems.Length; ++i)
        {
            int buttonIndex = i;
            menuItems[i].GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
            {
                ButtonClickedWithMouse(buttonIndex);
            }));
        }
        
        SelectWithOffset(0);
    }

    void ButtonClickedWithMouse(int index)
	{
        selectedIndex = index;
        UpdateUI();
	}

	private void SelectWithOffset(int offset)
	{
        selectedIndex += offset;
        if (selectedIndex < 0)
            selectedIndex = LastSelectableElement;
        else if(selectedIndex >= menuItems.Length)
            selectedIndex = FirstSelectableElement;
        UpdateUI();
    }

	private void UpdateUI()
	{
        foreach(MenuButton mi in menuItems)
            mi.Deselect();

        menuItems[selectedIndex].Select();

        selectorImage.transform.position = new Vector3(selectorImage.transform.position.x, menuItems[selectedIndex].transform.position.y, 
            selectorImage.transform.position.z);
	}

	public override void HandleInput()
	{
        if (Input.GetKeyDown(KeyCode.UpArrow))
            SelectWithOffset(-1);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            SelectWithOffset(1);
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            menuItems[selectedIndex].InvokeClick();
    }
}

