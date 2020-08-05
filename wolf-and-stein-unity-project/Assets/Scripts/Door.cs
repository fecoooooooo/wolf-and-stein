using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
	Animator animator;
	Collider pathBlockCollider;
	Bounds canInteractBounds;
	float animDirection = -1.0f;
	List<Collider> currentlyCollidingWithPathBlocker = new List<Collider>();

	bool shouldAutoCloseDoor = false;
	float timeTillDoorClose = 0;

	void Start()
	{
		animator = GetComponent<Animator>();
		
		pathBlockCollider = GetComponents<Collider>().FirstOrDefault(t => !t.isTrigger);
		
		Collider canInteractCollider = GetComponents<Collider>().FirstOrDefault(t => t.isTrigger);
		canInteractBounds = canInteractCollider.bounds;
		canInteractCollider.enabled = false;
	}

	void Update()
	{
		HandleInteraction();
		HandleAutoClose();
	}

	private void HandleInteraction()
	{
		if (Input.GetKeyDown(KeyCode.E) && CanInteract())
		{
			animDirection *= -1;
			animator.SetFloat("Direction", animDirection);
		}
	}

	private void HandleAutoClose()
	{
		if (shouldAutoCloseDoor)
		{
			timeTillDoorClose -= Time.deltaTime;
		
			if (0 >= timeTillDoorClose && currentlyCollidingWithPathBlocker.Count == 0)
			{
				animDirection *= -1;
				animator.SetFloat("Direction", animDirection);
				shouldAutoCloseDoor = false;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		currentlyCollidingWithPathBlocker.Add(other);
	}

	private void OnTriggerExit(Collider other)
	{
		currentlyCollidingWithPathBlocker.Remove(other);
	}

	private bool CanInteract()
	{
		return canInteractBounds.Intersects(Character.instance.GetComponent<CapsuleCollider>().bounds);
	}

	public void AnimationReachedFirstFrame()
	{
		bool closingDoor = animDirection == -1.0f;
		if (closingDoor)
		{
			animator.SetFloat("Direction", 0);
			shouldAutoCloseDoor = false;
			timeTillDoorClose = -1f;
		}
	}

	public void AnimationReachedLastFrame()
	{
		bool openingDoor = animDirection == 1.0f;
		if (openingDoor)
		{
			animator.SetFloat("Direction", 0);
			timeTillDoorClose = GamePreferences.Instance.DoorAutoCloseTime;
			shouldAutoCloseDoor = true;
		}

		pathBlockCollider.isTrigger = openingDoor ? true : false;
	}
}
