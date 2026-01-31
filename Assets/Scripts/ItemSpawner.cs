using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public List<GameObject> listOfItems = new List<GameObject>();

    public Transform[] spawnPos;

    private void Start()
    {
        for (int i = 0; i < listOfItems.Count; i++)
        {
            GameObject item = listOfItems[i];
            Transform spawnLocation = spawnPos[i];
            Instantiate(item, spawnLocation);
        }
    }
}
