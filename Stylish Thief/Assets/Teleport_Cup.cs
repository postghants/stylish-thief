using System.Collections.Generic;
using UnityEngine;

public class Teleport_Cup : MonoBehaviour
{

    public bool PonP;
    private Collider Telly;
    public List<GameObject> Cups;
    public PlayerStateDriver player;
    public Transform TPpos;


    void Start()
    {
        Telly = GetComponent<Collider>();
        PonP = false; 
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 && PonP == false)
        {
            int a = UnityEngine.Random.Range(0, Cups.Count - 1);
            int b = Cups.IndexOf(this.gameObject);
            if (a >= b)
            {
                a++;
            }

            TPpos = Cups[a].transform;
            player.transform.position = TPpos.position;
            Cups[a].GetComponent<Teleport_Cup>().PonP = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            PonP = false;
        }
    }
}
