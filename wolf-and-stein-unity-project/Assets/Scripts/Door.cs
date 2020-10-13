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

	public static bool DoorOpened;

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
		if (Input.GetKeyDown(KeyCode.Space) && CanInteract())
			Toggle();
	}

	public void Open()
	{
		animDirection = 1;
		animator.SetFloat("Direction", animDirection);
	}

	public void Close()
	{
		animDirection = -1;
		animator.SetFloat("Direction", animDirection);
	}

	public void Toggle()
	{
		animDirection *= -1;
		animator.SetFloat("Direction", animDirection);
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

	internal Vector3 GetRotationAfterPass(Vector3 positionOfAsker)
	{
		switch (GetOrientationToMe(positionOfAsker))
		{
			case Orientation.UnderMe:
				return new Vector3(0, 0, 0);
			case Orientation.OverMe:
				return new Vector3(0, 180, 0);
			case Orientation.LeftToMe:
				return new Vector3(0, 90, 0);
			case Orientation.RightToMe:
				return new Vector3(0, -90, 0);
			default:
				throw new Exception("Cant decide orientation");
		}
	}

	internal Vector3 GetFartherPoint(Vector3 positionOfAsker)
	{
		switch (GetOrientationToMe(positionOfAsker))
		{
			case Orientation.UnderMe:
				return transform.position + new Vector3(0, 0, .5f);
			case Orientation.OverMe:
				return transform.position - new Vector3(0, 0, .5f);
			case Orientation.LeftToMe:
				return transform.position + new Vector3(.5f, 0, 0);
			case Orientation.RightToMe:
				return transform.position - new Vector3(.5f, 0, 0);
			default:
				throw new Exception("Cant decide orientation");
		}
	}
	internal Vector3 GetHitherPoint(Vector3 positionOfAsker)
	{
		switch (GetOrientationToMe(positionOfAsker))
		{
			case Orientation.UnderMe:
				return transform.position - new Vector3(0, 0, .5f);
			case Orientation.OverMe:
				return transform.position + new Vector3(0, 0, .5f);
			case Orientation.LeftToMe:
				return transform.position - new Vector3(.5f, 0, 0);
			case Orientation.RightToMe:
				return transform.position + new Vector3(.5f, 0, 0);
			default:
				throw new Exception("Cant decide orientation");
		}
	}

	Orientation GetOrientationToMe(Vector3 positionOfAsker)
	{
		bool horizontalDoor = transform.forward.x != 0;

		if (horizontalDoor)
		{
			float orientation = Mathf.Sign(transform.position.x - positionOfAsker.x);
			bool toTheLeft = orientation == 1;

			if (toTheLeft)
				return Orientation.LeftToMe;
			else
				return Orientation.RightToMe;
		}
		else //verticalDoor
		{
			float orientation = Mathf.Sign(transform.position.z - positionOfAsker.z);
			bool toBottom = orientation == 1;

			if (toBottom)
				return Orientation.UnderMe;
			else
				return Orientation.OverMe;
		}
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
			animator?.SetFloat("Direction", 0);
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
			DoorOpened = true;
		}

		pathBlockCollider.isTrigger = openingDoor ? true : false;
	}

	enum Orientation
	{
		UnderMe,
		OverMe,
		LeftToMe,
		RightToMe
	}

}
