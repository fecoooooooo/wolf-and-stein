using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    TextMeshProUGUI LevelLabel;
    TextMeshProUGUI ScoreLabel;
    TextMeshProUGUI LivesLabel;
    TextMeshProUGUI HealthLabel;
    TextMeshProUGUI AmmoLabel;
    TextMeshProUGUI KeysLabel;
    TextMeshProUGUI NotesLabel;

    void Start()
    {
        LevelLabel  = transform.Find("LevelLabel").GetComponent<TextMeshProUGUI>();
        ScoreLabel  = transform.Find("ScoreLabel").GetComponent<TextMeshProUGUI>();
        LivesLabel  = transform.Find("LivesLabel").GetComponent<TextMeshProUGUI>();
        HealthLabel = transform.Find("HealthLabel").GetComponent<TextMeshProUGUI>();
        AmmoLabel   = transform.Find("AmmoLabel").GetComponent<TextMeshProUGUI>();
        KeysLabel   = transform.Find("KeysLabel").GetComponent<TextMeshProUGUI>();
        NotesLabel  = transform.Find("NotesLabel").GetComponent<TextMeshProUGUI>();

        Character.instance.ShouldUpdateUI += OnUpdateUI;
    }

    private void OnUpdateUI(object sender, EventArgs e)
	{
        LevelLabel.text = Map.instance.CurrentLevel.ToString();
        ScoreLabel.text = Character.instance.Score.ToString();
        LivesLabel.text = Character.instance.Lives.ToString();
        HealthLabel.text = Character.instance.HP.ToString() + "%";
        AmmoLabel.text = Character.instance.Ammo.ToString();
        KeysLabel.text = Character.instance.Keys.ToString();
        NotesLabel.text = Character.instance.Notes.ToString();
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
