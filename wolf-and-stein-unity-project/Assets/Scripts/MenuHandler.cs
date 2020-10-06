using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
	MenuItemsHandler mainMenu;

	private void Start()
	{
		mainMenu = GameObject.Find("MainMenu").GetComponent<MenuItemsHandler>();
		mainMenu.EnableInput();
	}

	public void NewGameClicked()
	{
		if (SaveGameHandler.instance.SaveGameExists)
		{
			mainMenu.DisableInput();
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
		mainMenu.DisableInput();
		PopUpWindow.instance.Show("Do you really want to quit", new Action(ConfirmExit), new Action(Empty));
	}

	void ConfirmExit()
	{
		mainMenu.EnableInput();
		Application.Quit();
	}

	void Empty()
	{
		mainMenu.EnableInput();
	}
}
