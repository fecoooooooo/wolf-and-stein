using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
	public void ContinueClicked()
	{
		SceneManager.LoadScene("Game");
	}
	public void ExitClicked()
	{
		PopUpWindow.instance.Show("Do you really want to quit", new Action(DeclineExit), new Action(ConfirmExit));
	}

	void ConfirmExit()
	{
		Application.Quit();
	}

	void DeclineExit()
	{
	}
}
