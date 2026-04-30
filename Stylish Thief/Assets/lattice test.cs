using System;
using UnityEngine;

public class latticetest
{
    private Animator mAnimator;
    void Start()
    {
        mAnimator = GetComponent<Animator>();
    }

    private T GetComponent<T>()
    {
        throw new NotImplementedException();
    }

    void Update ()
    {
        if(mAnimator != null)
        {
            if (Input.GetKeyDown(KeyCode.P))
            { mAnimator.SetTrigger("Trigger"); }
           
        }
    }
}
