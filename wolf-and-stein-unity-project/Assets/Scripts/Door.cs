using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool canInteract;
    Animator animator;
    Collider pathBlockCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        pathBlockCollider = GetComponents<Collider>().FirstOrDefault(t => !t.isTrigger);
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            var currentAnimations = animator.GetCurrentAnimatorClipInfo(0);

            if(currentAnimations.Length > 0 && currentAnimations[0].clip.name == "DoorOpen")
                pathBlockCollider.enabled = true;

            animator.SetTrigger("Interact");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (null == other.GetComponent<CharController>())
            return;

        canInteract = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (null == other.GetComponent<CharController>())
            return;

        canInteract = false;
    }

    public void OnOpenAnimEnded()
    {
        pathBlockCollider.enabled = false;
    }

    public void OnCloseAnimStarted()
    {
        //why is this called at the end of the animation instead of start as it is set in the animation
        pathBlockCollider.enabled = true;
        Debug.Log("close started");
    }
}
