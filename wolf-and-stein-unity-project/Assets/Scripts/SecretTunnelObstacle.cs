using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretTunnelObstacle : MonoBehaviour
{
	const float SPEED = 0.3f;

	bool opened;
	bool canInteract;
	float BlocksToMove { get => transform.childCount; }
	Vector3 direction;

	public void Init(Vector3 direction, GameObject obstacleDoor)
	{
		this.direction = direction;
		transform.position = obstacleDoor.transform.position;
	}

	private void Update()
	{
		if (opened)
			return;

		if (canInteract && Input.GetKeyDown(KeyCode.Space))
			StartCoroutine(Open());
	}


	IEnumerator Open()
	{
		opened = true;

		float amountMoved = 0;
		Vector3 originalPosition = transform.position;

		while(amountMoved <= BlocksToMove)
		{
			amountMoved += SPEED * Time.deltaTime;
			transform.position = originalPosition  + direction * amountMoved;

			yield return null;
		}

		transform.position = originalPosition  + direction * BlocksToMove;
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
