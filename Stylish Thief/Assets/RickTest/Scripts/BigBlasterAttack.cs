using HSM;
using System;
using UnityEngine;
using UnityEngine.AI;

public class BigBlasterAttack : EnemyAttack
{
    [Header("Big Blast")]
    public bool fired;

    [Header("References")]
    private NavMeshAgent agent;
    [SerializeField] private Transform rotationTf;
    [SerializeField] private BigBlast projectile;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (ctr == null)
        {
            if (TryGetComponent(out EnemyController controller))
            {
                ctr = controller;
            }
        }
    }

    public override void OnEnter()
    {
        agent.enabled = false;
        fired = true;
        projectile.gameObject.SetActive(true);
    }

    public override void OnUpdate(float deltaTime)
    {
        if (fired)
        {
            if (!projectile.gameObject.activeSelf)
            {
                ctr.ExitAttack(this);
            }
        }
    }
    public void OnHit()
    {
        //ctr.ctx.player.TakeKnockback(new Vector3(1, 1, 1));
        //ctr.ctx.player.TakeDamage(damage);
        Debug.Log("I acshually hit her! I mean of courshe! Jusht to be exshpected from my foolproof calculashionsh");
    }
    public override void OnExit()
    {
        fired = false;
        agent.enabled = true;
    }
    protected override void Reset()
    {
        base.Reset();
    }

    public override void OnBehaviourDeactivate()
    {
        throw new NotImplementedException();
    }

    public override void OnBehaviourReactivate()
    {
        throw new NotImplementedException();
    }
}


//Uses Jumper system to follow you
//Faster than player
//Chooses a random spot around you to pathfind towards
//When area reached, do a full attack

//To do in this script:
        //When attacking, disable pathfinding
//Then spawn or enable the telegraph decal (maybe spawn the whole attack separately but instantly make it a child of the enemy to make tracking work easily) (def do enable to prevent lag spikes)
//Then track player horizontally for a few seconds, rotating the decal too
//Then stop tracking for a few seconds
//Then spawn or enable the beam
//When the beam stops, wait a few seconds
//End script and activate pathfinding... in opposite order