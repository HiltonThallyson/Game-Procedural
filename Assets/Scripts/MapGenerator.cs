using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public enum DrawMode {NoiseMap, ColorMap, Mesh, Trees, Caverns}

    public DrawMode drawMode;

    public const int mapChunkSize = 50;

    public Noise.NormalizeMode normalizeMode;
    
    public float noiseScale;

    public int seed;
    public Vector2 offset;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public bool autoUpdate;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public TerrainType[] regions; 

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();
    Queue<MapThreadInfo<TreesData>> treesDataThreadInfoQueue = new Queue<MapThreadInfo<TreesData>>();
    Queue<MapThreadInfo<CavernsData>> cavernsDataThreadInfoQueue = new Queue<MapThreadInfo<CavernsData>>();

    public void DrawMapInEditor() {

        MapData mapData = GenerateMapData(Vector2.zero);
        //Show map without running the game
        MapDisplay display = FindObjectOfType<MapDisplay>();

        if(drawMode == DrawMode.NoiseMap) {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        } else if (drawMode == DrawMode.ColorMap) {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        } else if (drawMode == DrawMode.Mesh) {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        } else if(drawMode == DrawMode.Trees) {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(TreesGenerator.GenerateTrees(mapChunkSize, mapChunkSize, noiseScale, seed, octaves, persistance, lacunarity, offset).treesMap));
        } else if(drawMode == DrawMode.Caverns) {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(CavernsGenerator.GenerateCaverns(mapChunkSize, mapChunkSize, noiseScale, seed, octaves, persistance, lacunarity, offset).cavernMap));
        }
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback) {
        ThreadStart threadStart = delegate {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 center, Action<MapData> callback) {
        MapData mapData = GenerateMapData(center);
        lock (mapDataThreadInfoQueue) {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, Action<MeshData> callBack) {
        ThreadStart threadStart = delegate {
            MeshDataThread(mapData, callBack);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, Action<MeshData> callback) {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve);
        lock (meshDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    public void RequestTreesData(Vector2 center, Action<TreesData> callback) {
        ThreadStart threadStart = delegate {
            TreesDataThread(center, callback);
        };
        new Thread(threadStart).Start();
    }

    void TreesDataThread(Vector2 center, Action<TreesData> callback) {
        TreesData treesData = TreesGenerator.GenerateTrees(mapChunkSize, mapChunkSize, noiseScale, seed, octaves, persistance, lacunarity, center + offset);
        lock(treesDataThreadInfoQueue) {
            treesDataThreadInfoQueue.Enqueue(new MapThreadInfo<TreesData>(callback, treesData));
        }
    }
    public void RequestCavernsData(Vector2 center, Action<CavernsData> callback) {
        ThreadStart threadStart = delegate {
            CavernsDataThread(center, callback);
        };
        new Thread(threadStart).Start();
    }

    void CavernsDataThread(Vector2 center, Action<CavernsData> callback) {
        CavernsData cavernsData = CavernsGenerator.GenerateCaverns(mapChunkSize, mapChunkSize, noiseScale, seed, octaves, persistance, lacunarity, center + offset);
        lock(cavernsDataThreadInfoQueue) {
            cavernsDataThreadInfoQueue.Enqueue(new MapThreadInfo<CavernsData>(callback, cavernsData));
        }
    }

    void Update() {
        if(mapDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if(meshDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if(treesDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < treesDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<TreesData> threadInfo = treesDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if(cavernsDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < cavernsDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<CavernsData> threadInfo = cavernsDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public void DisableMap() {
        gameObject.SetActive(false);
    }

    public void EnabledMap() {
        gameObject.SetActive(true);
    }

    MapData GenerateMapData(Vector2 center) {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseScale, seed, octaves, persistance, lacunarity, center + offset, normalizeMode);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++) {
            for (int x = 0; x < mapChunkSize; x++) {
                float currentHeight = noiseMap[x,y];
                for (int i = 0; i < regions.Length; i++) {
                    if(currentHeight >= regions[i].height) {
                        colorMap[x + y * mapChunkSize] = regions[i].color;
                    }else {
                        break;
                    }
                }
                 
            }
        }

        return new MapData(noiseMap, colorMap);
    }

    void OnValidate() {
        
        if (lacunarity < 1) {
            lacunarity = 1;
        }
        if (octaves < 0) {
            octaves = 0;
        }
    }

    struct MapThreadInfo<T> {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}

public struct MapData {
    public float[,] heightMap;
    public Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}

public struct TreesData {
    public float[,] treesMap;

    public TreesData(float[,] treesNoise)
    {
        this.treesMap = treesNoise;
    }

}

public struct CavernsData {
    public float[,] cavernMap;

    public CavernsData(float[,] cavernMap)
    {
        this.cavernMap = cavernMap;
    }
}
