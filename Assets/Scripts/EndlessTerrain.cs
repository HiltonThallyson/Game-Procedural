using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class EndlessTerrain : MonoBehaviour {
    const float scale = 1f;

    public Transform viewer;

    const float viewerMoveThresholdForChunkUpdate = 50f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public static Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    int chunkSize;
    int chunkVisibleInDistance;

    static MapGenerator mapGenerator;

    public Material mapMaterial;

    Dictionary<Vector2, TerrainChunck> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunck>();
    static List<TerrainChunck> terrainChuncksVisibleLastUpdate = new List<TerrainChunck>();

    void Start() {
        mapGenerator = FindAnyObjectByType<MapGenerator>();

        chunkSize = MapGenerator.mapChunkSize;
        chunkVisibleInDistance = 1;
        UpdateVisibleChunk();
    }

    void Update() {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / scale;
        if((viewerPositionOld - viewerPosition).sqrMagnitude  > sqrViewerMoveThresholdForChunkUpdate) {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunk();
        }
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
                }else {
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunck(viewedChunkCoord, chunkSize, transform, mapMaterial));
                }

            }
        }
    }

    public class TerrainChunck {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        MapData mapData;
        Mesh mesh;

        public TerrainChunck(Vector2 coord, int size, Transform parent, Material material) {
            position = coord * size;
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            bounds = new Bounds(position, Vector2.one * size);
            

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * scale;
            SetVisible(false);

            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        void OnMapDataReceived(MapData mapData) {
            this.mapData = mapData;
            mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);

            Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture  = texture;

        }

        void OnMeshDataReceived(MeshData meshData) {
            mesh = meshData.CreateMesh();
            meshFilter.mesh = mesh;
            UpdateTerrainChunk();
        }

        void OnTreeDataReceived(TreesData treesData) {
            var tree = Resources.Load("Prefabs/Tree") as GameObject;
            for (int y = 0; y < treesData.treesMap.GetLength(1); y++) {
                for (int x = 0; x < treesData.treesMap.GetLength(0); x++) {
                    if(treesData.treesMap[x,y] < 0.42f && treesData.treesMap[x,y] > 0.40f){
                       float height = mapGenerator.meshHeightCurve.Evaluate(mapData.heightMap[x,y]) * mapGenerator.meshHeightMultiplier;
                        GameObject trees = Instantiate(tree, Vector3.one, Quaternion.identity);
                       Vector3 treesPosition = new Vector3(position.x - (MapGenerator.mapChunkSize / 2f) + x, height + trees.transform.localScale.y, position.y + (MapGenerator.mapChunkSize/ 2f) - y);
                       trees.transform.localPosition = treesPosition;
                       trees.transform.parent = meshObject.transform;
                       trees.transform.position = treesPosition;
                       
                    }
                }
            }
        }

        void OnCavernsDataReceived(CavernsData cavernsData) {
            var cavernEntrance = Resources.Load("Prefabs/CavernEntrance") as GameObject;
            for (int y = 0; y < cavernsData.cavernMap.GetLength(1); y++) {
                for (int x = 0; x < cavernsData.cavernMap.GetLength(0); x++) {
                    if(cavernsData.cavernMap[x,y] < 0.75f && cavernsData.cavernMap[x,y] > 0.68f && cavernsData.cavernMap[x,y] != 0f){
                       float height = mapGenerator.meshHeightCurve.Evaluate(mapData.heightMap[x,y]) * mapGenerator.meshHeightMultiplier;
                        GameObject cavernEntrances = Instantiate(cavernEntrance, Vector3.one, Quaternion.identity);
                       Vector3 cavernEntrancesPosition = new Vector3(position.x - (MapGenerator.mapChunkSize / 2f) + x, height + cavernEntrances.transform.localScale.y, position.y + (MapGenerator.mapChunkSize/ 2f) - y);
                       cavernEntrances.transform.localPosition = cavernEntrancesPosition;
                       cavernEntrances.transform.parent = meshObject.transform;
                       cavernEntrances.transform.position = cavernEntrancesPosition;
                       
                    }
                }
            }
        }

       
        public void UpdateTerrainChunk () {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool isVisible = viewerDstFromNearestEdge <= 300;
            if(isVisible) {
                terrainChuncksVisibleLastUpdate.Add(this);
                meshCollider.sharedMesh = mesh;
                mapGenerator.RequestTreesData(position, OnTreeDataReceived);
                mapGenerator.RequestCavernsData(position, OnCavernsDataReceived);
            }
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

