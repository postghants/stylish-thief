using System;
using System.Collections.Generic;
using UnityEngine;

public class WallPart : MonoBehaviour
{
    Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }


    public void WallBreak(float Force, Vector3 pos, float Rad, float Up)
    {
        body.AddExplosionForce(Force, pos, Rad, Up);
    }

}
