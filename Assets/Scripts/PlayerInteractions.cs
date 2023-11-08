using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour {

    void OnTriggerStay(Collider other) {
        if(other.tag == "Cave entrance") {
            if(Input.GetKey(KeyCode.E)) {
                other.GetComponent<CavernEntrance>().SetPlayerPosition(GetComponent<Rigidbody>());
            }
        }
    }

}
