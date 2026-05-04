using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LatticeTesting : MonoBehaviour
{
    public Animator mAnimator;
    public string triggerName;
    void Start()
    {
        //mAnimator = GetComponent<Animator>();
    }

    private T GetComponent<T>()
    {
        throw new NotImplementedException();
    }

    void Update()
    {
        //if (mAnimator != null)
        //{
        if(Keyboard.current.pKey.IsPressed())
        {
            Debug.Log("ijsdfkjl");
            mAnimator.SetTrigger(triggerName);
        }
        if (Input.GetKeyDown(KeyCode.P))
            { Debug.Log("PPPPPPP");  //mAnimator.SetTrigger("Trigger");
                //mAnimator.parameters.SetValue("integer", 1);
            }
            //mAnimator.parameters.SetValue("integer", 1);
        //}
    }
}
