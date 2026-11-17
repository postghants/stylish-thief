using System.Collections.Generic;
using UnityEngine;

public class AnimationControlHolder : StateMachineBehaviour
{
    [SerializeField] public List<string> overrideTriggers;
    [SerializeField] protected string exitState;
}
