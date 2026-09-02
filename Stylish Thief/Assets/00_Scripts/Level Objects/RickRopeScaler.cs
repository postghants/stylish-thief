using UnityEngine;

public class RickRopeScaler : MonoBehaviour
{
    [SerializeField] Transform start;
    [SerializeField] Transform end;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float distance = Vector3.Distance(start.position, end.position);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, distance);
        transform.position = (start.position + end.position) / 2;
        transform.LookAt(end.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
//Grab both transforms
//Get their distance
//Set scale to half of that
//Find the middle point between transforms
//Place it there