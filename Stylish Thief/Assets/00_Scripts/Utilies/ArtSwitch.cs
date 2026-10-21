using System;
using UnityEngine;

public class ArtSwitch : MonoBehaviour
{
    [SerializeField][Tooltip("Which set of art assets to show based on the list's index. The first number is always 0, followed by 1, 2, 3, etc")] private int artSetToDisplay;
    [SerializeField][Tooltip("This is a list that should contain all the sets of art for this item. The sets are themselves their own lists. Art assets should be fully separate from the functional assets")] private ArtList[] artLists;

    void Start()
    {
        HideAll();
        foreach (var artObject in artLists[artSetToDisplay].artObjects)
        {
            if (artObject != null)
            {
                artObject.SetActive(true);
            }
        }
    }
    private void HideAll()
    {
        foreach (var artList in artLists)
        {
            foreach (var artObject in artList.artObjects)
            {
                if (artObject != null)
                {
                    artObject.SetActive(false);
                }
            }
        }
    }
}

[Serializable]
public class ArtList
{
    public GameObject[] artObjects;
}