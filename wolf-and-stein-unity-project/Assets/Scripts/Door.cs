using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool canInteract;
    Animator animator;
    Collider pathBlockCollider;
    float direction = 0.0f;

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

            bool firstRun = direction == 0.0f;
            if (firstRun)
                direction = 1.0f;
            else
                direction *= -1;

            animator.SetFloat("Direction", direction);
            
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

    public void AnimationReachedFirstFrame()
    {
        bool playingCloseAnim = direction == -1.0f;
        if (playingCloseAnim)
            animator.SetFloat("Direction", 0);
    }

    public void AnimationReachedLastFrame()
    {
        bool playingOpenAnim = direction == 1.0f;
        if (playingOpenAnim)
        {
            animator.SetFloat("Direction", 0);
        }

        pathBlockCollider.enabled = playingOpenAnim ? false : true;
    }
}
