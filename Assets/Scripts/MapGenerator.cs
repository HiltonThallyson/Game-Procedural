using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public enum DrawMode {NoiseMap, ColorMap, Mesh, Collectables}

    public DrawMode drawMode;

    public const int mapChunkSize = 100;

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
    Queue<MapThreadInfo<CollectableData>> collectableDataThreadInfoQueue = new Queue<MapThreadInfo<CollectableData>>();

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
        } else if(drawMode == DrawMode.Collectables) {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(CollectablesGenerator.GenerateCollectables(mapChunkSize + 2, mapChunkSize + 2, noiseScale, seed, octaves, persistance, lacunarity, offset).collectablesMap));
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

    public void RequestCollectableData(Vector2 center, Action<CollectableData> callback) {
        ThreadStart threadStart = delegate {
            CollectableDataThread(center, callback);
        };
        new Thread(threadStart).Start();
    }

    void CollectableDataThread(Vector2 center, Action<CollectableData> callback) {
        CollectableData collectableData = CollectablesGenerator.GenerateCollectables(mapChunkSize + 2, mapChunkSize + 2, noiseScale, seed, octaves, persistance, lacunarity, center + offset);
        lock(collectableDataThreadInfoQueue) {
            collectableDataThreadInfoQueue.Enqueue(new MapThreadInfo<CollectableData>(callback, collectableData));
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
        if(collectableDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < collectableDataThreadInfoQueue.Count; i++) {
                MapThreadInfo<CollectableData> threadInfo = collectableDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
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

public struct CollectableData {
    public float[,] collectablesMap;

    public CollectableData(float[,] collectablesNoise)
    {
        this.collectablesMap = collectablesNoise;
    }

}
