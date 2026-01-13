using UnityEngine;

public class StartTimer : MonoBehaviour
{
    public float startTimer;
    [Header("internal")]
    public bool frozen;
    public float startTime = 0;
    PlayerStateDriver player;
    [HideInInspector] public ChaseUI chaseUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerStateDriver>();
        //player.Machine.ChangeState(player.Root.Leaf(), player.Root.frozen);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime < startTimer)
        {
            startTime += Time.deltaTime;
            Debug.Log(startTime);

            player.Machine.ChangeState(player.Root.Leaf(), player.Root.frozen);
            frozen = true;
        }
        else
        {
            if (frozen)
            {
                player.Machine.ChangeState(player.Root.Leaf(), player.Root.grounded);
            }
            frozen = false;
            Debug.Log(startTime);
            Debug.Log("GO");
        }
    }
}
//Find player
//Freeze them
//Start counting down
//Enable player after countdown
//Maybe enable the whole crime spree manager