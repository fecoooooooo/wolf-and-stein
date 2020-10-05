using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    public void ExitClicked()
	{
		PopUpWindow.instance.Show("Do you really want to quit", new Action(DeclineExit), new Action(ConfirmExit));
	}

	void ConfirmExit()
	{
		Debug.Log("Exit ye");
	}

	void DeclineExit()
	{
		Debug.Log("Exit no");
	}
}
