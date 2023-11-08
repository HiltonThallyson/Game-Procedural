using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManageScript : MonoBehaviour {

    EndlessTerrain endlessTerrain;
    public MapGenerator mapGenerator;
    public CaveGenerator caveGenerator;
    public GameObject player;
    Vector3 playerLastPosition;
    bool isPlayerInsideCave;

    void Start() {
        endlessTerrain = FindAnyObjectByType<EndlessTerrain>();
        GenerateMap();
        isPlayerInsideCave = false;
    }

    public void setIsPlayerInCave(bool isInside) {
        isPlayerInsideCave = isInside;
    }

    public bool CheckIfPlayerIsInCave(){
        return isPlayerInsideCave;
    }

     void  GenerateMap() {
        endlessTerrain.GenerateTerrain();
    }

    public void SetPlayerLastPosition(Vector3 position) {
        playerLastPosition = position;
    }

    public void GenerateCave(string id) {
        caveGenerator.GenerateCave(id);
    }

    public void ReturnToWorld() {
        mapGenerator.EnabledMap();
        setIsPlayerInCave(false);
        player.transform.position = playerLastPosition;
    }

}
