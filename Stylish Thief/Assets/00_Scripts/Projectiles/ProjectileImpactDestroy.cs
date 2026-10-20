using UnityEngine;

public class ProjectileImpactDestroy : MonoBehaviour
{
    public LayerMask layers;
    private void OnTriggerEnter(Collider other)
    {
        if (this.isActiveAndEnabled)
        {
            if ((layers & (1 << other.gameObject.layer)) != 0) //No I don't know what this means and I'm not about to find out
            {
                Debug.Log("Layer recognised");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Layer not in mask");
            }
        }
    }
}
