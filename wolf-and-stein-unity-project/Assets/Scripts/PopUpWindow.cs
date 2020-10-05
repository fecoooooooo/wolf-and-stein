﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpWindow : MonoBehaviourSingleton<PopUpWindow>
{
    GameObject visuals;
    TextMeshProUGUI messageLbl;
    Button declineButton;
    Button confirmButton;

    void Start()
    {
        visuals = transform.Find("Visuals").gameObject;
        messageLbl = transform.Find("Visuals/MessageLbl").GetComponent<TextMeshProUGUI>();
        declineButton = transform.Find("Visuals/DeclineBtn").GetComponent<Button>();
        confirmButton = transform.Find("Visuals/ConfirmBtn").GetComponent<Button>();

        visuals.SetActive(false);
    }

    public void Show(string message, Action onDecline, Action onConfirm, string declineText = "No", string confirmText = "Yes")
	{
        messageLbl.text = message;

        declineButton.onClick.RemoveAllListeners();
        confirmButton.onClick.RemoveAllListeners();

        declineButton.onClick.AddListener(delegate() { 
            onDecline();
            visuals.SetActive(false);
        });
        confirmButton.onClick.AddListener(delegate() { 
            onConfirm(); 
            visuals.SetActive(false);
        });

        declineButton.GetComponentInChildren<TextMeshProUGUI>().text = declineText;
        confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = confirmText;

        visuals.SetActive(true);
	}
}
