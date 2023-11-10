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
    bool caveGenerated;

    void Start() {
        endlessTerrain = FindAnyObjectByType<EndlessTerrain>();
        GenerateMap();
        isPlayerInsideCave = false;
        caveGenerated = false;
    }

    public void SetIsPlayerInCave(bool isInside) {
        isPlayerInsideCave = isInside;
    }

    public bool CheckIfPlayerIsInCave(){
        return isPlayerInsideCave;
    }

    public void SetCaveIsGenerated(bool isGenerated) {
        caveGenerated = isGenerated;
    }

    public bool CheckIfCaveIsGenerated() {
        return caveGenerated;
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
        SetIsPlayerInCave(false);
        player.transform.position = playerLastPosition;
    }

}
