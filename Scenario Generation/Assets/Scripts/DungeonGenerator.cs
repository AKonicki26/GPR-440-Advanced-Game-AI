using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DungeonGenerator : MonoBehaviour
{
    
    public List<GameObject> roomPrefabs;
    
    // Start is called before the first frame update
    void Start()
    {
        // Filter Prefabs to ensure only valid rooms are included in the list
        roomPrefabs.RemoveAll(room => !room.IsValidRoom());

        StartCoroutine(GenerateDungeon());
    }

    IEnumerator GenerateDungeon()
    {
        yield return null;
    }
}
