using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour {
    public Transform viewer;

    public static Vector2 viewerPosition;
    int chunkSize;
    int chunkVisibleInDistance;

    Dictionary<Vector2, TerrainChunck> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunck>();
    List<TerrainChunck> terrainChuncksVisibleLastUpdate = new List<TerrainChunck>();

    void Start() {
        chunkSize = MapGenerator.mapChunkSize;
        chunkVisibleInDistance = 1;
    }

    void Update() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunk();
    }
    void UpdateVisibleChunk() {

        foreach(TerrainChunck lastVisibleTerrainChunk in terrainChuncksVisibleLastUpdate) {
            lastVisibleTerrainChunk.SetVisible(false);
        }
        terrainChuncksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunkVisibleInDistance; yOffset <= chunkVisibleInDistance; yOffset++) {
            for (int xOffset= -chunkVisibleInDistance; xOffset <= chunkVisibleInDistance; xOffset++) {
                Vector2 viewedChunkCoord = new Vector2 (currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if(terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if(terrainChunkDictionary[viewedChunkCoord].IsVisible()) {
                        terrainChuncksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    } 
                }else {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunck(viewedChunkCoord, chunkSize, transform));
                }

            }
        }
    }

    public class TerrainChunck {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        public TerrainChunck(Vector2 coord, int size, Transform parent) {
            position = coord * size;
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            bounds = new Bounds(position, Vector2.one * size);
            

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one * size / 10f;
            meshObject.transform.parent = parent;
            SetVisible(false);
        }

        public void UpdateTerrainChunk () {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool isVisible = viewerDstFromNearestEdge <= 300;
            SetVisible(isVisible);
        }

        public void SetVisible(bool visible) {
            meshObject.SetActive(visible);
        }

        public bool IsVisible() {
            return meshObject.activeSelf;
        }

    }

}

