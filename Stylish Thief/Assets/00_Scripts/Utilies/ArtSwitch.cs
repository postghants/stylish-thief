using System;
using UnityEngine;

public class ArtSwitch : MonoBehaviour
{
    [Tooltip("Overrides artSetToDisplay and just shows the art list with the highest index")] public bool displayHighest;
    [Tooltip("Overrides artSetToDisplay and just shows the art list with the lowest index, which is 0")] public bool displayLowest;
    [Tooltip("Which set of art assets to show based on the list's index. The first number is always 0, followed by 1, 2, 3, etc")] public int artSetToDisplay;
    [Tooltip("This is a list that should contain all the sets of art for this item. The sets are themselves their own lists. Art assets should be fully separate from the functional assets")] public ArtList[] artLists;

    void Start()
    {
        if (displayHighest)
        {
            HideAll();
            foreach (var artObject in artLists[artLists.Length - 1].artObjects)
            {
                if (artObject != null)
                {
                    artObject.SetActive(true);
                }
            }
        }
        else if (displayLowest)
        {
            HideAll();
            foreach (var artObject in artLists[0].artObjects)
            {
                if (artObject != null)
                {
                    artObject.SetActive(true);
                }
            }
        }
        else
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
//List A for all temp art models
//List B for all art art models
//Ability to add more lists
//Int for which one to use

[Serializable]
public class ArtList
{
    public GameObject[] artObjects;
}