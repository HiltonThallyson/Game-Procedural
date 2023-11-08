using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavernEntrance : MonoBehaviour
{
    string id;
    CaveGenerator caveGenerator;
    MapGenerator mapGenerator;

    CaveEntranceCoord caveEntranceCoord;
    Transform playerEntrancePosition;

    bool isPlayerIn;

    void Start()
    {
        isPlayerIn = false;
        caveGenerator = FindAnyObjectByType<CaveGenerator>(); 
        mapGenerator = FindAnyObjectByType<MapGenerator>();
        caveEntranceCoord = new CaveEntranceCoord(transform.localPosition.x, transform.localPosition.z);
        id = GetInstanceID().ToString();
    }

    public void SetPlayerPosition(Rigidbody player) {
            var playerSpawnPosition = caveGenerator.caveSpawnPosition;
            mapGenerator.enabled = false;
            isPlayerIn = true;
            player.transform.position = playerSpawnPosition;
           
    }

    

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player" && !isPlayerIn) {
            if(!caveGenerator.cavesId.Contains(id)) {
                caveGenerator.GenerateMap(id);
            }
        }
    }

    

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player") {
            if(isPlayerIn) {
                return;
            }else {
                if(caveGenerator.cavesId.Contains(id)) {
                    caveGenerator.DestroyCave(id);
                }
            }
        }
    }
}

public struct CaveEntranceCoord {
    public float xCoord;
    public float yCoord;

    public CaveEntranceCoord(float xCoord, float yCoord)
    {
        this.xCoord = xCoord;
        this.yCoord = yCoord;
    }
}
