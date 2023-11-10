using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveEntrance : MonoBehaviour
{
    string id;
    GameManageScript gameManager;
    
    void Start()
    {
        id = "Cave Id " + gameObject.transform.position.x.ToString() + gameObject.transform.position.y.ToString() + gameObject.transform.position.z.ToString();
        gameManager = FindAnyObjectByType<GameManageScript>();
    }

    public void SetPlayerPosition(Rigidbody player) {
        if(!gameManager.CheckIfPlayerIsInCave()) {
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
            if(!gameManager.CheckIfCaveIsGenerated()) {
                gameManager.GenerateCave(id);
                gameManager.SetCaveIsGenerated(true);
            }
        }
    }

    

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player") {
            if(gameManager.CheckIfPlayerIsInCave()) {
                return;
            }else {
                if(gameManager.caveGenerator.cavesIds.Contains(id)) {
                    gameManager.SetCaveIsGenerated(false);
                    gameManager.caveGenerator.DestroyCave(id);
                }
            }
        }
    }
}



