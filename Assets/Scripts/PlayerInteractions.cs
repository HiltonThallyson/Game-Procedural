using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour {

    GameManageScript gameManager;
    bool isInsideCave;

    List<string> inventory;

    void Start() {
        gameManager = FindAnyObjectByType<GameManageScript>();
        inventory = new List<string>();
    }

    public List<string> GetInventoryData() {
        return inventory;
    }

    public void AddItemToInventory(string itemId) {
        inventory.Add(itemId);
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

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Collectable") {
            inventory.Add(other.gameObject.GetInstanceID().ToString());
        }
    }

}
