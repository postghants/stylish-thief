using System;
using System.Collections.Generic;
using UnityEngine;

public class Card_Wall : MonoBehaviour
{

    public PlayerStateDriver player;
    public float ExplosionForce = 50f;
    public float ExplosionRadius = 5f;
    public float UpwardsMod = 1f;

    [SerializeField] private List<WallPart> Wallpieces;

    void Start()
    {
        foreach (Transform child in transform)
        {
            Wallpieces.Add(child.GetComponent<WallPart>());
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 6)
        {
            if (player.Root.Leaf().ToString() == "HSM.PlayerGrabbing" || player.Root.Leaf().ToString() == "HSM.PlayerPound")
            {
                Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, ExplosionRadius);

                foreach (Collider hit in nearbyColliders)
                {
                    if (hit.TryGetComponent(out WallPart part))
                    {
                        hit.GetComponent<WallPart>().WallBreak(ExplosionForce, transform.position, ExplosionRadius, UpwardsMod);
                    }
                }
            }
        }
    }
}
