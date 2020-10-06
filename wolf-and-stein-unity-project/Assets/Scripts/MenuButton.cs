using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    readonly Color NORMAL_COLOR = new Color32(180, 0, 0, 255);
    readonly Color SELECTED_COLOR = new Color32(236, 31, 31, 255);

    TextMeshProUGUI label;

	public bool Inactive { get; internal set; }

	void Awake()
    {
        label = GetComponentInChildren<TextMeshProUGUI>();
    }


	internal void Deselect()
	{
        label.color = NORMAL_COLOR;
	}

	internal void Select()
	{
        label.color = SELECTED_COLOR;
	}

	internal void InvokeClick()
	{
		GetComponent<Button>().onClick.Invoke();
	}
}
