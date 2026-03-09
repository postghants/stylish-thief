using UnityEngine;

public class RotatingCylinder : MonoBehaviour
{
    [SerializeField] private Vector3 _rotation;

    void Update()
    {
        transform.Rotate(_rotation * Time.deltaTime);
    }
}
