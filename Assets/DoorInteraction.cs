using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    // [SerializeField] AnimationClip doorOpen;
    //[SerializeField] AnimationClip doorClose;
    [SerializeField] Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (gameObject.tag == "Isolation")
            {
                if (animator.GetBool("Open"))
                {
                    animator.SetBool("Open", false);
                    animator.SetBool("Close", true);
                }

            }
            else if (gameObject.tag == "UpDoor")
            {
                if (!animator.GetBool("Open"))
                {
                    animator.SetTrigger("FastOpen");
                    //animator.SetBool("Open", true);      
                }

            }
            else
            {
               animator.SetBool("Close", false);
                animator.SetBool("Open", true);
            }

            //  animator.Play("doorOpen");
            // doorOpen.Play();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // animator.SetTrigger("Close");
            // animator.Play("doorClose");
            // doorClose.Play();
        }
    }
}
