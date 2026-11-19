using Alchemy.Inspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class SplineObjectPlacer : MonoBehaviour
{
    [Header("For permanent trails")]
    [OnValueChanged("SpawnObjects")]
    [SerializeField] private bool useRotation;
    [OnValueChanged("SpawnObjects")]
    [SerializeField] private GameObject prefab;
    [OnValueChanged("SpawnObjects")]
    [SerializeField] private float count;

    [Header("For triggered trails")]
    public bool spawnOnTrigger;
    public float fullSpawnTime;
    private float timer;
    public float distance;
    private float nextSpawnPoint = 0;
    public bool triggered;

    private List<GameObject> objects = new();
    private SplineContainer spline;

    private void Start()
    {
        if (spline == null) { spline = GetComponent<SplineContainer>(); }
        if (spawnOnTrigger)
        {
            objects.Clear();
        }
    }

    private void Update()
    {
        if (spawnOnTrigger && triggered)
        {
            if (timer < fullSpawnTime)
            {
                float splinePos = timer / fullSpawnTime;
                if (nextSpawnPoint <= 1)
                {
                    if (splinePos > nextSpawnPoint)
                    {
                        GameObject o = Instantiate(prefab, spline.EvaluatePosition(splinePos), Quaternion.identity);
                        nextSpawnPoint += distance;
                        if (useRotation)
                        {
                            o.transform.LookAt(transform.position + (Vector3)spline.EvaluateTangent(splinePos));
                        }
                        objects.Add(o);
                    }
                    timer += Time.deltaTime;
                }
            }

        }
    }

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
