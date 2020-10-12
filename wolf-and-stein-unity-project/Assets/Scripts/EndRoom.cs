using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndRoom : MonoBehaviour
{
	bool canInteract;

	private void Update()
	{
		if(canInteract && Input.GetKeyDown(KeyCode.Space))
		{
			//TODO: do animation
			Debug.Log("EndLevel");
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == Character.instance.gameObject)
			canInteract = true;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == Character.instance.gameObject)
			canInteract = false;
	}
}
