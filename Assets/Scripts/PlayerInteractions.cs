using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour {

    GameManageScript gameManager;
    List<string> inventory;
    public TMP_Text coinText;
    void Start() {
        gameManager = FindAnyObjectByType<GameManageScript>();
        inventory = new List<string>();
        Debug.Log(inventory.Count);
        coinText.text = "Coins: " + inventory.Count.ToString();
    }

    public List<string> GetInventoryData() {
        return inventory;
    }

    public void AddItemToInventory(string itemId) {
        inventory.Add(itemId);
        coinText.text = "Coins: " + inventory.Count.ToString();
    }

    void OnTriggerStay(Collider other) {
        if(other.tag == "Cave entrance") {
            if(Input.GetKeyDown(KeyCode.E)) {
                other.GetComponent<CaveEntrance>().SetPlayerPosition(GetComponent<Rigidbody>());
                gameManager.SetIsPlayerInCave(true);
            }
        }else if(other.tag == "Cave exit") {
            if(Input.GetKeyDown(KeyCode.E)) {
                gameManager.SetIsPlayerInCave(false);
                gameManager.ReturnToWorld();
            }
        }
    }

   

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Collectable") {
            AddItemToInventory(other.gameObject.GetComponent<Collectable>().GetItemId());
            
        }
    }

}
