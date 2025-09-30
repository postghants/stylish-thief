using UnityEngine;

public class PlayerShadowDecal : MonoBehaviour
{
    [SerializeField] private float maxDist;
    [SerializeField] private float verticalOffset;
    [SerializeField] private LayerMask shadowLayers;
    [SerializeField] private ActorPhysics physics;

    //private void Update()
    //{
    //    if (Physics.BoxCast(new Vector3(transform.parent.position.x, transform.parent.position.y + verticalOffset, transform.parent.position.z), physics.environmentCollider.bounds.extents * (1 - physics.skinWidth), Vector3.down, out RaycastHit hit, Quaternion.identity, maxDist, shadowLayers))
    //    {
    //        Vector3 newPos = transform.parent.position;
    //        newPos.y = hit.point.y + verticalOffset;
    //        transform.position = newPos;
    //    }
    //    else
    //    {
    //        transform.localPosition = Vector3.zero;
    //    }
    //}
}
