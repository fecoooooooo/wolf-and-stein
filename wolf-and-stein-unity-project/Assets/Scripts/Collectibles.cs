using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Collectibles : MonoBehaviourSingleton<Collectibles>
{
    //THERE ARE 2 COLLECTIBLES ON EACH MAP, THE PLAYER GETS A RANDOM COLLECTIBLE BASED ON LEVEL
    //EXAMPLE: Level 2, picking up a note results in index 2 or 3

    const string PLAYER_PREFS_PREFIX = "Collectible_";
    const string NOT_YET_FOUND_TEXT = "No entries found yet.";

    bool FoundAny { get => MaxFoundEntryIndex != -1; }
    public int FoundEntriesCount { get => entries.Where(x => x.Found).Count(); }

    public int MaxFoundEntryIndex { get
        {
            int maxIndex = -1;
            int i = -1;

            foreach(var e in entries)
			{
                ++i;

                if (e.Found)
                    maxIndex = i;
			}

            return maxIndex;
        }
    }
    public int CurrentlySelectedIndex { get; set; } 

	List<(bool Found, string Entry)> entries = new List<(bool Found, string Entry)>();

    GameObject rootObject;
    TextMeshProUGUI entryLbl;
    TextMeshProUGUI currentLbl;

    void Start()
    {
        rootObject = transform.Find("Root").gameObject;
        entryLbl = rootObject.transform.Find("EntryLbl").GetComponent<TextMeshProUGUI>();
        currentLbl = rootObject.transform.Find("CurrentLbl").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ToggleVisibility();

        if (rootObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Disable();
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                Previous();
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                Next();
        }
    }

    public void Enable()
	{
        UpdateUI();
        Time.timeScale = 0;
        rootObject.SetActive(true);
    }

    public void Disable()
	{
        Time.timeScale = 1;
        rootObject.SetActive(false);
    }

    public void ToggleVisibility()
	{
        if (rootObject.activeSelf)
            Disable();
        else
            Enable();
    }

    public void Previous()
	{
        if (!FoundAny)
            return;

        CurrentlySelectedIndex = CurrentlySelectedIndex - 1 < 0 ? MaxFoundEntryIndex : CurrentlySelectedIndex - 1;
        UpdateUI();
	}

    public void Next()
	{
        if (!FoundAny)
            return;

        CurrentlySelectedIndex = CurrentlySelectedIndex + 1 > MaxFoundEntryIndex ? 0 : CurrentlySelectedIndex + 1;
        UpdateUI();
    }

    //THERE ARE 2 COLLECTIBLES ON EACH MAP, THE PLAYER GETS THE FIRST THAN THE SECOND COLLECTIBLE BASED ON LEVEL
    //EXAMPLE: Level 2, picking up a note results in index 2 or 3
    public void AddCollectible()
    {
        int index = (Map.instance.CurrentLevel - 1) * 2;
        if (entries[index].Found)
            index++;

        PlayerPrefs.SetInt(PLAYER_PREFS_PREFIX + index, 1);
        entries[index] = (Found: true, Entry: entries[index].Entry);
        CurrentlySelectedIndex = index;
    }

    public void Load()
	{
        string[] entriesRead = Resources.Load<TextAsset>("CollectibleEntries").text.Split('\n');

        for (int i = 0; i < entriesRead.Length; ++i)
        {
            bool found = PlayerPrefs.GetInt(PLAYER_PREFS_PREFIX + i, 0) == 1 ? true : false;
            entries.Add((Found: found, entriesRead[i]));
        }

        CurrentlySelectedIndex = MaxFoundEntryIndex;
    }

    void UpdateUI()
	{
        if(FoundAny)
		{
            currentLbl.text = (CurrentlySelectedIndex + 1)+ "/" + (MaxFoundEntryIndex + 1);
            entryLbl.text = entries[CurrentlySelectedIndex].Found ? entries[CurrentlySelectedIndex].Entry : "???";
        }
		else
		{
            currentLbl.text = "0/0";
            entryLbl.text = NOT_YET_FOUND_TEXT;
        }
	}
}
