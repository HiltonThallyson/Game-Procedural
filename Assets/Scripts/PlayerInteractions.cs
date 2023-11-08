using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour {

    GameManageScript gameManager;
    bool isInsideCave;

    void Start() {
        gameManager = FindAnyObjectByType<GameManageScript>();
    }

    void OnTriggerStay(Collider other) {
        isInsideCave = gameManager.CheckIfPlayerIsInCave();
        if(other.tag == "Cave entrance") {
            if(Input.GetKeyDown(KeyCode.E) && !isInsideCave) {
                gameManager.setIsPlayerInCave(true);
                isInsideCave = !isInsideCave;
                other.GetComponent<CaveEntrance>().SetPlayerPosition(GetComponent<Rigidbody>());
            }
        }else if(other.tag == "Cave exit") {
            if(Input.GetKeyDown(KeyCode.E) && isInsideCave) {
                gameManager.setIsPlayerInCave(false);
                isInsideCave = !isInsideCave;
                gameManager.ReturnToWorld();
            }
        }
    }

}
