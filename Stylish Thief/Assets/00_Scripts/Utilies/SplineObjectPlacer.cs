using Alchemy.Inspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class SplineObjectPlacer : MonoBehaviour
{
    [OnValueChanged("SpawnObjects")]
    [SerializeField] private bool useRotation;
    [OnValueChanged("SpawnObjects")]
    [SerializeField] private GameObject prefab;
    [OnValueChanged("SpawnObjects")]
    [SerializeField] private float count;

    private List<GameObject> objects = new();
    private SplineContainer spline;

    [Button]
    private void SpawnObjects()
    {
        if (count < 1) { return; }
        if (spline == null) { spline = GetComponent<SplineContainer>(); }

        foreach (GameObject obj in objects)
        {
            DestroyImmediate(obj);
        }

        float t = 0;
        while (t < 1)
        {
            GameObject o = Instantiate(prefab, spline.EvaluatePosition(t), Quaternion.identity);
            if (useRotation)
            {
                o.transform.LookAt(transform.position + (Vector3)spline.EvaluateTangent(t));
            }
            objects.Add(o);

            t += 1 / count;
        }
    }
}
