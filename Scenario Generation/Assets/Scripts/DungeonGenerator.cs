using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;
using UnityEngine;
using UnityEngine.Serialization;


public class DungeonGenerator : MonoBehaviour
{
    public List<GameObject> roomPrefabs;
    private static System.Random _rng =  new(_rngSeed);
    private List<GameObject> _dungeonRooms = new();
    [SerializeField]
    private List<GameObject> UnusedConnectors = new();

    public float StepDelay { get; set; } = 0.8f;

    private static int _rngSeed = 120;

    public int RngSeed
    {
        get => _rngSeed;
        set
        {
            _rngSeed = value;
            _rng = new(value);
        }
    }

    #region CONSTANTS

public static readonly int MAX_ROOMS = 100;
public static readonly int MIN_ROOMS = 0;

#endregion

private const int TotalRoomsToGenerate = 5; 
    
    // Start is called before the first frame update
    void Start()
    {
        // Filter Prefabs to ensure only valid rooms are included in the list
        // TODO: Fix this
        //roomPrefabs.RemoveAll(room => !room.IsValidRoom());
        
        StartCoroutine(GenerateDungeon());
    }

    IEnumerator GenerateDungeon()
    {
        Debug.Log("Generating dungeon");
        
        var firstRoom = Instantiate(GetRandomRoom(), transform);
        Debug.Log(firstRoom);
        
        yield return new WaitForSeconds(StepDelay);
        
        UnusedConnectors = firstRoom.GetAllChildrenWithTag("Connector");

        // start with one less room because we already generated the first one
        int roomsToGenerate = TotalRoomsToGenerate - 1;

        while (roomsToGenerate > 0)
        {
            // Select a connector
            int connectorIndex = _rng.Next(UnusedConnectors.Count);
            var connector = UnusedConnectors[connectorIndex];
            
            // Generate a room attached to that connector
            {
                // Get a random room
                var nextRoom = Instantiate(GetRandomRoom(), connector.transform.position, new Quaternion(), transform);;
                
                // Instantiate the room joining the base connector and one random connector from the new room
                var nextRoomConnectors = nextRoom.GetAllChildrenWithTag("Connector");
                var newRoomJoinConnector = nextRoomConnectors.ElementAt(_rng.Next(nextRoomConnectors.Count));
                
                /*
                // TODO: Delete line later
                newRoomJoinConnector.transform.position = new Vector3(newRoomJoinConnector.transform.position.x,
                    newRoomJoinConnector.transform.position.y + 5, newRoomJoinConnector.transform.position.z);
                    */

                // Rotate the room based on connectors
                var joinConnectorRotation = newRoomJoinConnector.transform.localEulerAngles;
                var newRoomYRotation = 360 - (connector.transform.eulerAngles.y + joinConnectorRotation.y);
                nextRoom.transform.eulerAngles = new Vector3(
                    nextRoom.transform.rotation.x,
                    newRoomYRotation,
                    nextRoom.transform.rotation.z
                    );
                
                // Change the rooms position based on connectors
                // Get the difference between where the new connector is now and the old connector
                var translationVector = connector.transform.position - newRoomJoinConnector.transform.position;
                // Move the whole room on that vector
                nextRoom.transform.position += translationVector;

                // Ensure new room does not collide with any previous rooms
                // If it does, select a new connector on the new room
                // Loop ^

                // If all connectors will cause a collision, get a different room

                // If there are no valid rooms, give up trying to use the connector
                // (Still remove it from the list as a known "failure" connector, since it can never be used)
                
                // Room must have generated successfully
                roomsToGenerate--;
                UnusedConnectors = UnusedConnectors.Concat(nextRoom.GetAllChildrenWithTag("Connector")).ToList();
                UnusedConnectors.Remove(newRoomJoinConnector);
            }
            
            // Remove connector from unused list
            UnusedConnectors.RemoveAt(connectorIndex);
            
            // Add all connectors from generated room to Unused list
            yield return new WaitForSeconds(StepDelay);;
        }
        
        // Turn some rooms into fancy rooms? Only do if time
        
        yield return null;
    }
    
    

    GameObject GetRandomRoom()
    {
        if (roomPrefabs.Count == 0)
            throw new Exception("Rooms list is empty");
            
        int index = _rng.Next(roomPrefabs.Count);
        return roomPrefabs[index];
    }
}