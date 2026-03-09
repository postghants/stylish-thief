using UnityEngine;

public class GrabCheckerrrrr : MonoBehaviour
{
    public PlayerStateDriver states;
    public PlayerContext cont;
    public MeshRenderer mesh;
    public Transform model;
    public Vector3 startScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startScale = model.transform.localScale;

    }

    // Update is called once per frame
    void Update()
    {
        if (cont.grabTimer > 0)
        {
            //mesh.material.color = new Color(255, 255, 255);
            model.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else
        {

            model.transform.localScale = startScale;
            //mesh.material.color = new Color(255, 61, 106);
        }
    }
}
