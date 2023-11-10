using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    string id;

    public void SetItemId(string itemId) {
        id = itemId;
    }

    public string GetItemId() {
        return id;
    }
}
