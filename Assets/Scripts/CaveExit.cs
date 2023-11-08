using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveExit : MonoBehaviour {
    GameManageScript gameManager;
    bool isPlayerInsideCave;

    void Start() {
        gameManager = FindAnyObjectByType<GameManageScript>();
    }
    void Update() {
        isPlayerInsideCave = gameManager.CheckIfPlayerIsInCave();
        if(!isPlayerInsideCave) {
            Destroy(gameObject);
        }
    }


    
}
