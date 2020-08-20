using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickable : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == Character.instance.gameObject)
			OnPickUp();

		Destroy(gameObject);
	}

	public abstract void OnPickUp();
}
