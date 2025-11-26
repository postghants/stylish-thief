using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public float score;


    public void AddScore(float _score)
    {
        score += _score;
    }

    public void RemoveScore(float _score)
    {
        score -= _score;
    }
}
