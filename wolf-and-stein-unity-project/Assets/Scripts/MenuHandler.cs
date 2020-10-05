using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
	public void NewGameClicked()
	{
		PlayerPrefs.DeleteAll();

		if (SaveGameHandler.instance.SaveGameExists)
		{
			PopUpWindow.instance.Show("Are you sure you want to start a new game? All saved data will be lost?", 
				new Action(ConfirmNewGame), new Action(Empty));
		}
		else
		{
			SceneManager.LoadScene("Game");
		}
	}

	void ConfirmNewGame()
	{
		PlayerPrefs.DeleteAll();
		SceneManager.LoadScene("Game");
	}

	public void ContinueClicked()
	{
		SceneManager.LoadScene("Game");
	}
	public void ExitClicked()
	{
		PopUpWindow.instance.Show("Do you really want to quit", new Action(ConfirmExit), new Action(Empty));
	}

	void ConfirmExit()
	{
		Application.Quit();
	}

	void Empty(){}
}
