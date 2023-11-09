using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CollectablesGenerator {

    public static float[,] GenerateNoise(string seed, int width, int height) {
        return CollectablesNoise.GenerateNoise(seed, width, height);
    }
}
