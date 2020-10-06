using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpWindow : MonoWithInputSingleton<PopUpWindow>
{
    GameObject visuals;
    TextMeshProUGUI messageLbl;
    Button declineButton;
    Button confirmButton;

	public bool Showing { get => visuals.activeSelf; }

	void Start()
    {
        Transform rootCanvas = transform.parent;
        DontDestroyOnLoad(rootCanvas.gameObject);

        visuals = transform.Find("Visuals").gameObject;
        messageLbl = transform.Find("Visuals/MessageLbl").GetComponent<TextMeshProUGUI>();
        declineButton = transform.Find("Visuals/DeclineBtn").GetComponent<Button>();
        confirmButton = transform.Find("Visuals/ConfirmBtn").GetComponent<Button>();

        visuals.SetActive(false);
    }

	public void Show(string message, Action onConfirm, Action onDecline, string confirmText = "Yes", string declineText = "No")
	{
        EnableInput();

        messageLbl.text = message;

        confirmButton.onClick.RemoveAllListeners();
        declineButton.onClick.RemoveAllListeners();

        
        confirmButton.onClick.AddListener(delegate() { 
            visuals.SetActive(false);
            DisableInput();
            onConfirm(); 
        });

        declineButton.onClick.AddListener(delegate () {
            visuals.SetActive(false);
            DisableInput();
            onDecline();
        });

        confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = confirmText;
        declineButton.GetComponentInChildren<TextMeshProUGUI>().text = declineText;

        visuals.SetActive(true);
	}

	public override void HandleInput()
	{
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            confirmButton.onClick.Invoke();
        if (Input.GetKeyDown(KeyCode.Escape))
            declineButton.onClick.Invoke();
    }
}
