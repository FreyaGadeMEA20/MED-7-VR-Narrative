using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRootMotionFix : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Assuming the Animator component is attached to the same GameObject as this script
        animator = GetComponent<Animator>();

        // Enable root motion
        EnableRootMotion();
    }

    void EnableRootMotion()
    {
        if (animator != null)
        {
            animator.applyRootMotion = true;
        }
    }
}