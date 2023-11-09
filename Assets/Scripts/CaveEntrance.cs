using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveEntrance : MonoBehaviour
{
    string id;
    GameManageScript gameManager;
    bool playerInsideCave;
    
    void Start()
    {
        id = GetInstanceID().ToString();
        gameManager = FindAnyObjectByType<GameManageScript>();
        playerInsideCave = gameManager.CheckIfPlayerIsInCave();
    }

    void FixedUpdate() {
        if(gameManager.CheckIfPlayerIsInCave() != playerInsideCave) {
            playerInsideCave = gameManager.CheckIfPlayerIsInCave();
        }
    }

    public void SetPlayerPosition(Rigidbody player) {
        if(!playerInsideCave) {
            gameManager.setIsPlayerInCave(true);
            gameManager.SetPlayerLastPosition(player.transform.position);
            var playerSpawnPosition = gameManager.caveGenerator.caveSpawnPosition;
            var caveExitPosition = gameManager.caveGenerator.caveExitPosition;
            gameManager.mapGenerator.DisableMap();
            player.transform.position = playerSpawnPosition;
            var caveExitAsset = Resources.Load("Prefabs/CavernExit") as GameObject;
            GameObject caveExit = Instantiate(caveExitAsset);
            caveExit.transform.localPosition = caveExitPosition;
            caveExit.transform.parent = gameManager.caveGenerator.transform;
        }
    }

    

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            gameManager.GenerateCave(id);
        }
    }

    

    private void OnTriggerExit(Collider other) {

        if(other.tag == "Player") {
            if(playerInsideCave) {
                return;
            }else {
                if(gameManager.caveGenerator.cavesIds.Contains(id)) {
                    gameManager.caveGenerator.DestroyCave(id);
                }
            }
        }
    }
}

// public struct CaveEntranceCoord {
//     public float xCoord;
//     public float yCoord;

//     public CaveEntranceCoord(float xCoord, float yCoord)
//     {
//         this.xCoord = xCoord;
//         this.yCoord = yCoord;
//     }

