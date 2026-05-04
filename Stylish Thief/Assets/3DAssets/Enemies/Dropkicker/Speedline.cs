using UnityEngine;
using System.Collections;

public class Speedline : MonoBehaviour //Can be referred to by unity if on object.
{
    public ParticleSystem GodSpeedline; //Stores reference to object
    public void StartVFX() // (function), can be called from somewhere else to do certain thing
    {
        GodSpeedline.Play(); //(play = function) 
    }
}
